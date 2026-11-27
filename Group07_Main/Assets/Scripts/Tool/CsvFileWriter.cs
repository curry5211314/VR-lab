using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class CsvFileWriter : MonoBehaviour
{

    public static CsvFileWriter _instance;
    public Text tt_csv;
    string androidID;

    string folderPath;
    string filePath;

    // 表头新增命中、失误、用时三列，结算数据可直接对账
    static readonly string[] HeaderColumns = { "User", "Hand", "Time", "Action", "Difficulty", "Star", "Angle_X", "Angle_Y", "Angle_Z", "HitCount", "MissCount", "Duration" };
    static readonly string HeaderLine = string.Join(",", HeaderColumns);

    private void Awake()
    {
        _instance= this;
    }
    void Start()
    {
        androidID = SystemInfo.deviceUniqueIdentifier;
        folderPath = Path.Combine(Application.persistentDataPath, "APPData");
        Directory.CreateDirectory(folderPath);
        filePath = Path.Combine(folderPath, "FitData.csv");

        if (!File.Exists(filePath))
        {
            CreateCsvFileWithHeader();
        }
        else if (!HasCurrentHeader())
        {
            // 旧表头缺少命中/失误/用时列，先改名备份再建新表，避免新旧行错位
            MigrateOldFile();
        }
        else
        {
            Debug.Log($"CSV文件已存在，无需重复创建: {filePath}");
        }
    }

    private bool HasCurrentHeader()
    {
        try
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                string first = reader.ReadLine();
                return first != null && first.Trim() == HeaderLine;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"读取CSV表头失败: {e.Message}");
            return false;
        }
    }

    private void MigrateOldFile()
    {
        string legacyPath = Path.Combine(folderPath, "FitData_legacy_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv");
        try
        {
            File.Move(filePath, legacyPath);
            Debug.Log($"旧版 FitData.csv 已备份为: {legacyPath}");
            CreateCsvFileWithHeader();
        }
        catch (Exception e)
        {
            Debug.LogError($"备份旧版 FitData.csv 失败: {e.Message}");
        }
    }


    /// <summary>
    /// 创建CSV文件并写入表头
    /// </summary>
    private void CreateCsvFileWithHeader()
    {
        try
        {
            // 确保文件夹存在
            Directory.CreateDirectory(folderPath);

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine(HeaderLine);
            }

            Debug.Log($"CSV文件创建成功: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"创建CSV文件失败: {e.Message}");
        }
    }


    public void SetDataToCsvFile(string action,int diff, int star,Vector3 angle,string hand,int hitCount,int missCount,float durationSeconds)
    {
        star = star>=0?star:0;
        string time  = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        // 拆分Vector3为X/Y/Z三个字段（保留1位小数，避免冗余）
        string angleX = angle.x.ToString("F1");
        string angleY = angle.y.ToString("F1");
        string angleZ = angle.z.ToString("F1");

        // 服务端难度统一按 0 基（0 简单 / 1 中等 / 2 困难），不再做 -1 换算
        // 转换为枚举
        Difficulty difficulty = (Difficulty)diff;
        string diffData = "";
        // 使用示例
        switch (difficulty)
        {
            case Difficulty.easy:
                diffData = "简单难度";
                break;
            case Difficulty.medium:
                diffData = "中等难度";
                break;
            case Difficulty.hard:
                diffData = "困难难度";
                break;
            default:
                diffData = "无效难度";
                break;
        }
        
        List<string[]> fitData = new List<string[]>()
        {
            new string[] {androidID, hand,time, action, diffData,star.ToString(),angleX,angleY,angleZ,hitCount.ToString(),missCount.ToString(),durationSeconds.ToString("F1")}   //设备id,手,时间，动作名称，星级，角度，命中，失误，用时
        };

        // 写入 CSV
        using (StreamWriter sw = new StreamWriter(filePath,true))
        {
            foreach (string[] row in fitData)
            {
                sw.WriteLine(string.Join(",", row));
            }
        }
        Debug.Log($"CSV添加成功！: {filePath}");
    }

    //public void OnReadCsv(string csvfilePath)
    //{
    //    string[] lines = File.ReadAllLines(csvfilePath);
    //    foreach (string line in lines)
    //    {
    //        string[] cols = line.Split(',');
    //        Debug.Log("User: " + cols[0] + ", Score: " + cols[1]);
    //        tt_csv.text += cols[0] + " " + cols[1];
    //    }
    //}
}
