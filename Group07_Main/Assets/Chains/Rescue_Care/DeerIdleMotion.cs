using UnityEngine;

namespace Group07.Chains
{
    /// <summary>
    /// 小鹿躺卧待机微动作：缓慢呼吸起伏 + 极轻的躯体翻转。
    /// 小鹿已换成带骨骼模型并按侧卧收腿静态摆放（未播放动画），
    /// 因此仍用整体位移与绕躯体长轴的微转表现呼吸；幅度刻意做小，避免贴花与垫料穿帮。
    /// 挂在 Deer 根节点上，体表贴花均为其子物体，会随呼吸一起轻微起伏。
    /// </summary>
    public class DeerIdleMotion : MonoBehaviour
    {
        [Header("呼吸（世界向上位移，米）")]
        [SerializeField] private float breathAmplitude = 0.006f;
        [SerializeField] private float breathSpeed = 1.9f;

        [Header("躯体微翻（绕躯体长轴，度）")]
        [SerializeField] private float rollAngle = 0.7f;
        [SerializeField] private float rollSpeed = 0.32f;

        private Vector3 basePosition;
        private Quaternion baseRotation;
        private Vector3 upInParent;
        private Vector3 rollAxisInParent;

        private void Awake()
        {
            basePosition = transform.localPosition;
            baseRotation = transform.localRotation;

            Transform parent = transform.parent;
            upInParent = parent != null
                ? parent.InverseTransformDirection(Vector3.up).normalized
                : Vector3.up;

            // 躺卧后鹿身长轴沿世界 Z 方向，绕它旋转即为「胸腔起伏」式的轻微翻转。
            rollAxisInParent = parent != null
                ? parent.InverseTransformDirection(Vector3.forward).normalized
                : Vector3.forward;
        }

        private void OnDisable()
        {
            transform.localPosition = basePosition;
            transform.localRotation = baseRotation;
        }

        private void Update()
        {
            float t = Time.time;

            transform.localPosition = basePosition + upInParent * (Mathf.Sin(t * breathSpeed) * breathAmplitude);
            transform.localRotation = Quaternion.AngleAxis(Mathf.Sin(t * rollSpeed) * rollAngle, rollAxisInParent) * baseRotation;
        }
    }
}
