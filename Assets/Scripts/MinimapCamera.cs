using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] private Transform positionToFollow;
    [SerializeField] private Transform rotationToFollow;
    
    private Vector3 positionOffset;
    private Vector3 rotationOffset;



    private void Awake()
    {
        positionOffset = transform.localPosition;
        rotationOffset = transform.localEulerAngles;
    }

    private void LateUpdate()
    {
        transform.position = positionToFollow.position + positionOffset;
        transform.eulerAngles = rotationToFollow.eulerAngles + rotationOffset;
    }
}