using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] private Transform sprite;



    private void LateUpdate()
    {
        transform.localEulerAngles = sprite.localEulerAngles;
    }
}