using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Player playerBehaviour;
    private Animator animator;




    private void Awake()
    {
        playerBehaviour = GetComponentInParent<Player>();
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        animator.SetBool("isMoving", playerBehaviour.isMoving);
        animator.SetBool("isSprinting", playerBehaviour.isSprinting);
    }
}