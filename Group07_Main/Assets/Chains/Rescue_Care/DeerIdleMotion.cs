using DG.Tweening;
using UnityEngine;

namespace Group07.Chains
{
    /// <summary>
    /// 小鹿待机微动作：轻微呼吸起伏 + 头部缓慢摆动，缓解站立僵硬的观感。
    /// 命中/完成时由链路播放的受击动作优先，待机在补间结束后自动恢复。
    /// </summary>
    public class DeerIdleMotion : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private float breathAmplitude = 0.010f;
        [SerializeField] private float breathSpeed = 1.0f;
        [SerializeField] private float headPitchAngle = 2.5f;
        [SerializeField] private float headYawAngle = 3.5f;
        [SerializeField] private float headSwaySpeed = 0.45f;

        private Vector3 basePosition;
        private Quaternion baseHeadRotation;

        private void Awake()
        {
            basePosition = transform.localPosition;

            if (head != null)
            {
                baseHeadRotation = head.localRotation;
            }
        }

        private void Update()
        {
            float t = Time.time;
            transform.localPosition = basePosition + Vector3.up * (Mathf.Sin(t * breathSpeed) * breathAmplitude);

            if (head == null || DOTween.IsTweening(head))
            {
                return;
            }

            head.localRotation = baseHeadRotation * Quaternion.Euler(
                Mathf.Sin(t * headSwaySpeed) * headPitchAngle,
                Mathf.Sin(t * headSwaySpeed * 0.63f) * headYawAngle,
                0f);
        }
    }
}
