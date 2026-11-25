using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePos : MonoBehaviour
{
    public Transform tf_ppos;
    void Start()
    {
        
    }

    void Update()
    {
        transform.position= tf_ppos.position;
    }
}
