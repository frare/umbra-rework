using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum NavigationState { NONE, Wander, Chase };

    // Properties
    public bool isMoving { get { return navMeshAgent.velocity.sqrMagnitude > 0f; } }
    public bool isChasing { get { return currentState == NavigationState.Chase; } }

    // Attributes
    [SerializeField] private NavigationState currentState = NavigationState.NONE;
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float wanderDistance;
    [SerializeField] private float wanderRetargetTime;
    [SerializeField] private float chaseSpeed;
    [SerializeField] private float chaseDurationWithoutLOS;

    // References
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

        ChangeState(currentState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Player.layer && !LevelManager.isGameOver)
        {
            LevelManager.GameOver();

            // change to jumpscare, then gameover
        }
    }

    private void Update()
    {
        if (currentState == NavigationState.Chase) navMeshAgent.destination = Player.position;
    }


    // Class methods
    public void ChangeState(NavigationState targetState)
    {
        NavigationState previousState = currentState;
        currentState = targetState;

        switch (targetState)
        {
            case NavigationState.NONE:
            navMeshAgent.destination = transform.position;
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
    }

    private void Wander()
    {
        Vector3 randomDirection = Random.insideUnitSphere * wanderDistance;
        randomDirection += Player.position;
        
        NavMeshHit navHit;
        NavMesh.SamplePosition(randomDirection, out navHit, wanderDistance, 1);
        navMeshAgent.destination = navHit.position;

        wanderCoroutine = StartCoroutine(WanderCoroutine());
    }

    private IEnumerator WanderCoroutine()
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
        // stops moving,
        // do jumpscare,
        // then starts chasing

        // should check if player is in line of sight for X seconds,
        // if not, returns to wandering
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
