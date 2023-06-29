using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAnimator : MonoBehaviour
{
    private Enemy enemyBehaviour;
    private Animator animator;
    private Transform mainCam;



    private void OnEnable()
    {
        // could help solve the issue with animating the a mesh instead of sprite
        // GetComponentInChildren<SpriteRenderer>().RegisterSpriteChangeCallback(OnSpriteChanged);
    }

    private void OnValidate()
    {
        enemyBehaviour = GetComponent<Enemy>();
        animator = GetComponentInChildren<Animator>(true);
        mainCam = Camera.main?.transform;
    }

    private void Update()
    {
        bool isFacingCamera = Vector3.Dot(mainCam.forward, transform.forward) < 0f;
        animator.SetLayerWeight(0, isFacingCamera ? 1f : 0f);
        animator.SetLayerWeight(1, isFacingCamera ? 0f : 1f);

        animator.SetBool("isMoving", enemyBehaviour.isMoving);
        animator.SetBool("isChasing", enemyBehaviour.isChasing);
    }
}