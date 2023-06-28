using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum NavigationState { NONE, Wander, Chase };
    private NavigationState currentState = NavigationState.NONE;

    [SerializeField] private float wanderDistance;
    [SerializeField] private float wanderSpeed;
    [SerializeField] private float wanderRetargetTime;

    private Coroutine wanderCoroutine;

    [Space(15)]
    [SerializeField] private UnityEngine.AI.NavMeshAgent navMeshAgent;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private UnityEngine.Rendering.Volume postProcessVolume;

    // old stuff
    private Coroutine fadeOutCoroutine;
    private Coroutine stunCoroutine;
    public delegate void OnFadeOut();
    public event OnFadeOut onFadeOut;



    private void OnEnable()
    {
        meshRenderer.material.color = Color.white;
        navMeshAgent.isStopped = false;

        ChangeState(NavigationState.Wander);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == Player.layer && !LevelManager.isGameOver)
        {
            LevelManager.GameOver();
        }
    }




    // NEW STUFF 👇
    public void ChangeState(NavigationState targetState)
    {
        currentState = targetState;

        switch (currentState)
        {
            case NavigationState.NONE:
            break;

            case NavigationState.Wander:
            navMeshAgent.autoBraking = true;
            Wander();
            break;

            case NavigationState.Chase:
            navMeshAgent.autoBraking = false;
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
    // NEW STUFF 👆



    public void FadeOut()
    {
        if (!gameObject.activeSelf) return;

        if (fadeOutCoroutine == null) StartCoroutine(FadeOutCoroutine());
    }

    private IEnumerator FadeOutCoroutine()
    {
        Material material = meshRenderer.material;
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
