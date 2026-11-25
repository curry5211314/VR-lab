using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class IKControl : MonoBehaviour
{
    protected Animator animator;

    public bool ikActive = false;
    public Transform HeadObj;
    public Transform rightHandObj = null;
    public Transform LeftHandObj = null;
    public Transform rightFootObj = null;
    public Transform leftFootObj = null;

    public Transform lookObj = null;

    public Transform tf_rot;

    void Start()
    {
        animator = GetComponent<Animator>();

    }

    int animLoops = -1;
    public void OnBiRotUp()
    {

        tf_rot.DOLocalRotate(Vector3.left * 0, 3).SetEase(Ease.Linear).SetLoops(animLoops, LoopType.Yoyo);
    }

    void OnAnimatorIK()
    {
        if (animator)
        {

            if (ikActive)
            {

                if (lookObj != null)
                {

                }
                if (HeadObj != null)
                {

                }

                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);    //改变手的方向
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

                if (LeftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandObj.rotation);
                }

                if (rightFootObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootObj.rotation);
                }

                if (leftFootObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootObj.rotation);
                }
            }

            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                animator.SetLookAtWeight(0);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
            }
        }
    }
}

