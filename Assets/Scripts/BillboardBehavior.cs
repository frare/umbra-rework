using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardBehavior : MonoBehaviour
{
    [SerializeField] private bool ignoreX = false;



    private void LateUpdate()
    {
        transform.forward = this.transform.position - Camera.main.transform.position;
        if (ignoreX) transform.localEulerAngles = new Vector3(0f, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
}