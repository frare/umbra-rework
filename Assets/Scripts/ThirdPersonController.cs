using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float acceleration = 10f;
    private float speed;
    private float targetRotation = 0f;
    private float rotationVelocity;
    private float verticalVelocity;
    private float terminalVelocity = 53f;
    
    [Space(10)]
    [SerializeField] private float jumpHeight = 1.2f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float jumpTimeout = 0.5f;
    [SerializeField] private float fallTimeout = 0.15f;
    [SerializeField] private bool grounded = true;
    [SerializeField] private float groundOffset = -0.14f;
    [SerializeField] private float groundedRadius = 0.28f;
    [SerializeField] private LayerMask groundLayers;
    private float jumpTimeoutDelta;
    private float fallTimeoutDelta;

    [Header("Cinemachine")]
    [SerializeField] private GameObject cinemachineCameraTarget;
    [SerializeField] private float cameraTopClamp = 70f;
    [SerializeField] private float cameraBottomClamp = -30f;
    [SerializeField] private float cameraAngleOverride = 0f;
    [SerializeField] private bool cameraLockRotation = false;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private const float threshold = 0.01f;
    private bool isCurrentDeviceMouse = true; // temporário, alterar para ser dinâmico de acordo com o input

    [Header("Components")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CharacterController controller;
    [SerializeField] private InputManager input;
    [SerializeField] private GameObject mainCamera;



    private void Start()
    {
        cinemachineTargetYaw = cinemachineCameraTarget.transform.eulerAngles.y;
    
        jumpTimeoutDelta = jumpTimeout;
        fallTimeoutDelta = fallTimeout;
    }

    private void Update()
    {
        JumpAndGravity();
        GroundedCheck();
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }


    private void JumpAndGravity()
    {

    }

    private void GroundedCheck()
    {
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundOffset, transform.position.z);
        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void Move()
    {

    }

    private void CameraRotation()
    {
        if (input.look.sqrMagnitude >= threshold && !cameraLockRotation)
        {
            float deltaTimeMultiplier = isCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            cinemachineTargetYaw += input.look.x * deltaTimeMultiplier;
            cinemachineTargetPitch += input.look.y * deltaTimeMultiplier;
        }

        cinemachineTargetYaw = ClampAngle(cinemachineTargetYaw, float.MinValue, float.MaxValue);
        cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, cameraBottomClamp, cameraTopClamp);

        cinemachineCameraTarget.transform.rotation = 
            Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}
