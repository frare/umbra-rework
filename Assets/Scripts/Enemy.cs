using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Singleton<Enemy>
{
    // Static fields
    public static int layer { get { return 13;} }
    public static Vector3 position { get { return instance.transform.position; } }

    public enum NavigationState { NONE, Wander, Chase };

    // Properties
    public bool isMoving { get { return navMeshAgent.velocity.sqrMagnitude > 0f; } }
    public bool isChasing { get { return currentState == NavigationState.Chase; } }

    // Inspector attributes
    [SerializeField] private NavigationState currentState;
    [SerializeField, ReadOnly] private float detection;
    [SerializeField] private float detectionRange;
    [SerializeField] private float detectionDuration;
    [SerializeField] private float detectionCooldownSpeed;
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float wanderDistance;
    [SerializeField] private float wanderRetargetTime;
    [SerializeField] private float wanderTeleportDistance;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float chaseDuration;
    [SerializeField] private float grabAnimationDuration;

    [Header("References")]
    [SerializeField] private Transform hand;
    [SerializeField] private Transform head;
    [SerializeField] private Sprite grabSprite;

    // Other components
    private NavMeshAgent navMeshAgent;
    private SpriteRenderer spriteRenderer;
    private UnityEngine.Rendering.Volume postProcessVolume;

    private Coroutine wanderCoroutine;





    // Unity callbacks
    private void Awake()
    {
        navMeshAgent = GetComponentInChildren<NavMeshAgent>(true);
        spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        postProcessVolume = GetComponentInChildren<UnityEngine.Rendering.Volume>(true);
    }

    private void OnEnable()
    {
        spriteRenderer.material.color = Color.white;
        navMeshAgent.isStopped = false;

        SetNavigationState(currentState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Player.layer && !LevelManager.isGameOver)
        {
            Chase_GrabPlayer();
        }
    }

    private void Update()
    {
        if (currentState == NavigationState.Wander)
        {
            if (Vector3.Distance(transform.position, Player.position) > wanderTeleportDistance) 
                Wander_Teleport();
            else
                Wander_Loop();
        }
        else if (currentState == NavigationState.Chase) Chase_Loop();
    }





    // Class methods
    public void SetNavigationState(NavigationState targetState)
    {
        // eventually should probably change so that each navigation state have its own behaviour
        NavigationState previousState = currentState;
        currentState = targetState;

        switch (targetState)
        {
            case NavigationState.NONE:
            navMeshAgent.isStopped = true;
            break;

            case NavigationState.Wander:
            navMeshAgent.autoBraking = true;
            navMeshAgent.speed = wanderSpeed;
            detection = 0f;
            Wander_Begin();
            break;

            case NavigationState.Chase:
            navMeshAgent.autoBraking = false;
            navMeshAgent.speed = chaseSpeed;
            detection = chaseDuration;
            break;
        }

        if (previousState == NavigationState.NONE) navMeshAgent.isStopped = false;
    }

    private void Wander_Begin()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderDistance;
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection + Player.position, out navHit, wanderDistance, 1))
        {
            navMeshAgent.destination = navHit.position;
            wanderCoroutine = StartCoroutine(Wander_GetNextDestination());
        }
    }

    private void Wander_Loop()
    {
        RaycastHit hit;
        if (Physics.Raycast(head.position, Player.position - head.position, out hit, 
            detectionRange, 1 << Player.layer | 1 << 7, QueryTriggerInteraction.Collide))
        {
            if (hit.transform.gameObject.layer == Player.layer)
            {
                detection += Time.deltaTime;
                navMeshAgent.speed = 0f;
                transform.rotation = Quaternion.LookRotation(Player.position - transform.position);
                if (detection >= detectionDuration) SetNavigationState(NavigationState.Chase);
                return;
            }
        }

        navMeshAgent.speed = wanderSpeed;
        detection -= Time.deltaTime * detectionCooldownSpeed;
    }

    private IEnumerator Wander_GetNextDestination()
    {
        float time = 0f;
        while (time < 30f)
        {
            time += Time.deltaTime;
            if (Vector3.Distance(transform.position, navMeshAgent.destination) < .1f) break;
            yield return null;
        }

        yield return new WaitForSeconds(wanderRetargetTime);
        
        Wander_Begin();
    }

    private void Wander_Teleport()
    {
        Vector3 randomDirection = (Random.insideUnitSphere).normalized * (wanderDistance + 5f);
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(randomDirection + Player.position, out navHit, wanderDistance, 1))
            transform.position = navHit.position;
    }

    private void Chase_Loop()
    {
        // stops moving, do a jumpscare,
        // then starts chasing

        RaycastHit hit;
        if (Physics.Raycast(head.position, Player.position - head.position, out hit, 
            detectionRange, 1 << Player.layer | 1 << 7, QueryTriggerInteraction.Collide))
        {
            if (hit.transform.gameObject.layer == Player.layer)
            {
                navMeshAgent.destination = Player.position;
                return;
            }
        }

        detection -= Time.deltaTime;
        if (detection <= 0f) SetNavigationState(NavigationState.Wander);
    }

    private void Chase_GrabPlayer()
    {
        GetComponentInChildren<Animator>().enabled = false;
        spriteRenderer.sprite = grabSprite;
        SetNavigationState(NavigationState.NONE);
        navMeshAgent.velocity = Vector3.zero;
        Player.Grabbed();
        StartCoroutine(Chase_GrabPlayerSmooth());
    }

    private IEnumerator Chase_GrabPlayerSmooth()
    {
        Vector3 initialPosition = Player.position;
        Quaternion initialRotation = Player.cameraRotation;
        float time = 0f;
        float normalizedTime = 0f;
        while (time < grabAnimationDuration)
        {
            time += Time.deltaTime;
            normalizedTime = time / grabAnimationDuration;

            Player.position = Vector3.Slerp(initialPosition, hand.position, normalizedTime);
            Quaternion lookDirection = Quaternion.LookRotation(head.position - (hand.position + new Vector3(0f, 1.69f, 0f)));
            Player.cameraRotation = Quaternion.Slerp(initialRotation, lookDirection, normalizedTime);

            yield return null;
        }

        Player.position = hand.position;
        Player.cameraRotation = Quaternion.LookRotation(head.position - (hand.position + new Vector3(0f, 1.69f, 0f)));

        yield return new WaitForSeconds(3f);

        LevelManager.GameEnd();
    }

    public static void UpdateSpeed()
    {
        instance.chaseSpeed *= 1.2f;
        instance.wanderSpeed *= 1.2f;
    }





    // old stuff 👇👇👇
    private Coroutine fadeOutCoroutine;
    private Coroutine stunCoroutine;
    public delegate void OnFadeOut();
    public event OnFadeOut onFadeOut;

    public void FadeOut()
    {
        if (!gameObject.activeSelf) return;

        if (fadeOutCoroutine == null) StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        Material material = spriteRenderer.material;
        Color colorStart = material.color;
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;

            material.color = Color.Lerp(colorStart, Color.clear, time / 1f);
            postProcessVolume.weight = time / 1f;

            yield return null;
        }

        onFadeOut?.Invoke();
        fadeOutCoroutine = null;
        gameObject.SetActive(false);
    }

    public void Stun(float duration)
    {
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        stunCoroutine = StartCoroutine(StunCoroutine(duration));
    }

    private IEnumerator StunCoroutine(float duration)
    {
        navMeshAgent.isStopped = true;

        yield return new WaitForSeconds(duration);

        navMeshAgent.isStopped = false;
    }
}
