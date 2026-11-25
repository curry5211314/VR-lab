using System;
using System.Net;
using System.Text;
using UnityEngine;

/// <summary>
/// UDP数据解析辅助类
/// 提供常用的数据编码/解码、打包/解包功能
/// </summary>
public static class UDPDataHelper
{
    /// <summary>
    /// 将字节数组转换为UTF8字符串
    /// </summary>
    /// <param name="data">字节数组</param>
    /// <returns>UTF8字符串</returns>
    public static string BytesToUTF8String(byte[] data)
    {
        if (data == null || data.Length == 0)
            return string.Empty;

        try
        {
            return Encoding.UTF8.GetString(data);
        }
        catch (Exception e)
        {
            Debug.LogError($"字节数组转字符串失败：{e.Message}");
            return string.Empty;
        }
    }

    /// <summary>
    /// 将字符串转换为UTF8字节数组
    /// </summary>
    /// <param name="str">字符串</param>
    /// <returns>UTF8字节数组</returns>
    public static byte[] UTF8StringToBytes(string str)
    {
        if (string.IsNullOrEmpty(str))
            return Array.Empty<byte>();

        try
        {
            return Encoding.UTF8.GetBytes(str);
        }
        catch (Exception e)
        {
            Debug.LogError($"字符串转字节数组失败：{e.Message}");
            return Array.Empty<byte>();
        }
    }

    /// <summary>
    /// 打包数据（添加头部标识，便于解析）
    /// </summary>
    /// <param name="data">原始数据</param>
    /// <param name="dataType">数据类型标识</param>
    /// <returns>打包后的字节数组</returns>
    public static byte[] PackData(byte[] data, int dataType)
    {
        if (data == null)
            return Array.Empty<byte>();

        // 头部：4字节数据类型 + 4字节数据长度 + 数据内容
        byte[] typeBytes = BitConverter.GetBytes(dataType);
        byte[] lengthBytes = BitConverter.GetBytes(data.Length);

        byte[] packedData = new byte[8 + data.Length];
        Buffer.BlockCopy(typeBytes, 0, packedData, 0, 4);
        Buffer.BlockCopy(lengthBytes, 0, packedData, 4, 4);
        Buffer.BlockCopy(data, 0, packedData, 8, data.Length);

        return packedData;
    }

    /// <summary>
    /// 解包数据
    /// </summary>
    /// <param name="packedData">打包后的字节数组</param>
    /// <param name="dataType">解析出的数据类型</param>
    /// <returns>原始数据</returns>
    public static byte[] UnpackData(byte[] packedData, out int dataType)
    {
        dataType = 0;

        if (packedData == null || packedData.Length < 8)
            return Array.Empty<byte>();

        try
        {
            // 解析头部
            dataType = BitConverter.ToInt32(packedData, 0);
            int dataLength = BitConverter.ToInt32(packedData, 4);

            // 验证数据长度
            if (dataLength <= 0 || dataLength > packedData.Length - 8)
                return Array.Empty<byte>();

            // 提取数据内容
            byte[] data = new byte[dataLength];
            Buffer.BlockCopy(packedData, 8, data, 0, dataLength);

            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"解包数据失败：{e.Message}");
            return Array.Empty<byte>();
        }
    }

    /// <summary>
    /// 格式化IPEndPoint为字符串（IP:Port）
    /// </summary>
    /// <param name="endPoint">网络端点</param>
    /// <returns>格式化字符串</returns>
    public static string FormatEndPoint(IPEndPoint endPoint)
    {
        if (endPoint == null)
            return "未知地址";

        return $"{endPoint.Address}:{endPoint.Port}";
    }
}