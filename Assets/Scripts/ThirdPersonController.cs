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
    private const float speedOffset = 0.1f;
    private float targetRotation = 0f;
    private float rotationVelocity;
    private float verticalVelocity;

    [Header("Cinemachine")]
    [SerializeField] private GameObject cinemachineCameraTarget;
    [SerializeField] private float cameraTopClamp = 70f;
    [SerializeField] private float cameraBottomClamp = -30f;
    [SerializeField] private float cameraAngleOverride = 0f;
    [SerializeField] private bool cameraLockRotation = false;
    private float cinemachineTargetYaw;
    private float cinemachineTargetPitch;
    private const float threshold = 0.01f;
    private bool isCurrentDeviceMouse { get { return playerInput.currentControlScheme == "KeyboardMouse"; } }

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private CharacterController controller;
    [SerializeField] private InputManager input;
    [SerializeField] private GameObject mainCamera;



    private void Start()
    {
        cinemachineTargetYaw = cinemachineCameraTarget.transform.eulerAngles.y;
    }

    private void Update()
    {
        Move();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    

    private void Move()
    {
        float targetSpeed = input.move == Vector2.zero ? 0f : (input.sprint ? sprintSpeed : moveSpeed);
        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0f, controller.velocity.z).magnitude;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * acceleration);
            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else speed = targetSpeed;

        Vector3 inputDirection = new Vector3(input.move.x, 0f, input.move.y).normalized;
        if (input.move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.transform.eulerAngles.y;
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
        }

        Vector3 targetDirection = (Quaternion.Euler(0f, targetRotation, 0f) * Vector3.forward).normalized;

        controller.Move(targetDirection * speed * Time.deltaTime + new Vector3(0f, verticalVelocity, 0f) * Time.deltaTime);
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
