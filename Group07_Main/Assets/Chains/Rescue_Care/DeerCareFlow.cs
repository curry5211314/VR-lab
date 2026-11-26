using System.Collections;
using Group07.ActionCore;
using Group07.Story;
using UnityEngine;

namespace Group07.Chains
{
    /// <summary>
    /// 野生动物救助站 - 内旋护理（体侧内旋链路，左右双侧）。
    /// 命中推进 S0→S1→S2，完成进入 S3；失误只提示，不扣进度。
    /// </summary>
    public class DeerCareFlow : StoryFlowBase
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

        private StoryStateController state;
        private int hitCount;
        private Coroutine missRoutine;

        public override string ChainId => "Rescue_Care";
        public override string DisplayName => "内旋护理";
        public override int TargetCount => 10; // 左右各 5 个内旋目标

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
            HideMissHint();
            ResetBedding();

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

            if (state != null)
            {
                if (hitCount == 3)
                {
                    state.SetStage(1);
                }
                else if (hitCount == 6)
                {
                    state.SetStage(2);
                }
            }

            CleanBedding(hit.side, hit.index);
            PlayClip(hitClip);
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
        }

        public override void OnRoundAbort()
        {
            HideMissHint();
            ResetBedding();

            if (state != null)
            {
                state.ResetToStart();
            }
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
    }
}
