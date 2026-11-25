using UnityEngine;

namespace Group07.ActionCore
{
    /// <summary>
    /// 前屈动作组绑定器：把 QianQuRig prefab 与主题场景里的 XR 手部、肩部、脊柱重新连接。
    /// prefab 内部的 StarParent / Cube_star / 示范模型保持内部引用，不需要重新指定。
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class QianQuRigBinder : MonoBehaviour
    {
        public Transform leftHand;
        public Transform rightHand;
        public Transform leftShoulder;
        public Transform rightShoulder;
        public Transform spine1;

        private void Awake()
        {
            Bind();
        }

        [ContextMenu("重新绑定前屈动作组")]
        public void Bind()
        {
            MainControl main = MainControl._instance != null
                ? MainControl._instance
                : FindObjectOfType<MainControl>(true);

            if (main == null)
            {
                Debug.LogError("[QianQuRigBinder] 场景中没有 MainControl。");
                return;
            }

            if (leftHand == null) leftHand = FindByName("Left Controller");
            if (rightHand == null) rightHand = FindByName("Right Controller");
            if (leftShoulder == null) leftShoulder = FindByName("LEFT_SHOULDER");
            if (rightShoulder == null) rightShoulder = FindByName("RIGHT_SHOULDER");
            if (spine1 == null) spine1 = FindByName("SPINE1");

            Transform starParent = FindDeep(transform, "QianQu_StarParent");
            Transform template = FindDeep(transform, "QianQu_Cube_star");

            foreach (StarCreater creator in GetComponentsInChildren<StarCreater>(true))
            {
                creator.actionAnim = ActionAnim.QianQu;
                creator.tf_hand = creator.isL ? leftHand : rightHand;
                creator.tf_shoulder = creator.isL ? leftShoulder : rightShoulder;
                creator.tf_center_sp1 = spine1;
                creator.tf_starP = starParent;
                creator.go_cubeStar = template != null ? template.gameObject : null;

                if (creator.isL)
                {
                    main.starCreater_L = creator;
                }
                else
                {
                    main.starCreater_R = creator;
                }
            }

            Transform man = FindDeep(transform, "ModelShow01_QianQu");
            if (man != null)
            {
                main.tf_man = man;
                IKControl ik = man.GetComponentInChildren<IKControl>(true);
                if (ik != null)
                {
                    main.ik_jx = ik;
                }
            }
        }

        private static Transform FindByName(string name)
        {
            foreach (Transform t in FindObjectsOfType<Transform>(true))
            {
                if (t.name == name)
                {
                    return t;
                }
            }
            return null;
        }

        private static Transform FindDeep(Transform root, string name)
        {
            if (root.name == name)
            {
                return root;
            }
            foreach (Transform child in root)
            {
                Transform found = FindDeep(child, name);
                if (found != null)
                {
                    return found;
                }
            }
            return null;
        }
    }
}
