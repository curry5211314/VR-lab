using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    public ActionAnim action;
    public int end;     //5,6,7
    public string actionName;   //后期共享
    public int difficulty;//难度
    Button btn_action;
    Text tt_lesson;
    int index;

    public string content;
    public AudioClip clip;

    void Start()
    {
        index = transform.GetSiblingIndex();
        btn_action= GetComponent<Button>();
        btn_action.onClick.AddListener(OnClickAction);
        tt_lesson = transform.Find("Text 1").GetComponent<Text>();
        tt_lesson.text = "康复训练第" + (transform.GetSiblingIndex() + 1).ToString() + "节";
    }

    void OnClickAction()
    {
        MainControl._instance.OnSceneInit(action,difficulty, end, clip, content,index);
        MainControl._instance.OnStartPanel();
    }
}

