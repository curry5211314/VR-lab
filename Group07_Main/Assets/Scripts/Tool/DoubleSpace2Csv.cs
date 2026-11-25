using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


//
public class DoubleSpace2Csv : MonoBehaviour
{

    public static DoubleSpace2Csv _instance;
    [Tooltip("基本文件名，不含后缀")]
    public string baseName = "FitRecord";
    public int maxRowsPerFile = 108_000;   // 每个文件最多 108000 条数据

    readonly List<string> buffer = new();
    bool recording;
    bool applicationPaused;
    bool skipResumeFrame;
    float timer;
    const float interval = 1f / 30f;
    int frameIndex;
    int fileIndex;
    int rowsInCurrFile;   // 当前文件已写行数（不含表头）

    string folderPath => Path.Combine(Application.persistentDataPath, "APPData");
    string CurrPath;
    string sessionId;
    float flushTimer;
    const float flushInterval = 5f;
    const string Header = "User,Action,Difficulty,Arm length,HandType,DateTime,Score,StarL,StarR,Pain,frameIndex," +
                "SHOULDER_posX_L, SHOULDER_posY_L, SHOULDER_posZ_L,ELBOW_posX_L, ELBOW_posY_L, ELBOW_posZ_L,HAND_posX_L, HAND_posY_L, HAND_posZ_L," +
                "SHOULDER_posX_R, SHOULDER_posY_R, SHOULDER_posZ_R,ELBOW_posX_R, ELBOW_posY_R, ELBOW_posZ_R,HAND_posX_R, HAND_posY_R, HAND_posZ_R";

    public Transform tf_shoulderL;
    public Transform tf_elbowL;
    public Transform tf_handL;
    //LEFT_WRIST —— 左腕部 
    public Transform tf_shoulderR;
    public Transform tf_elbowR;
    public Transform tf_handR;

    private void Awake()
    {
        _instance = this;
    }

    string androidID;
    private void Start()
    {
        androidID = SystemInfo.deviceUniqueIdentifier;
    }

    public void StartStopRec()
    {
        if (recording)
        {
            StopRecording();
            return;
        }

        Directory.CreateDirectory(folderPath);
        sessionId = DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + "_" + Guid.NewGuid().ToString("N");
        fileIndex = 0;
        frameIndex = 0;
        timer = 0f;
        flushTimer = 0f;
        buffer.Clear();
        OpenCurrentFile();
        recording = true;
        Debug.Log($"开始记录（30 次/秒）→ {CurrPath}");
    }

    void OpenCurrentFile()
    {
        CurrPath = Path.Combine(folderPath, $"{baseName}_{sessionId}_{action}_{fileIndex:D2}.csv");
        // CreateNew 防止意外覆盖已有记录；每个分文件使用相同表头。
        using (var stream = new FileStream(CurrPath, FileMode.CreateNew, FileAccess.Write))
        using (var writer = new StreamWriter(stream))
        {
            writer.WriteLine(Header);
        }
        rowsInCurrFile = 0;
    }

    public void StopRecording()
    {
        if (!recording) return;
        FlushToDisk();
        recording = false;
    }

    public void SetStartData(string act,int diffic,int shoulderLeng,string handtype)
    {
        StopRecording();
        action = act;
        diff = GetDataDiff(diffic);
        armL = shoulderLeng.ToString() + "cm";
        handTP = handtype;
        fileIndex = 0;
        StartStopRec();
    }

    public string action, diff, handTP, Datime,pain,armL;
    public int sco, triggerStarL, triggerStarR;
    public float shd_Lx;

    void Update()
    {
        if (!recording) return;

        if (applicationPaused) return;
        if (skipResumeFrame)
        {
            skipResumeFrame = false;
            return;
        }

        // 30 Hz 采样
        timer += Time.unscaledDeltaTime;
        while (timer >= interval)
        {
            timer -= interval;
            RecFIT("NONE");
        }

        flushTimer += Time.unscaledDeltaTime;
        if (flushTimer >= flushInterval)
        {
            FlushToDisk();
            flushTimer = 0f;
        }
    }


    public void RecFIT(string tempPain)
    {
        if (!recording) return;

        // 在下一条记录到来时切文件，避免停止时多生成空的分文件。
        if (rowsInCurrFile >= Mathf.Max(1, maxRowsPerFile))
        {
            FlushToDisk();
            fileIndex++;
            OpenCurrentFile();
        }

        string Datime = DateTime.Now.ToString("MM-dd HH:mm:ss");
        sco = MainControl._instance.score;
        triggerStarL = MainControl._instance.triggerIndex_L;
        triggerStarR = MainControl._instance.triggerIndex_R;
        pain = tempPain;
        frameIndex++;

        buffer.Add($"{androidID},{action},{diff},{armL},{handTP},{Datime},{sco},{triggerStarL},{triggerStarR},{pain},{frameIndex}," +
                   $"{tf_shoulderL.position.x},{tf_shoulderL.position.y},{tf_shoulderL.position.z}," +
                   $"{tf_elbowL.position.x},{tf_elbowL.position.y},{tf_elbowL.position.z}," +
                   $"{tf_handL.position.x},{tf_handL.position.y},{tf_handL.position.z}," +
                   $"{tf_shoulderR.position.x},{tf_shoulderR.position.y},{tf_shoulderR.position.z}," +
                   $"{tf_elbowR.position.x},{tf_elbowR.position.y},{tf_elbowR.position.z}," +
                   $"{tf_handR.position.x},{tf_handR.position.y},{tf_handR.position.z}");
        rowsInCurrFile++;
    }

    void FlushToDisk()
    {
        if (buffer.Count == 0) return;
        File.AppendAllLines(CurrPath, buffer);
        buffer.Clear();
    }

    void OnApplicationPause(bool paused)
    {
        applicationPaused = paused;
        skipResumeFrame = !paused;
        if (!recording) return;
        if (paused) FlushToDisk();
        // 恢复后重新计时，避免补写暂停期间不存在的采样。
        timer = 0f;
        flushTimer = 0f;
    }

    void OnApplicationQuit()
    {
        StopRecording();
    }

    void OnDisable()
    {
        StopRecording();
    }


    public string GetDataDiff(int diffInde)
    {
        diffInde -= 1;
        // 转换为枚举
        Difficulty difficulty = (Difficulty)diffInde;
        switch (difficulty)
        {
            case Difficulty.easy:
                diff = "简单难度";
                return diff;
            case Difficulty.medium:
                diff = "中等难度";
                return diff;
            case Difficulty.hard:
                diff = "困难难度";
                return diff;
            default:
                diff = "无效难度";
                return diff;
        }
    }

}
