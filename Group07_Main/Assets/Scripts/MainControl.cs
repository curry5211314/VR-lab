using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.XR;

public class MainControl : MonoBehaviour
{
    public static MainControl _instance;

    public bool isGaming;
    public int curStarIndexL;
    public int curStarIndexR;
    public List<Star> StarLs = new List<Star>();
    public List<Star> StarRs = new List<Star>();

    public Text tt_log;
    public Text tt_log_avata;

    public Button btn_StarPos;
    public Button btn_Menu;
    int creat_endIndex_01 = 7;

    public StarCreater starCreater_L_tcNx;
    public StarCreater starCreater_R_tcNx;

    [Header("前屈动作引用（可选）")]
    public StarCreater starCreater_L;
    public StarCreater starCreater_R;
    public Transform tf_man;
    public IKControl ik_jx;

    public RectTransform tf_group;
    public Button btn_last;
    public Button btn_next;

    public int score;
    public Vector3 angL;
    public Vector3 angR;

    public Text tt_score;
    public Text tt_AngL;
    public Text tt_AngR;

    public GameObject[] gos_uistar;
    int curUIstar_index = -1;

    public Slider sld;
    public GameObject particleImg;

    public Transform tf_meun;
    public Transform tf_startPanel;

    public Transform tf_man_tcNx_L;
    public Transform tf_man_tcNx_R;

    public Text tt_info;
    private void Awake()
    {
        _instance = this;
        isStar5 = false;
    }

    AudioSource aud_score;
    void Start()
    {
        // 旧 UI 全部按可选处理：删除 Controller Canvas UI 后这里不会报空引用
        if (tog != null) tog.onValueChanged.AddListener(OnSetTap);
        audMain = GetComponent<AudioSource>();

        if (btn_Menu != null) btn_Menu.onClick.AddListener(OnUIMEUN);
        if (btn_last != null) btn_last.onClick.AddListener(OnUILastPanel);
        if (btn_next != null) btn_next.onClick.AddListener(OnUINextPanel);
        if (tf_group != null)
        {
            GridLayoutGroup menuLayout = tf_group.GetComponent<GridLayoutGroup>();
            if (menuLayout != null)
            {
                gap = menuLayout.cellSize.x + menuLayout.spacing.x;
            }
            menuStartX = tf_group.anchoredPosition.x;
        }

        if (btn_StarPos != null) btn_StarPos.onClick.AddListener(OnPosCreater);
        if (btn_Picomove != null) btn_Picomove.onClick.AddListener(OnMoveAvata);

        if (sld_shoulderLeng != null)
        {
            sld_shoulderLeng.onValueChanged.AddListener(OnSetShoulderLength);
            float num = PlayerPrefs.GetFloat("shoulderLeng", sld_shoulderLeng.value);
            sld_shoulderLeng.SetValueWithoutNotify(num);
            OnSetShoulderLength(sld_shoulderLeng.value);
        }

        UpdateMenuButtons();
    }

    public void GetStarAtIndex()
    {
        PlayerPrefs.GetFloat("action_"+ curIndexAction);

    }

    AudioSource audMain;

    public int triggerIndex_L;
    public int triggerIndex_R;
    public string actName;
    public void OnSaveCsvData_L()
    {
        if (!isGaming) return;
        actName = GetActionName();

        DoubleSpace2Csv._instance.RecFIT("L");
        if (audMain != null) audMain.Play();
    }

    public void OnSaveCsvData_R()
    {
        if (!isGaming) return;
        actName = GetActionName();

        DoubleSpace2Csv._instance.RecFIT("R");
        if (audMain != null) audMain.Play();
    }

    string GetActionName()
    {
        switch (action)
        {

            case ActionAnim.QianQu:
                return "前屈";
            case ActionAnim.TiCeNXuan_L:
                return "体侧内旋 左";
            case ActionAnim.TiCeNXuan_R:
                return "体侧内旋 右";
            case ActionAnim.TiCeNXuan_Both:
                return "体侧内旋 双侧";

        }
        return "null";
    }

    public AudioSource actAd;
    string actContent = string.Empty;
    public Text tt_title;
    IEnumerator OnPlayActionContent()
    { 
        yield return new WaitForSeconds(1);
        if (actContent.Length > 0)
        {
            if (actAd != null) actAd.Play();
            if (tt_info != null) tt_info.text = actContent;
        }
    }

    int curIndexAction;

