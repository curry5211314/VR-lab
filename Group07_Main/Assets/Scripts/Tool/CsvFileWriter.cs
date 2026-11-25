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
    string configPath;
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
        //configPath = Path.Combine(folderPath, "Config.cfg");
       

        // 仅在文件不存在时创建并写入表头
        if (!File.Exists(filePath))
        {
            CreateCsvFileWithHeader();
        }
        else
        {
            Debug.Log($"CSV文件已存在，无需重复创建: {filePath}");
        }
        // 确保文件夹存在
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

            // 定义CSV表头
            List<string[]> fitDataHeader = new List<string[]>()
            {
                new string[] { "User",  "Hand" , "Time", "Action", "Difficulty", "Star", "Angle_X", "Angle_Y" , "Angle_Z" }  //设备id,手,时间，动作名称，星级，角度
            };

            // 写入表头数据
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (string[] data in fitDataHeader)
                {
                    string line = string.Join(",", data);
                    writer.WriteLine(line);
                }
            }

            Debug.Log($"CSV文件创建成功: {filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"创建CSV文件失败: {e.Message}");
        }
    }


    public void SetDataToCsvFile(string action,int diff, int star,Vector3 angle,string hand)
    {
        star = star>=0?star:0;
        string time  = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        // 拆分Vector3为X/Y/Z三个字段（保留1位小数，避免冗余）
        string angleX = angle.x.ToString("F1");
        string angleY = angle.y.ToString("F1");
        string angleZ = angle.z.ToString("F1");

        diff -= 1;
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
            new string[] {androidID, hand,time, action, diffData,star.ToString(),angleX,angleY,angleZ}   //设备id,时间，动作名称，星级，角度
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
