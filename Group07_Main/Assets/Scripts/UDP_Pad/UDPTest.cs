using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UDP使用示例
/// </summary>
public class UDPTest : MonoBehaviour
{
    [Header("UDP配置")]
    public int localPort = 8888;
    public string remoteIp = "192.168.1.100";
    public int remotePort = 9999;
    public Text tt_log;
    public Button btn_send;
    public TextMeshProUGUI tt_myip;

    public TMP_InputField ipInput;
    public Button btnLink;

    UDPManager udpManager;


    //private void Awake()
    //{
    //    if (_instance == null)
    //    {
    //        // 1. 赋值单例
    //        _instance = this;
    //        // 2. 标记该物体跨场景不销毁
    //        DontDestroyOnLoad(gameObject);
    //    }
    //    else
    //    {
    //        Destroy(gameObject);
    //        // 提前返回，避免重复执行后续初始化逻辑
    //        return;
    //    }
    //}

    private void Start()
    {
        
        // 生命周期内只订阅一次，先订阅以接收初始化错误。
        udpManager = UDPManager.Instance;
        udpManager.OnDataReceived += OnUDPDataReceived;
        udpManager.OnErrorOccurred += OnUDPError;
        udpManager.InitUDP(localPort, remoteIp, remotePort);
        //// 测试发送数据
        //Invoke("SendTestData", 2f);

        // 旧控制面板删除后，这些 UI 引用可能为空，按可选处理。
        if (btn_send != null) btn_send.onClick.AddListener(SendTestData);

        if (ipInput != null) ipInput.onValueChanged.AddListener(OnSetIp);
        if (btnLink != null) btnLink.onClick.AddListener(OnLinkNet);

        //// 2. 读取各种类型数据（带默认值）
        //string padIP = SimplePlayerPrefs.GetString("RemoteIP", "192.168.0.1");
        //print("默认192.168.0.1 ?" + padIP);
        //MainControl._instance.AddLog(padIP);
        if (tt_myip != null) tt_myip.text = GetLocalIpAddress();
    }


    void OnSetIp(string ipString)
    {
        IPAddress ipa = null;
        //检测是否是ip
        //if (IPAddress.TryParse(ipString, out IPAddress? _))
        if (IPAddress.TryParse(ipString, out ipa))
        {
            // 1. 保存各种类型数据
            SimplePlayerPrefs.SetString("RemoteIP", remoteIp);
            print(ipString + "没问题");
            remoteIp = ipString;
            //ipInput.textComponent.color = Color.green;
        }
        else
        {
            //ipInput.textComponent.color = Color.red;
            print(ipString + "不对");
        }
    }

    void OnLinkNet()
    {
        // 重新连接只初始化通信，不重复订阅回调。
        if (udpManager != null)
            udpManager.InitUDP(localPort, remoteIp, remotePort);
    }


    /// <summary>
    /// 发送测试数据
    /// </summary>
    private void SendTestData()
    {
        if (UDPManager.Instance.IsUDPRunning())
        {
            // 发送字符串
            UDPManager.Instance.SendString("Hello UDP from Unity Android!", remoteIp, remotePort);
            tt_log.text += "发送字符串:Hello UDP from Unity Android!";

            // 发送字节数组
            byte[] testData = new byte[] { 0x01, 0x02, 0x03, 0x04 };
            UDPManager.Instance.SendData(testData, remoteIp, remotePort);
            tt_log.text += "发送字符串:{ 0x01, 0x02, 0x03, 0x04 }";

            // 使用打包数据发送
            byte[] packedData = UDPDataHelper.PackData(UDPDataHelper.UTF8StringToBytes("打包的数据"), 1);
            UDPManager.Instance.SendData(packedData, remoteIp, remotePort);
            tt_log.text += "打包数据发送:打包的数据";
            Debug.Log("测试数据发送完成");
        }
        else
        {
            Debug.LogError("UDP未运行，无法发送数据");
        }
    }


    public void SendSscore()
    {
        if (UDPManager.Instance.IsUDPRunning())
        {
            string score = "score_" + MainControl._instance.score;
            // 发送字符串
            UDPManager.Instance.SendString("score_", remoteIp, remotePort);
            tt_log.text += "发送字符串:Hello UDP from Unity Android!";
        }
    }


    /// <summary>
    /// UDP数据接收回调
    /// </summary>
    /// <param name="data">接收的字节数组</param>
    /// <param name="endPoint">发送方地址</param>
    private void OnUDPDataReceived(byte[] data, IPEndPoint endPoint)
    {
        string sender = UDPDataHelper.FormatEndPoint(endPoint);
        string strData = UDPDataHelper.BytesToUTF8String(data);
        tt_log.text += $"收到来自 {sender} 的数据：{strData}";
        Debug.Log($"收到来自 {sender} 的数据：{strData}");
        BtnDownList._instance.OnReceivedDataReader(strData);


        // 尝试解包数据
        int dataType;
        byte[] unpackedData = UDPDataHelper.UnpackData(data, out dataType);
        if (unpackedData.Length > 0)
        {
            string unpackedStr = UDPDataHelper.BytesToUTF8String(unpackedData);
            Debug.Log($"解包后的数据（类型：{dataType}）：{unpackedStr}");
            MainControl._instance.AddLog( $"收到来自 {sender} 的数据：{strData}");
        }
    }

    /// <summary>
    /// UDP错误回调
    /// </summary>
    /// <param name="errorMsg">错误信息</param>
    private void OnUDPError(string errorMsg)
    {
        Debug.LogError($"UDP错误：{errorMsg}");
    }

    private void OnDestroy()
    {
        // 使用已有实例，避免退出时通过单例访问器创建新对象。
        if (udpManager == null) return;
        udpManager.OnDataReceived -= OnUDPDataReceived;
        udpManager.OnErrorOccurred -= OnUDPError;
        udpManager.StopUDP();
    }

    public string GetLocalIpAddress()
    {
        try
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                // 连接到公共 DNS 服务器（不会真正建立连接，只是用来获取本地出口 IP）
                socket.Connect("8.8.8.8", 80);
                var endPoint = socket.LocalEndPoint as IPEndPoint;
                Debug.Log("endPoint?.Address:" + endPoint?.Address.ToString());
                return endPoint?.Address.ToString() ?? "127.0.0.1";
            }

            //using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            //{
            //    socket.Connect("192.168.0.1", 80);
            //    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
            //    return endPoint?.Address.ToString() ?? "127.0.0.1";
            //}
        }
        catch
        {
            return "127.0.0.1";
        }
    }


}