    ActionAnim action = ActionAnim.TiCeNXuan_L;
    public int difficulty;
    public void OnSceneInit(ActionAnim act,int diff, int endindex,AudioClip clp,string data,int inx)
    {
        curIndexAction = inx;
        isGaming = true;
        action = act;
        difficulty = diff;
        creat_endIndex_01= endindex;
        if (tt_title != null) tt_title.text = GetActionName();
        if (clp && !string.IsNullOrEmpty(data))
        {
            if (actAd != null) actAd.clip = clp;
            actContent = data;
        }

        switch (action)
        {

            case ActionAnim.TiCeNXuan_L:
            case ActionAnim.TiCeNXuan_R:
            case ActionAnim.TiCeNXuan_Both:
                if (Camera.main != null && Camera.main.GetComponent<Skybox>() != null)
                {
                    Camera.main.GetComponent<Skybox>().enabled = true;
                }
                break;
            case ActionAnim.QianQu:
                if (Camera.main != null && Camera.main.GetComponent<Skybox>() != null)
                {
                    Camera.main.GetComponent<Skybox>().enabled = false;
                }
                break;

        }
    }

    public string handIndex;

    public void OnPosCreater()     //0
    {
        if (btn_StarPos != null) btn_StarPos.interactable = false;
        actName = GetActionName();
        switch (action)
        {

            case ActionAnim.TiCeNXuan_L:
                handIndex = "L_Hand";
                if (starCreater_L_tcNx != null) starCreater_L_tcNx.OnSetPos(action, creat_endIndex_01);
                break;
            case ActionAnim.TiCeNXuan_R:
                handIndex = "R_Hand";
                if (starCreater_R_tcNx != null) starCreater_R_tcNx.OnSetPos(action, creat_endIndex_01);
                break;
            case ActionAnim.TiCeNXuan_Both:
                handIndex = "Double";
                if (starCreater_L_tcNx != null) starCreater_L_tcNx.OnSetPos(ActionAnim.TiCeNXuan_L, creat_endIndex_01);
                if (starCreater_R_tcNx != null) starCreater_R_tcNx.OnSetPos(ActionAnim.TiCeNXuan_R, creat_endIndex_01);
                break;
            case ActionAnim.QianQu:
                handIndex = "Double";
                if (starCreater_L != null) starCreater_L.OnSetPos(ActionAnim.QianQu, creat_endIndex_01);
                if (starCreater_R != null) starCreater_R.OnSetPos(ActionAnim.QianQu, creat_endIndex_01);
                break;

        }

        DoubleSpace2Csv._instance.SetStartData(actName,difficulty,shoulderLength,handIndex);
    }

    public Transform tf_picoBody;
    public Button btn_Picomove;
    bool isMove =false;
    public void OnMoveAvata()
    {
        isMove= !isMove;
        if (tf_picoBody != null)
        {
            tf_picoBody.DOMoveZ(isMove ? 1.3f : 0f, 1).SetEase(Ease.OutExpo);
        }
        if (btn_Picomove != null && btn_Picomove.transform.childCount > 0)
        {
            Text label = btn_Picomove.transform.GetChild(0).GetComponent<Text>();
            if (label != null) label.text = isMove ? "参考还原" : "参考前移";
        }

        if (starCreater_R_tcNx != null) AddLog(starCreater_R_tcNx.GetLength().ToString());

        StarCreater lengthCreater = starCreater_R_tcNx != null ? starCreater_R_tcNx : starCreater_R;
        if (lengthCreater != null) AddLog(lengthCreater.GetLength().ToString());
    }

    public int shoulderLength;
    public Slider sld_shoulderLeng;
    public Text tt_sldnum;  

    public void OnSetShoulderLength(float num)
    {
        shoulderLength = Mathf.RoundToInt(num);
        if (tt_sldnum != null) tt_sldnum.text = "设置手臂长度：" + shoulderLength.ToString() + "cm";
        float armLength = num / 100;
        if (starCreater_L_tcNx != null && starCreater_L_tcNx.transform.childCount > 0)
        {
            Transform leftArm = starCreater_L_tcNx.transform.GetChild(0);
            Vector3 leftScale = leftArm.localScale;
            leftScale.z = armLength + 0.02f;
            leftArm.localScale = leftScale;
        }
        if (starCreater_R_tcNx != null && starCreater_R_tcNx.transform.childCount > 0)
        {
            Transform rightArm = starCreater_R_tcNx.transform.GetChild(0);
            Vector3 rightScale = rightArm.localScale;
            rightScale.z = armLength + 0.02f;
            rightArm.localScale = rightScale;
        }
        // 前屈：原工程手臂模型按长度 ×2 缩放
        if (starCreater_L != null && starCreater_L.transform.childCount > 0)
        {
            Transform leftArmQ = starCreater_L.transform.GetChild(0);
            Vector3 leftScaleQ = leftArmQ.localScale;
            leftScaleQ.z = armLength * 2;
            leftArmQ.localScale = leftScaleQ;
        }
        if (starCreater_R != null && starCreater_R.transform.childCount > 0)
        {
            Transform rightArmQ = starCreater_R.transform.GetChild(0);
            Vector3 rightScaleQ = rightArmQ.localScale;
            rightScaleQ.z = armLength * 2;
            rightArmQ.localScale = rightScaleQ;
        }
        PlayerPrefs.SetFloat("shoulderLeng", num);
        PlayerPrefs.Save();
    }

