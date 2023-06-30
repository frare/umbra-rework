using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Enemy enemyBehaviour;
    private Transform mainCam;
    private Animator animator;




    private void Awake()
    {
        enemyBehaviour = GetComponent<Enemy>();
        mainCam = Camera.main?.transform;
        animator = GetComponentInChildren<Animator>(true);
    }

    private void FixedUpdate()
    {
        bool isFacingCamera = Vector3.Dot(mainCam.forward, transform.forward) < 0f;
        animator.SetLayerWeight(0, isFacingCamera ? 1f : 0f);
        animator.SetLayerWeight(1, isFacingCamera ? 0f : 1f);

        animator.SetBool("isMoving", enemyBehaviour.isMoving);
        animator.SetBool("isChasing", enemyBehaviour.isChasing);
    }
}