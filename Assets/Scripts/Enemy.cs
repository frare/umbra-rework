using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private UnityEngine.AI.NavMeshAgent navMeshAgent;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private UnityEngine.Rendering.Volume postProcessVolume;

    private Coroutine fadeOutCoroutine;
    private Coroutine stunCoroutine;

    public delegate void OnFadeOut();
    public event OnFadeOut onFadeOut;



    private void OnEnable()
    {
        meshRenderer.material.color = Color.white;
    }

    private void Update()
    {
        navMeshAgent.destination = Player.position;
    }

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
