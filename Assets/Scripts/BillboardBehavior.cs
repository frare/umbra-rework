using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardBehavior : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform.position, Vector3.up);
    }
}