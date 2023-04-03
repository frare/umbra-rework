using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardBehavior : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.forward = this.transform.position - Camera.main.transform.position;
        // transform.LookAt(Camera.main.transform.position, Vector3.up);
    }
}