    public void OnStartPanel()
    {
        if (tf_meun != null) tf_meun.DOScale(Vector3.zero, 1f).SetEase(Ease.OutExpo);
        if (tf_startPanel != null) tf_startPanel.DOScale(Vector3.one, 1f).SetEase(Ease.OutExpo);
        if (fairy != null) fairy.gameObject.SetActive(true);

        StartCoroutine(OnDealyFairyPlayAud0());
    }

    IEnumerator OnDealyFairyPlayAud0()
    {
        if (tt_info != null) tt_info.text = "欢迎来到康复训练中心，接下来我们学习一下右侧人体动作！";
        yield return new WaitForSeconds(3);

        if (fairy != null) fairy.OnPlayAudAtIndex(0);
      
        yield return new WaitForSeconds(5);
        StartCoroutine(OnPlayActionContent());

        StartAnimJX();
       
    }

    public void OnUIMEUN()
    {
        isGaming = false;
        StartCoroutine(SaveFileCsv());
    }

    IEnumerator SaveFileCsv()
    { 
        DoubleSpace2Csv._instance.StopRecording();
        yield return new WaitForEndOfFrame();
        SceneManager.LoadScene(0);
    }

    public float gap ;
    float delay_UI = 0.4f;
    float menuStartX;
    int menuPage;

    void UpdateMenuButtons()
    {
        if (btn_last != null) btn_last.interactable = menuPage > 0;
        if (btn_next != null && tf_group != null) btn_next.interactable = menuPage < tf_group.childCount - 1;
    }

    void OnUINextPanel()
    {
        if (tf_group == null) return;
        menuPage = Mathf.Min(menuPage + 1, tf_group.childCount - 1);
        tf_group.DOAnchorPosX(menuStartX - gap * menuPage, delay_UI).SetEase(Ease.OutExpo);
        UpdateMenuButtons();
    }

    void OnUILastPanel()
    {
        if (tf_group == null) return;
        menuPage = Mathf.Max(menuPage - 1, 0);
        tf_group.DOAnchorPosX(menuStartX - gap * menuPage, delay_UI).SetEase(Ease.OutExpo);
        UpdateMenuButtons();
    }

    public Toggle tog;
    int tap=1;

    public void OnSetTap(bool isTap)
    {
        if (isTap)
        {
            tap = 5;
        }
        else
        { 
            tap= 1;
        }
    }
    bool isStar5;

    public void OnAddScoreL(Vector3 ang, int sc)
    {
        print(isStar5);
        if (isStar5) return;
        angL = ang;                                     //L
        if (tt_AngL != null) tt_AngL.text = "" + ang.ToString();   //L

        score += tap;
        if (tt_score != null) tt_score.text = score.ToString();

        if (sld != null && sld.value >= sld.maxValue)
        {
            if (particleImg != null) particleImg.SetActive(false);
            sld.value = 0;

            if (curUIstar_index < 4 && gos_uistar != null && gos_uistar.Length > curUIstar_index + 1)
            {
                curUIstar_index++;
                if (gos_uistar[curUIstar_index] != null)
                {
                    gos_uistar[curUIstar_index].gameObject.SetActive(true);
                    AudioSource starAud = gos_uistar[curUIstar_index].GetComponent<AudioSource>();
                    if (starAud != null) starAud.Play();
                }
                if (curUIstar_index < 4)
                {
                    if (fairy != null) fairy.OnPlayAudAtIndex(2);
                }
                else
                {
                    if (fairy != null) fairy.OnPlayAudAtIndex(3);
                    isStar5 = true;
                    return;
                }
            }
        }
        if (particleImg != null) particleImg.SetActive(true);
        if (sld != null)
        {
            float num = sld.value;
            num = num + tap;
            sld.value = num;
        }
    }

    public void OnAddScoreR(Vector3 ang, int sc)
    {
        print(isStar5);
        if (isStar5) return;
        angR = ang;                                     //R
        if (tt_AngR != null) tt_AngR.text = "" + ang.ToString();   //R

        score += tap;
        if (tt_score != null) tt_score.text = score.ToString();

        if (sld != null && sld.value >= sld.maxValue)
        {
            if (particleImg != null) particleImg.SetActive(false);
            sld.value = 0;

            if (curUIstar_index < 4 && gos_uistar != null && gos_uistar.Length > curUIstar_index + 1)
            {
                curUIstar_index++;
                if (gos_uistar[curUIstar_index] != null)
                {
                    gos_uistar[curUIstar_index].gameObject.SetActive(true);
                    AudioSource starAud = gos_uistar[curUIstar_index].GetComponent<AudioSource>();
                    if (starAud != null) starAud.Play();
                }
                if (curUIstar_index < 4)
                {
                    if (fairy != null) fairy.OnPlayAudAtIndex(2);
                }
                else
                {
                    if (fairy != null) fairy.OnPlayAudAtIndex(3);
                    isStar5 = true;
                    return;
                }
            }
        }
        if (particleImg != null) particleImg.SetActive(true);
        if (sld != null)
        {
            float num = sld.value;
            num = num + tap;
            sld.value = num;
        }
    }

