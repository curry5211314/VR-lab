using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Group07.Story;

//获取手臂到手的距离。然后画弧。 生成一个长方块，一头旋转。 按角度生成星星。或者 提前出点，然后精灵飞出。
//测试 语音。关键词。ai or 119

public class Star : MonoBehaviour
{
    public int index;
    public bool isEnd;
    public Vector3 ang;
    public int score;//分数越高，难度越大。相当于多少个星星。 planA：10小星星会创建为大星星， 大星星再进行积累。
    public int littlestarNum;
    public ParticleSystem ef_get;
    public AudioSource aud_get;
    public BoxCollider collid; //依次打开。防止误操作
    public GameObject go_star;
    public AudioSource aud_show;

    public bool isL;

    /// <summary>故事链路开启后，当前目标超时未命中会上报失误。</summary>
    public static bool EnableMissDetection;
    /// <summary>单个目标允许的反应时间（秒），由动作适配器按难度设置。</summary>
    public static float MissTimeoutSeconds = 6f;

    private Coroutine missRoutine;
    private bool missReported;

    public void OnEnableBox(bool isT)
    {
        collid.enabled = isT;

        if (missRoutine != null)
        {
            StopCoroutine(missRoutine);
            missRoutine = null;
        }

        if (isT && EnableMissDetection && !missReported)
        {
            missRoutine = StartCoroutine(OnMissTimeout());
        }
    }

    // 当前目标超时未命中：上报失误，并推进到下一颗；最后一颗交给轮次结算。
    private IEnumerator OnMissTimeout()
    {
        yield return new WaitForSeconds(MissTimeoutSeconds);
        missRoutine = null;

        if (!collid.enabled)
        {
            yield break;
        }

        missReported = true;
        StoryActionBus.RaiseTargetMiss(index);
        transform.DOScale(Vector3.zero, 0.2f);

        if (!isEnd && MainControl._instance != null)
        {
            if (isL)
            {
                MainControl._instance.OnEnableNextBoxL(index);
            }
            else
            {
                MainControl._instance.OnEnableNextBoxR(index);
            }
        }
    }

    private void CancelMissWatch()
    {
        missReported = true;
        if (missRoutine != null)
        {
            StopCoroutine(missRoutine);
            missRoutine = null;
        }
    }

