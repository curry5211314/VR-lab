using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class BodyTracker311 : MonoBehaviour
{
    public Transform bodytrackers;
    private int bodytrackingSum = 24;

    BodyTrackingData bd= new BodyTrackingData();
    BodyTrackingGetDataInfo bdi = new BodyTrackingGetDataInfo();
    void Start()
    {
        for (int i = 0; i < bodytrackingSum; i++)
        {
            GameObject ga = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ga.transform.parent = bodytrackers;
            ga.transform.localScale = Vector3.one * 0.01f;
        }
    }

    void Update()
    {
        // 获取当前的追踪模式
        MotionTrackerMode trackingMode = PXR_MotionTracking.GetMotionTrackerMode();

        // 更新人体追踪位姿数据
        if (trackingMode == MotionTrackerMode.BodyTracking)
        {
            // 获取各个身体节点的位置和方向数据
            int ret = PXR_MotionTracking.GetBodyTrackingData(ref bdi, ref bd);
            MainControl._instance.tt_log_avata.text = "状态:" + ret.ToString() + " Length:" + bd.roleDatas.Length;

            for (int i = 0; i < bodytrackingSum; i++)
            {
                bodytrackers.GetChild(i).position = new Vector3((float)bd.roleDatas[i].localPose.PosX, (float)bd.roleDatas[i].localPose.PosY, (float)bd.roleDatas[i].localPose.PosZ);
                bodytrackers.GetChild(i).rotation = new Quaternion((float)bd.roleDatas[i].localPose.RotQx, (float)bd.roleDatas[i].localPose.RotQy, (float)bd.roleDatas[i].localPose.RotQz, (float)bd.roleDatas[i].localPose.RotQw);
                bodytrackers.GetChild(i).localScale = Vector3.one * 0.1f;
            }
        }
    }
}
