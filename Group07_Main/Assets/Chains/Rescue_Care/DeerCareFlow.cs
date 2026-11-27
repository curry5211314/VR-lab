using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Group07.ActionCore;
using Group07.Story;
using Group07.Story.UI;
using UnityEngine;

namespace Group07.Chains
{
    /// <summary>
    /// 野生动物救助站 - 内旋护理（体侧内旋链路，左右双侧）。
    /// 命中推进 S0→S1→S2，完成进入 S3；失误只提示，不扣进度。
    /// </summary>
    public class DeerCareFlow : StoryFlowBase, IStoryGuideProvider
    {
        [SerializeField] private TiCeNActionAdapter actionAdapter;

        [Header("垫料（顺序：L0-L4, R0-R4）")]
        [SerializeField] private Renderer[] beddingRenderers = new Renderer[10];
        [SerializeField] private Material dirtyMaterial;
        [SerializeField] private Material cleanMaterial;

        [Header("反馈（可选）")]
        [SerializeField] private AudioSource feedbackAudio;
        [SerializeField] private AudioClip hitClip;
        [SerializeField] private AudioClip missClip;
        [SerializeField] private GameObject missHint;

        [Header("受击与阶段表现")]
        [SerializeField] private Transform deerRoot;
        [SerializeField] private Transform deerHead;
        [SerializeField] private GameObject hitEffect;
        [SerializeField] private float hitEffectScale = 0.6f;
        [SerializeField] private Light stageLight;
        [SerializeField] private Color coldLightColor = new Color(0.55f, 0.70f, 1f);
        [SerializeField] private Color warmLightColor = new Color(1f, 0.83f, 0.60f);
        [SerializeField] private float coldIntensity = 0.6f;
        [SerializeField] private float warmIntensity = 1.4f;

        [Header("引导示范（可选）")]
        [SerializeField] private GameObject guideDemo;
        [SerializeField] private Transform guideForearmLeft;
        [SerializeField] private Transform guideForearmRight;
        [SerializeField] private float guideSweepOut = -20f;
        [SerializeField] private float guideSweepIn = 35f;
        [SerializeField] private float guideSweepDuration = 1.2f;

        private StoryStateController state;
        private int hitCount;
        private Coroutine missRoutine;

        public override string ChainId => "Rescue_Care";
        public override string DisplayName => "内旋护理";
        public override int TargetCount => 10; // 左右各 5 个内旋目标

        public List<StoryGuidePanel.GuideStep> BuildGuideSteps()
        {
            return new List<StoryGuidePanel.GuideStep>
            {
                new StoryGuidePanel.GuideStep
                {
                    title = "情境：脏污的护理区",
                    body = "受伤的小鹿蜷缩在脏污的垫料上。请清理它左右两侧的护理区，让它慢慢恢复。"
                },
                new StoryGuidePanel.GuideStep
                {
                    title = "动作：体侧内旋",
                    body = "双臂贴身、屈肘约 90°，前臂向内旋转，依次触碰身体两侧亮起的护理目标。可先跟着场内的蓝色示范手臂做一次。"
                },
                new StoryGuidePanel.GuideStep
                {
                    title = "失误：只提示不扣进度",
                    body = "保持上臂贴身；超时没碰到目标只会提示，不扣进度。左右手交替完成全部 10 个目标。"
                }
            };
        }

        private void Awake()
        {
            ShowGuideDemo();
        }

        public override void BindContext(StoryContext context)
        {
            state = context != null ? context.stateController : null;

            if (state == null && context != null)
            {
                state = context.GetComponent<StoryStateController>();
            }
        }

        public override void OnRoundStart(StorySide side, int difficulty)
        {
            hitCount = 0;
            HideGuideDemo();
            HideMissHint();
            ResetBedding();
            ResetVisuals();

            if (state != null)
            {
                state.ResetToStart();
            }

            if (actionAdapter == null)
            {
                actionAdapter = GetComponent<TiCeNActionAdapter>();
            }

            if (actionAdapter != null)
            {
                actionAdapter.BeginAction(difficulty);
            }
        }

        public override void OnTargetHit(StoryHitInfo hit)
        {
            hitCount++;

            int newStage = -1;
            if (hitCount == 3)
            {
                newStage = 1;
            }
            else if (hitCount == 6)
            {
                newStage = 2;
            }

            if (newStage >= 0)
            {
                if (state != null)
                {
                    state.SetStage(newStage);
                }

                ApplyStageVisual(newStage, false);
            }

            CleanBedding(hit.side, hit.index);
            PlayHitReaction(hit);
        }

        public override void OnTargetMiss(int index)
        {
            PlayClip(missClip);
            ShowMissHint();
            Debug.Log("[DeerCareFlow] 失误提示：保持上臂贴身，前臂向内旋转。");
        }

        public override void OnRoundComplete(StoryRoundResult result)
        {
            HideMissHint();

            if (state != null)
            {
                state.SetStage(3);
            }

            ApplyStageVisual(3, false);

            if (deerHead != null)
            {
                deerHead.DOKill();
                deerHead.DOLocalRotate(new Vector3(-10f, 0f, 0f), 0.6f).SetEase(Ease.OutBack);
            }
        }

