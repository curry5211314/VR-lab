using UnityEngine;

public class AngleConverter : MonoBehaviour
{
    // 将原始角度转换为0-180度递增的角度
    public float ConvertAngle(float originalAngle)
    {
        // 确保角度在0-360度范围内
        float normalizedAngle = originalAngle % 360;
        if (normalizedAngle < 0)
            normalizedAngle += 360;

        // 将360度角度转换为0-180度递增角度
        if (normalizedAngle > 180)
        {
            // 对于大于180度的角度，转换为对应的负角度再处理
            normalizedAngle = 360 - normalizedAngle;
            normalizedAngle = 180 - normalizedAngle;
        }

        return normalizedAngle;
    }

    // 测试转换效果
    void Start()
    {
        float[] originalAngles = { 60, 30, 0, 330, 300, 270 };

        Debug.Log("原始角度 -> 转换后角度");
        foreach (float angle in originalAngles)
        {
            float converted = ConvertAngle(angle);
            Debug.Log($"{angle} -> {converted}");
        }
    }
}
