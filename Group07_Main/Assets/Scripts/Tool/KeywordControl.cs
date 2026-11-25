#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

//window平台可用
public class KeywordControl : MonoBehaviour
{
    public static KeywordControl _Instance;
    [SerializeField]
    string[] m_Keywords;

    private KeywordRecognizer m_Recognizer;
    public Text tt_log;

    private void Start()
    {
        _Instance = this;
        OnSetKeywords(new string[] {"准备好了"});
    }

    public void OnSetKeywords(string[] keys)
    {
        m_Keywords = keys;
        m_Recognizer = new KeywordRecognizer(m_Keywords);
        m_Recognizer.OnPhraseRecognized += OnPhraseRecognized;
        m_Recognizer.Start();
        tt_log.text = "准备好了吗？";
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (args.text == m_Keywords[0])
        {
            Debug.Log("<color=green>说了 准备好了！</color>");
        }
        StringBuilder builder = new StringBuilder();
        builder.AppendFormat("{0} ({1}){2}", args.text, args.confidence, Environment.NewLine);
        builder.AppendFormat("\tTimestamp: {0}{1}", args.phraseStartTime, Environment.NewLine);
        builder.AppendFormat("\tDuration: {0} seconds{1}", args.phraseDuration.TotalSeconds, Environment.NewLine);
        tt_log.text = builder.ToString();
        Debug.Log(builder.ToString());
    }
}
#endif