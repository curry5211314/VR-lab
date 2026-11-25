using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class XRInputR : MonoBehaviour
{
    public Transform tf_man;
    [Tooltip("移动速度，单位 m/s")] public float moveSpeed = 2.5f;
    public InputActionReference actionXY;
    public InputActionReference actionA;

    void Start()
    {
        actionXY.action.performed += ActionXY_performed;
        actionA.action.performed += OnActionA_trigger;
    }

    private void ActionXY_performed(InputAction.CallbackContext callback)
    {
        Vector2 axis = callback.ReadValue<Vector2>();
        //print("ActionXY_performed:"+ axis);

        // 计算世界方向
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = right.y = 0;          // 只保留水平
        forward.Normalize();
        right.Normalize();

        //Vector3 delta = (forward * axis.y + right * axis.x) * moveSpeed * Time.deltaTime;
        //tf_man.transform.position += delta;

    }

    //记录疼痛值
    void OnActionA_trigger(InputAction.CallbackContext callback)
    {
        MainControl._instance.OnSaveCsvData_R();
        print("isDown");
    }


    void OnDestroy()
    {
        // 移除回调以避免内存泄漏
        if (actionA != null && actionA.action != null)
        {
            actionXY.action.performed -= ActionXY_performed;
            actionA.action.performed -= OnActionA_trigger;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