    public void SetData(int ind, Vector3 angle,bool isEd)
    {
        transform.localScale = Vector3.zero;

        index = ind;
        score = (ind) * 1;
        littlestarNum = score;
        ang = angle;
        if (index == 0)
        {
            go_star.SetActive(false);
        }
        else
        {
            //aud_show.Play();
        }
        if (index == 1)
        { 
            go_star.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        isEnd = isEd;
        if (isEnd)
        {
            go_star.SetActive(false);
            transform.GetChild(2).gameObject.SetActive(true);
        }
    }


    void Start()
    {
        
    }

    public void OnShowStar()
    {
        //StartCoroutine(showStar());
        aud_show.Play();
        transform.DOScale(Vector3.one * 0.1f, 0.5f).SetEase(Ease.OutExpo);
    }
    IEnumerator showStar()
    { 
        yield return new WaitForSeconds(index*0.3f);
        transform.DOScale(Vector3.one*0.1f,0.5f).SetEase(Ease.OutExpo);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (index == 0)
        {
            if (isL)
            {
                if (other.name == "L_Hand_CD")
                {
                    OnPlayZero();
                }
            }
            else
            {
                if (other.name == "R_Hand_CD")
                {
                    OnPlayZero();
                }
            }
        }
        else 
        {
            if (isL)
            {
                if (other.name == "L_Hand_CD")
                {
                    OnTiggerStar();
                }
            }
            else
            {
                if (other.name == "R_Hand_CD")
                {
                    OnTiggerStar();
                }
            }
        }

        /*
        // 获取碰撞器的中心点（世界坐标）
        Vector3 myCenter = GetComponent<Collider>().bounds.center;

        // 获取碰撞点（Trigger 也能拿到最接近的点）
        Vector3 closestPoint = other.ClosestPoint(myCenter);

        // 计算方向（从中心到碰撞点）
        Vector3 direction = (closestPoint - myCenter).normalized;

        // 判断大致方向
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.z))
        {
            Debug.Log("碰撞来自 " + (direction.x > 0 ? "右边" : "左边"));
            MainControl._instance.AddLog("碰撞来自 " + (direction.x > 0 ? "右边" : "左边"));
        }
        else
        {
            Debug.Log("碰撞来自 " + (direction.z > 0 ? "前方" : "后方"));
            MainControl._instance.AddLog("碰撞来自 " + (direction.z > 0 ? "前方" : "后方"));
        }

        if (index != 0)
        {
            if (isL)
            {
                if (other.name == "L_Hand_CD")
                {
                    OnInitiateStar();
                }
            }
            else
            {
                if (other.name == "R_Hand_CD")
                {
                    OnInitiateStar();
                }
            }
        }
        else
        {
            if (direction.z <= 0)  //如果是后方
            {
                if (isL)
                {
                    if (other.name == "L_Hand_CD")
                    {
                        OnInitiateStar();
                    }
                }
                else
                {
                    if (other.name == "R_Hand_CD")
                    {
                        OnInitiateStar();
                    }
                }
            }
        }
        */

    }


    //触发
    public void OnTiggerStar()
    {
        CancelMissWatch();
        StoryActionBus.RaiseTargetHit(index, isL);
        aud_get.Play();
        if (ef_get)
        {
            ef_get.Play();
        }
        else
        { 
            ef_get.gameObject.SetActive(true);
            ef_get.Play();
        }
        this.transform.DOScale(Vector3.one * 0, 0.3f);

        //创建特效小星星
        littlestarNum = score / 1;
        StartCoroutine(FlyStar(littlestarNum));
        AdvanceToNextTarget();
    }

    //触发 第一次触发
    public void OnPlayZero()
    {
        CancelMissWatch();
        StoryActionBus.RaiseTargetHit(index, isL);
        this.transform.DOScale(Vector3.one * 0, 0.3f);
        AdvanceToNextTarget();
    }

    // 切到下一颗目标；故事模式下最后一颗不再循环回第一颗，避免结算后整串重复出现。
    private void AdvanceToNextTarget()
    {
        if (MainControl._instance == null)
        {
            return;
        }

        if (isEnd && EnableMissDetection)
        {
            return;
        }

        if (isL)
        {
            MainControl._instance.OnEnableNextBoxL(index);
        }
        else
        {
            MainControl._instance.OnEnableNextBoxR(index);
        }
    }


    //创建小金币到进度条
    IEnumerator FlyStar(int num)
    {
        for (int i = 0; i < num; i++)
        {
            yield return new WaitForSeconds(0.8f/num);
            if (isL)
            {
                MainControl._instance.OnAddScoreL(ang, 1);
            }
            else
            { 
                MainControl._instance.OnAddScoreR(ang, 1);
            }

            GameObject go =  Instantiate(go_star,MainControl._instance.transform);
            go.transform.localScale = Vector3.one * 0.05f;
            go.transform.position = transform.position;

            //go.transform.DOMove(MainControl._instance.tf_ScoreStars.position, 1).SetEase(Ease.OutExpo).
            //    OnComplete(()=> { go.GetComponent<AudioSource>().Play(); Destroy(go,0.5f); });

            Transform flyTarget = MainControl._instance.sld != null ? MainControl._instance.sld.handleRect : null;
            if (flyTarget == null)
            {
                Destroy(go, 0.2f);
                continue;
            }

            go.transform.DOMove(flyTarget.position, 0.8f).SetEase(Ease.OutExpo).
               OnComplete(() => 
               {
                   //go.GetComponent<AudioSource>().Play(); //收集星星 音效
                   Destroy(go, 0.5f); 
               });
        }
    }



}