    void OnLookhand()
    { 

    }

    public void AddLog(string ang)
    { 
        if (tt_log != null) tt_log.text = ang.ToString()+"\n";
    }

    public void AddStarL(Star st)
    {
        StarLs.Add(st);
    }

    public void AddStarR(Star st)
    {
        StarRs.Add(st);
    }

    public FairyControl fairy;
    public void OnRunHand()
    {
        // 双侧内旋时左右生成器各调用一次：等两侧目标都生成完，只由后完成的一侧推进一次。
        if (action == ActionAnim.TiCeNXuan_Both)
        {
            int need = creat_endIndex_01 + 1;
            if (StarLs.Count < need || StarRs.Count < need)
            {
                return;
            }
        }

        curStarIndexL = 100;
        curStarIndexR = 100;
        print("OnRunHand");
        if (StarLs.Count > 0) OnEnableNextBoxL(0);
        if (StarRs.Count > 0) OnEnableNextBoxR(0);

    }

    public void StartAnimJX()
    {
        Debug.Log(action);
        switch (action)
        {

            case ActionAnim.TiCeNXuan_L:
                if (tf_man_tcNx_L != null) tf_man_tcNx_L.gameObject.SetActive(true);
                break;
            case ActionAnim.TiCeNXuan_R:
                if (tf_man_tcNx_R != null) tf_man_tcNx_R.gameObject.SetActive(true);
                break;
            case ActionAnim.TiCeNXuan_Both:
                if (tf_man_tcNx_L != null) tf_man_tcNx_L.gameObject.SetActive(true);
                if (tf_man_tcNx_R != null) tf_man_tcNx_R.gameObject.SetActive(true);
                break;
            case ActionAnim.QianQu:
                if (tf_man != null) tf_man.gameObject.SetActive(true);
                if (ik_jx != null) ik_jx.OnBiRotUp();
                break;

        }

    }

    public void OnPlayGetScore()
    {
        if (aud_score != null) aud_score.Play();
    }

    public void OnEnableNextBoxL(int trigInd)
    {
        if (StarLs.Count == 0) return;
        triggerIndex_L = trigInd;
        for (int i = 0; i < StarLs.Count; i++)
        {
            StarLs[i].OnEnableBox(false);
        }
        if (curStarIndexL >= StarLs.Count)
        {

            curStarIndexL = 0;
            if (isStar5) return;
            StartCoroutine(OnShowStar(StarLs));
        }
        if (curStarIndexL != 0)
        {
            StarLs[curStarIndexL].OnEnableBox(true);
        }
        curStarIndexL++;
        print("onStartStar:" + curStarIndexL);
    }

    public void OnEnableNextBoxR(int trigInd)
    {
        if (StarRs.Count == 0) return;
        triggerIndex_R = trigInd;
        for (int i = 0; i < StarRs.Count; i++)
        {
            StarRs[i].OnEnableBox(false);
        }
        if (curStarIndexR >= StarRs.Count)
        {

            curStarIndexR = 0;
            if (isStar5) return;
            StartCoroutine(OnShowStar(StarRs));
        }
        if (curStarIndexR != 0)
        { 
            StarRs[curStarIndexR].OnEnableBox(true);
        }
        print("onStartStar:" + curStarIndexR);
        curStarIndexR++;
    }

    IEnumerator OnShowStar(List<Star> stars)
    {
        yield return new WaitForSeconds(2);

        // 等待期间可能已切换链路并清空原列表，用快照避免越界或点亮新一轮目标。
        List<Star> snapshot = new List<Star>(stars);
        for (int i = 0; i < snapshot.Count; i++)
        {
            yield return new WaitForSeconds(0.2f);
            if (snapshot[i] == null) continue;
            snapshot[i].OnShowStar();
        }

        if (snapshot.Count > 0 && snapshot[0] != null) snapshot[0].OnEnableBox(true);
    }

}

public enum ActionAnim
{
    QianQu = 0,
    TiCeNXuan_L = 4,
    TiCeNXuan_R = 5,
    TiCeNXuan_Both = 6,   // 体侧内旋双侧：左右各一套目标
}

public enum Direction
{
    up, //0
    down,
    left,
    right,
}

public enum Difficulty
{ 
    easy,
    medium,
    hard,
}
