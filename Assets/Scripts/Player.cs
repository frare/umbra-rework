using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player instance;
    public static int layer { get { return 8; } private set {  } }
    public static Vector3 position { get { return instance.transform.position; } private set { } }

    [Header("Player Attributes")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float rotationSmoothTime = 0.12f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float interactDistance = 2.5f;
    private float speed;
    private const float speedOffset = 0.1f;
    private float targetRotation = 0f;
    private float rotationVelocity;
    private float verticalVelocity;

    [Header("Cinemachine")]
    [SerializeField] private Transform cinemachineCameraTarget;
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
    [SerializeField] private Transform flashlight;
    [SerializeField] private NightVision nightVision;
    private Transform mainCamera;
    private UnityEngine.UI.Image reticle;
    private RaycastHit reticleHit;



    private void Awake()
    {
        Player.instance = this;
    }

    private void Start()
    {
        cinemachineTargetYaw = cinemachineCameraTarget.eulerAngles.y;

        input.onVisorPressed += nightVision.TryToEnable;
        input.onInteractPressed += TryInteract;

        mainCamera = Camera.main.transform;
    }

    private void Update()
    {
        Move();
        UpdateReticle();
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
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + mainCamera.eulerAngles.y;
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

        cinemachineCameraTarget.rotation = 
            Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride, cinemachineTargetYaw, 0.0f);
    }

    private void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, Mathf.Infinity, 1 << Page.layer | 1 << 7, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject.layer == Page.layer &&
                Vector3.Distance(transform.position, hit.transform.position) <= interactDistance)
            {
                hit.collider.gameObject.GetComponent<Page>().Collect();
            }
        }
    }

    private void UpdateReticle()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out reticleHit, Mathf.Infinity, 1 << Page.layer, QueryTriggerInteraction.Collide))
        {
            if (reticleHit.collider.gameObject.layer == Page.layer &&
                Vector3.Distance(transform.position, reticleHit.transform.position) <= interactDistance)
            {
                UIManager.SetReticleSize(true);
                return;
            }
        }
        
        UIManager.SetReticleSize(false);
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}