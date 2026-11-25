using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class XRInputL : MonoBehaviour
{
    public InputActionReference actionA;

    void Start()
    {
        actionA.action.performed += OnActionA_trigger;
    }


    //记录疼痛值
    void OnActionA_trigger(InputAction.CallbackContext callback)
    {
        MainControl._instance.OnSaveCsvData_L();
        print("isDownL");
    }

    void OnDestroy()
    {
        // 移除回调以避免内存泄漏
        if (actionA != null && actionA.action != null)
        {
            actionA.action.performed -= OnActionA_trigger;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
