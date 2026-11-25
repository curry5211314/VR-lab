using UnityEngine;

/// <summary>
/// 简化版 PlayerPrefs 工具类
/// 仅封装核心基础类型，轻便易用
/// </summary>
public static class SimplePlayerPrefs
{
    #region 基础类型存取（核心功能）
    /// <summary>
    /// 保存 int 类型数据
    /// </summary>
    /// <param name="key">存储键名</param>
    /// <param name="value">要存储的值</param>
    public static void SetInt(string key, int value)
    {
        PlayerPrefs.SetInt(key, value);
    }

    /// <summary>
    /// 读取 int 类型数据（带默认值，避免键不存在报错）
    /// </summary>
    /// <param name="key">存储键名</param>
    /// <param name="defaultValue">键不存在时返回的默认值</param>
    /// <returns>存储的值或默认值</returns>
    public static int GetInt(string key, int defaultValue = 0)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetInt(key) : defaultValue;
    }

    /// <summary>
    /// 保存 float 类型数据
    /// </summary>
    public static void SetFloat(string key, float value)
    {
        PlayerPrefs.SetFloat(key, value);
    }

    /// <summary>
    /// 读取 float 类型数据（带默认值）
    /// </summary>
    public static float GetFloat(string key, float defaultValue = 0f)
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetFloat(key) : defaultValue;
    }

    /// <summary>
    /// 保存 string 类型数据
    /// </summary>
    public static void SetString(string key, string value)
    {
        // 避免存入 null 导致异常，默认转为空字符串
        value = value ?? string.Empty;
        PlayerPrefs.SetString(key, value);
    }

    /// <summary>
    /// 读取 string 类型数据（带默认值）
    /// </summary>
    public static string GetString(string key, string defaultValue = "")
    {
        return PlayerPrefs.HasKey(key) ? PlayerPrefs.GetString(key) : defaultValue;
    }

    /// <summary>
    /// 保存 bool 类型数据（原生 PlayerPrefs 不支持，转为 int 存储）
    /// </summary>
    public static void SetBool(string key, bool value)
    {
        // true 存 1，false 存 0
        SetInt(key, value ? 1 : 0);
    }

    /// <summary>
    /// 读取 bool 类型数据（带默认值）
    /// </summary>
    public static bool GetBool(string key, bool defaultValue = false)
    {
        // 读取 int 值，1 转为 true，0 转为 false
        return GetInt(key, defaultValue ? 1 : 0) == 1;
    }
    #endregion

    #region 辅助工具方法（可选，简化常用操作）
    /// <summary>
    /// 移除指定键值对
    /// </summary>
    public static void DeleteKey(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }

    /// <summary>
    /// 清空所有 PlayerPrefs 数据（谨慎使用！）
    /// </summary>
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteAll();
    }

    /// <summary>
    /// 提交修改（将数据写入本地文件，Unity 会自动提交，特殊场景手动调用）
    /// </summary>
    public static void Save()
    {
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 检查指定键是否存在
    /// </summary>
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    #endregion
}