using System.Collections;
using UnityEngine;
using DG.Tweening;

public class StarCreater : MonoBehaviour
{
    public ActionAnim actionAnim = ActionAnim.TiCeNXuan_L;
    public bool isL;
    public GameObject go_cubeStar;
    public Transform tf_starP;//这个跟随人体最好。或者人物走出边界，或者程序控制，星星消失

    public Transform tf_center_sp1;
    public Transform tf_shoulder; //轴心
    public Transform tf_creatRot;   //只负责旋转
    public Transform tf_creat;

    public Transform tf_hand;

    int curIndex;
    int endIndex;

    Quaternion initCreatRot = Quaternion.Euler(90f, 0f, 0f);

    private void Awake()
    {
        if (tf_creatRot != null)
        {
            initCreatRot = tf_creatRot.localRotation;
        }
    }

    public void OnSetPos(ActionAnim act, int end)
    {
        if (act != actionAnim) { Debug.Log("不对"); return; }
        endIndex = end;
        curIndex = 0;
        switch (act) 
        { 
            case ActionAnim.TiCeNXuan_L:
                transform.position = tf_shoulder.position;
                if (isL)
                {
                    StartCoroutine(TiCeNeiXuan());
                }
                break;
            case ActionAnim.TiCeNXuan_R:
                transform.position = tf_shoulder.position;
                if (!isL)
                {
                    StartCoroutine(TiCeNeiXuan());
                }
                break;
            case ActionAnim.QianQu:
                transform.position = tf_shoulder.position;
                if (tf_creatRot != null)
                {
                    tf_creatRot.localRotation = initCreatRot;
                }
                CreatStarForQianQu();
                break;

        }   

    }

    float delay=0.1f;

  

    IEnumerator TiCeNeiXuan()
    {

        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(delay);

            tf_creatRot.Rotate(Vector3.left, 30);

            GameObject go = Instantiate(go_cubeStar, tf_starP);
            Star st = go.GetComponent<Star>();
            st.isL = isL;

            bool isEnd = curIndex == endIndex ? true : false;
            st.SetData(curIndex, transform.GetChild(0).localEulerAngles, isEnd);
            curIndex++;
            go.gameObject.SetActive(true);
            go.transform.position = tf_creat.position;

            if (isL)
            {
                MainControl._instance.AddStarL(st);
            }
            else
            {
                MainControl._instance.AddStarR(st);
            }
        }

        int num = endIndex - 3;      //5,6

        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSeconds(delay);

            if (!isL)
            {
                tf_creatRot.Rotate(Vector3.up, -20);
            }
            else
            {
                tf_creatRot.Rotate(Vector3.down, -20);
            }

            GameObject go = Instantiate(go_cubeStar, tf_starP);
            Star st = go.GetComponent<Star>();
            st.isL = isL;

            bool isEnd = curIndex == endIndex ? true : false;
            st.SetData(curIndex, transform.GetChild(0).localEulerAngles, isEnd);
            curIndex++;
            go.gameObject.SetActive(true);
            go.transform.position = tf_creat.position;

            if (isL)
            {
                MainControl._instance.AddStarL(st);
            }
            else
            {
                MainControl._instance.AddStarR(st);
            }
        }

        Debug.Log("<color=green>创建结束！</color>");

        MainControl._instance.OnRunHand();

        tf_starP.parent = tf_center_sp1;
    }

    // 前屈：绕肩部轴心每次向前旋转 30°，在指定位置生成一串目标
    public void CreatStarForQianQu()
    {
        if (curIndex > endIndex)
        {
            Debug.Log("<color=green>创建结束！</color>");

            MainControl._instance.OnRunHand();

            tf_starP.parent = tf_center_sp1;
            return;
        }

        Vector3 rotAng = Vector3.right * -30;

        GameObject go = Instantiate(go_cubeStar, tf_starP);
        go.gameObject.SetActive(true);
        Star st = go.GetComponent<Star>();
        st.isL = isL;

        bool isEnd = curIndex == endIndex ? true : false;
        st.SetData(curIndex, new Vector3(curIndex * 30, 0, 0), isEnd);
        curIndex++;
        go.transform.position = tf_creat.position;

        if (isL)
        {
            MainControl._instance.AddStarL(st);
        }
        else
        {
            MainControl._instance.AddStarR(st);
        }

        tf_creatRot.transform.DORotate(rotAng, delay, RotateMode.LocalAxisAdd).
            SetEase(Ease.OutQuad).
            OnComplete(CreatStarForQianQu);
    }

    public float GetLength()
    {
        float num = Vector3.Distance(tf_hand.position,tf_shoulder.position);
        return num;
    }

}
