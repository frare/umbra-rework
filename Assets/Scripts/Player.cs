using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : Singleton<Player>
{
    public static int layer { get { return 8; } }
    public static Vector3 position { get { return instance.transform.position; } }
    public static bool grabbed { get; private set; }

    [Header("Player Attributes")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float sprintSpeed = 5f;
    [SerializeField] private float sprintDuration = 1f;
    [SerializeField] private float sprintRechargeDelay;
    [SerializeField] private float sprintRechargeSpeed;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float interactDistance = 2.5f;
    private float speed;
    private const float speedOffset = 0.1f;
    private float sprintEnergy;
    private float sprintCurrentRecharge;
    private bool canSprint { get { return sprintEnergy > 0f; } }
    private bool isSprinting { get { return input.sprint && canSprint; } }
    private float rotationVelocity;
    private float verticalVelocity;

    [Header("Cinemachine")]
    [SerializeField] private Transform cinemachineCameraTarget;
    [SerializeField] private float cameraTopClamp = 70f;
    [SerializeField] private float cameraBottomClamp = -30f;
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




 
    private void Start()
    {
        cinemachineTargetYaw = cinemachineCameraTarget.eulerAngles.y;

        input.onVisorPressed += nightVision.TryToEnable;
        input.onInteractPressed += TryInteract;

        mainCamera = Camera.main.transform;

        sprintEnergy = sprintDuration;
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

    

    private void CameraRotation()
    {
        if (input.look.sqrMagnitude >= threshold)
        {
            float deltaTimeMultiplier = isCurrentDeviceMouse ? 1.0f : Time.deltaTime;
            
            cinemachineTargetPitch += input.look.y * rotationSpeed * deltaTimeMultiplier;
            rotationVelocity = input.look.x * rotationSpeed * deltaTimeMultiplier;

            cinemachineTargetPitch = ClampAngle(cinemachineTargetPitch, cameraBottomClamp, cameraTopClamp);

            cinemachineCameraTarget.transform.localRotation = Quaternion.Euler(cinemachineTargetPitch, 0.0f, 0.0f);

            transform.Rotate(Vector3.up * rotationVelocity);
        }
    }

    private void Move()
    {
        Sprint();

        float targetSpeed = isSprinting ? sprintSpeed : moveSpeed;
        if (input.move == Vector2.zero) targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * acceleration);

            speed = Mathf.Round(speed * 1000f) / 1000f;
        }
        else speed = targetSpeed;

        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;
        if (input.move != Vector2.zero)
        {
            inputDirection = transform.right * input.move.x + transform.forward * input.move.y;
        }

        controller.Move(inputDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }

    private void Sprint()
    {
        // if (Input.GetKeyDown(KeyCode.F1)) sprintEnergy = sprintDuration;

        if (isSprinting) 
        {
            sprintEnergy -= Time.deltaTime;
            sprintCurrentRecharge = 0f;
        }
        else sprintCurrentRecharge += Time.deltaTime;

        if (sprintCurrentRecharge >= sprintRechargeDelay && sprintEnergy < sprintDuration) 
            sprintEnergy += Time.deltaTime * sprintRechargeSpeed;

        UIManager.UpdateSprintBar(sprintEnergy / sprintDuration);
    }

    private void TryInteract()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, Mathf.Infinity, 1 << Collectible.layer | 1 << 7, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject.layer == Collectible.layer &&
                Vector3.Distance(transform.position, hit.transform.position) <= interactDistance)
            {
                hit.collider.gameObject.GetComponent<Collectible>().Collect();
            }
        }
    }

    private void UpdateReticle()
    {
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out reticleHit, Mathf.Infinity, 1 << Collectible.layer, QueryTriggerInteraction.Collide))
        {
            if (reticleHit.collider.gameObject.layer == Collectible.layer &&
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