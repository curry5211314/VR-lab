using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

/// <summary>
/// UDP通信管理类（单例）
/// 适配Unity安卓平台，支持异步发送和接收UDP数据
/// </summary>
public class UDPManager : MonoBehaviour
{
    // 单例实例
    private static UDPManager _instance;
    public static UDPManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject obj = new GameObject("UDPManager");
                _instance = obj.AddComponent<UDPManager>();
                DontDestroyOnLoad(obj);
            }
            return _instance;
        }
    }

    // UDP客户端对象
    private UdpClient _udpClient;
    // 接收数据的线程
    private Thread _receiveThread;
    // 远端服务器地址
    private IPEndPoint _remoteEndPoint;
    // 本地监听端口
    private int _localListeningPort;
    // 是否正在运行
    private bool _isRunning = false;

    // 数据接收回调（主线程执行）
    public Action<byte[], IPEndPoint> OnDataReceived;
    // 错误回调
    public Action<string> OnErrorOccurred;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 初始化UDP客户端
    /// </summary>
    /// <param name="localPort">本地监听端口</param>
    /// <param name="remoteIp">远端IP地址</param>
    /// <param name="remotePort">远端端口</param>
    public void InitUDP(int localPort, string remoteIp = null, int remotePort = 0)
    {
        try
        {
            // 停止已有连接
            StopUDP();

            _localListeningPort = localPort;
            _udpClient = new UdpClient(localPort);
            _udpClient.Client.ReceiveTimeout = 1000; // 设置接收超时，避免卡死

            // 设置远端地址（可选）
            if (!string.IsNullOrEmpty(remoteIp) && remotePort > 0)
            {
                _remoteEndPoint = new IPEndPoint(IPAddress.Parse(remoteIp), remotePort);
            }
            else
            {
                _remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
            }

            _isRunning = true;

            // 启动接收线程
            _receiveThread = new Thread(ReceiveDataLoop);
            _receiveThread.IsBackground = true; // 后台线程，退出时自动销毁
            _receiveThread.Start();

            Debug.Log($"UDP客户端初始化成功，本地监听端口：{localPort}");
        }
        catch (Exception e)
        {
            OnErrorOccurred?.Invoke($"初始化UDP失败：{e.Message}");
            Debug.LogError($"初始化UDP失败：{e}");
        }
    }

    /// <summary>
    /// 接收数据循环（后台线程）
    /// </summary>
    private void ReceiveDataLoop()
    {
        while (_isRunning && _udpClient != null)
        {
            try
            {
                if (_udpClient.Available > 0)
                {
                    // 接收数据
                    byte[] receivedBytes = _udpClient.Receive(ref _remoteEndPoint);
                    if (receivedBytes != null && receivedBytes.Length > 0)
                    {
                        // 在主线程中执行回调
                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            OnDataReceived?.Invoke(receivedBytes, _remoteEndPoint);
                        });
                    }
                }
                else
                {
                    Thread.Sleep(10); // 减少CPU占用
                }
            }
            catch (SocketException ex)
            {
                // 忽略超时异常
                if (ex.SocketErrorCode != SocketError.TimedOut && _isRunning)
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        OnErrorOccurred?.Invoke($"接收数据异常：{ex.Message}");
                    });
                    Debug.LogWarning($"UDP接收异常：{ex.Message}");
                }
            }
            catch (Exception e)
            {
                if (_isRunning)
                {
                    UnityMainThreadDispatcher.Instance.Enqueue(() =>
                    {
                        OnErrorOccurred?.Invoke($"接收数据错误：{e.Message}");
                    });
                    Debug.LogError($"UDP接收错误：{e}");
                }
            }
        }
    }

    /// <summary>
    /// 发送字节数组数据
    /// </summary>
    /// <param name="data">要发送的字节数组</param>
    /// <param name="targetIp">目标IP（为空则使用初始化时的远端地址）</param>
    /// <param name="targetPort">目标端口（为0则使用初始化时的远端端口）</param>
    public void SendData(byte[] data, string targetIp = null, int targetPort = 0)
    {
        if (!_isRunning || _udpClient == null || data == null || data.Length == 0)
        {
            OnErrorOccurred?.Invoke("UDP未初始化或发送数据为空");
            return;
        }

        try
        {
            IPEndPoint targetEndPoint = _remoteEndPoint;

            // 使用指定的目标地址
            if (!string.IsNullOrEmpty(targetIp) && targetPort > 0)
            {
                targetEndPoint = new IPEndPoint(IPAddress.Parse(targetIp), targetPort);
            }

            // 异步发送数据
            _udpClient.SendAsync(data, data.Length, targetEndPoint)
                .ContinueWith(task =>
                {
                    if (task.Exception != null)
                    {
                        UnityMainThreadDispatcher.Instance.Enqueue(() =>
                        {
                            OnErrorOccurred?.Invoke($"发送数据失败：{task.Exception.InnerException?.Message}");
                        });
                        Debug.LogError($"UDP发送失败：{task.Exception}");
                    }
                });
        }
        catch (Exception e)
        {
            OnErrorOccurred?.Invoke($"发送数据异常：{e.Message}");
            Debug.LogError($"UDP发送异常：{e}");
        }
    }

    /// <summary>
    /// 发送字符串数据（UTF8编码）
    /// </summary>
    /// <param name="message">要发送的字符串</param>
    /// <param name="targetIp">目标IP</param>
    /// <param name="targetPort">目标端口</param>
    public void SendString(string message, string targetIp = null, int targetPort = 0)
    {
        if (string.IsNullOrEmpty(message))
        {
            OnErrorOccurred?.Invoke("发送的字符串为空");
            return;
        }

        byte[] data = Encoding.UTF8.GetBytes(message);
        SendData(data, targetIp, targetPort);
    }

    /// <summary>
    /// 停止UDP通信
    /// </summary>
    public void StopUDP()
    {
        _isRunning = false;

        // 停止接收线程
        if (_receiveThread != null && _receiveThread.IsAlive)
        {
            try
            {
                _receiveThread.Join(1000); // 等待线程结束
            }
            catch { }
            _receiveThread = null;
        }

        // 释放UDP客户端
        if (_udpClient != null)
        {
            try
            {
                _udpClient.Close();
                _udpClient.Dispose();
            }
            catch { }
            _udpClient = null;
        }

        _remoteEndPoint = null;
        Debug.Log("UDP客户端已停止");
    }

    /// <summary>
    /// 获取当前UDP运行状态
    /// </summary>
    /// <returns>是否正在运行</returns>
    public bool IsUDPRunning()
    {
        return _isRunning && _udpClient != null;
    }

    private void OnApplicationQuit()
    {
        StopUDP();
    }

    private void OnDestroy()
    {
        StopUDP();
        if (_instance == this)
        {
            _instance = null;
        }
    }
}