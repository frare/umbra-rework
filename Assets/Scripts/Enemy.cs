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
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float wanderDistance;
    [SerializeField] private float wanderRetargetTime;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float chaseGiveupTime;
    [SerializeField] private float grabAnimationDuration;

    [Header("References")]
    [SerializeField] private Transform handPivot;
    [SerializeField] private Transform headPivot;

    // Other components
    private NavMeshAgent navMeshAgent;
    private new Renderer renderer;
    private UnityEngine.Rendering.Volume postProcessVolume;

    private Coroutine wanderCoroutine;





    // Unity callbacks
    private void OnValidate()
    {
        navMeshAgent = GetComponentInChildren<NavMeshAgent>(true);
        renderer = GetComponentInChildren<Renderer>(true);
        postProcessVolume = GetComponentInChildren<UnityEngine.Rendering.Volume>(true);
    }

    private void OnEnable()
    {
        renderer.material.color = Color.white;
        navMeshAgent.isStopped = false;

        SetNavigationState(currentState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Player.layer && !LevelManager.isGameOver)
        {
            GrabPlayer();
        }
    }

    private void Update()
    {
        if (currentState == NavigationState.Chase) navMeshAgent.destination = Player.position;
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
            Wander();
            break;

            case NavigationState.Chase:
            navMeshAgent.autoBraking = false;
            navMeshAgent.speed = chaseSpeed;
            Chase();
            break;
        }

        if (previousState == NavigationState.NONE) navMeshAgent.isStopped = false;
    }

    private void Wander()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderDistance;
        
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection + Player.position, out navHit, wanderDistance, 1);
        navMeshAgent.destination = navHit.position;

        wanderCoroutine = StartCoroutine(Wander_GetNextDestination());
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
        
        Wander();
    }

    private void Chase()
    {
        // stops moving, do a jumpscare,
        // then starts chasing

        // should check if player is in line of sight for X seconds,
        // if not, returns to wandering
    }

    private void GrabPlayer()
    {
        SetNavigationState(NavigationState.NONE);
        navMeshAgent.velocity = Vector3.zero;
        Player.Grabbed();
        StartCoroutine(GrabPlayerSmooth());
    }

    private IEnumerator GrabPlayerSmooth()
    {
        Vector3 initialPosition = Player.position;
        Quaternion initialRotation = Player.rotation;
        float time = 0f;
        float normalizedTime = 0f;
        while (time < grabAnimationDuration)
        {
            time += Time.deltaTime;
            normalizedTime = time / grabAnimationDuration;

            Player.position = Vector3.Slerp(initialPosition, handPivot.position, normalizedTime);
            Player.rotation = Quaternion.Slerp(initialRotation, Quaternion.LookRotation(headPivot.position - handPivot.position), normalizedTime);

            yield return null;
        }

        Player.position = handPivot.position;
        Player.rotation = Quaternion.LookRotation(headPivot.position - handPivot.position);

        yield return new WaitForSeconds(3f);

        LevelManager.GameLose();
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
        Material material = renderer.material;
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