        public override void OnRoundAbort()
        {
            HideMissHint();
            ResetBedding();
            ResetVisuals();

            if (state != null)
            {
                state.ResetToStart();
            }
        }

        private void PlayHitReaction(StoryHitInfo hit)
        {
            SpawnHitEffect(hit);

            if (deerRoot != null)
            {
                deerRoot.DOKill();
                deerRoot.localScale = Vector3.one;
                deerRoot.DOPunchScale(Vector3.one * 0.06f, 0.25f, 3, 0.5f);
            }

            if (deerHead != null)
            {
                deerHead.DOKill();
                deerHead.DOPunchRotation(new Vector3(-8f, 0f, 0f), 0.3f, 2, 0.5f);
            }

            PlayClip(hitClip);
        }

        private void SpawnHitEffect(StoryHitInfo hit)
        {
            if (hitEffect == null || beddingRenderers == null || beddingRenderers.Length == 0)
            {
                return;
            }

            int offset = hit.side == StorySide.Left ? 0 : 5;
            int i = Mathf.Clamp(offset + hit.index, 0, beddingRenderers.Length - 1);
            Renderer bedding = beddingRenderers[i];
            if (bedding == null)
            {
                return;
            }

            Vector3 pos = bedding.transform.position + Vector3.up * 0.2f;
            GameObject fx = Instantiate(hitEffect, pos, Quaternion.identity);
            fx.transform.localScale = Vector3.one * hitEffectScale;
            Destroy(fx, 2f);
        }

        private void ApplyStageVisual(int stage, bool instant)
        {
            if (stageLight == null)
            {
                return;
            }

            float t = Mathf.Clamp01(stage / 3f);
            Color color = Color.Lerp(coldLightColor, warmLightColor, t);
            float intensity = Mathf.Lerp(coldIntensity, warmIntensity, t);

            stageLight.DOKill();
            if (instant)
            {
                stageLight.color = color;
                stageLight.intensity = intensity;
            }
            else
            {
                stageLight.DOColor(color, 0.4f);
                stageLight.DOIntensity(intensity, 0.4f);
            }
        }

        private void ResetVisuals()
        {
            if (deerRoot != null)
            {
                deerRoot.DOKill();
                deerRoot.localScale = Vector3.one;
            }

            if (deerHead != null)
            {
                deerHead.DOKill();
                deerHead.localRotation = Quaternion.identity;
            }

            ApplyStageVisual(0, true);
        }

        private void CleanBedding(StorySide side, int index)
        {
            if (beddingRenderers == null || beddingRenderers.Length == 0 || cleanMaterial == null)
            {
                return;
            }

            int offset = side == StorySide.Left ? 0 : 5;
            int i = Mathf.Clamp(offset + index, 0, beddingRenderers.Length - 1);
            if (beddingRenderers[i] != null)
            {
                beddingRenderers[i].sharedMaterial = cleanMaterial;
            }
        }

        private void ResetBedding()
        {
            if (beddingRenderers == null || dirtyMaterial == null)
            {
                return;
            }

            for (int i = 0; i < beddingRenderers.Length; i++)
            {
                if (beddingRenderers[i] != null)
                {
                    beddingRenderers[i].sharedMaterial = dirtyMaterial;
                }
            }
        }

        private void PlayClip(AudioClip clip)
        {
            if (feedbackAudio != null && clip != null)
            {
                feedbackAudio.PlayOneShot(clip);
            }
        }

        private void ShowMissHint()
        {
            if (missHint == null)
            {
                return;
            }

            if (missRoutine != null)
            {
                StopCoroutine(missRoutine);
            }

            missRoutine = StartCoroutine(ShowMissHintRoutine());
        }

        private IEnumerator ShowMissHintRoutine()
        {
            missHint.SetActive(true);
            yield return new WaitForSeconds(2f);
            missHint.SetActive(false);
            missRoutine = null;
        }

        private void HideMissHint()
        {
            if (missRoutine != null)
            {
                StopCoroutine(missRoutine);
                missRoutine = null;
            }

            if (missHint != null)
            {
                missHint.SetActive(false);
            }
        }

        /// <summary>引导阶段的简化示范：两只手臂前臂来回内旋，开始后隐藏。</summary>
        private void ShowGuideDemo()
        {
            if (guideDemo != null)
            {
                guideDemo.SetActive(true);
            }

            StartGuideSweep(guideForearmLeft, 1f);
            StartGuideSweep(guideForearmRight, -1f);
        }

        private void StartGuideSweep(Transform pivot, float direction)
        {
            if (pivot == null)
            {
                return;
            }

            pivot.DOKill();
            pivot.localRotation = Quaternion.Euler(0f, guideSweepOut * direction, 0f);
            pivot.DOLocalRotate(new Vector3(0f, guideSweepIn * direction, 0f), guideSweepDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }

        private void HideGuideDemo()
        {
            if (guideDemo != null)
            {
                guideDemo.SetActive(false);
            }

            if (guideForearmLeft != null)
            {
                guideForearmLeft.DOKill();
            }

            if (guideForearmRight != null)
            {
                guideForearmRight.DOKill();
            }
        }
    }
}
