using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighControl : MonoBehaviour
{
    public Transform tfMan;
    public Button btn;
    void Start()
    {
        btn.onClick.AddListener(OnUp);
    }

    public void OnUp()
    {
        isUp = true;
        isStart= true;
    }

    bool isStart;
    bool isUp = true;
    private void OnTriggerEnter(Collider other)
    {
        print(other);
        if (other.name == "HeadCd")
        {
            isUp = false;
            isStart = false;
        }
    }

    float size;
    void Update()
    {
    
        if (isUp&&isStart)
        {
            size += Time.deltaTime;
            tfMan.localScale = new Vector3(size, size, size);
        }
    }
}
