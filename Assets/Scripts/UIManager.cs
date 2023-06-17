using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private Image visorCooldown;
    [SerializeField] private RectTransform reticle;
    [SerializeField] private TMP_Text pagesCount;
    [SerializeField] private float pagesCountFadeTime;

    private Coroutine pagesCountFade;



    private void Awake()
    {
        instance = this;

        visorCooldown.fillAmount = 0f;
    }

    public static void UpdateVisorCooldown(float normalizedTime)
    {
        instance.visorCooldown.fillAmount = normalizedTime;
    }

    public static void UpdatePagesCount(int pagesCollected, int pagesTotal)
    {
        if (instance.pagesCountFade != null) instance.StopCoroutine(instance.pagesCountFade);
        instance.pagesCount.text = $"{pagesCollected}/{pagesTotal}";
        instance.StartCoroutine(instance.PagesCountFade());
    }

    private IEnumerator PagesCountFade()
    {
        float time = 0f;
        float normalizedTime = 0f;
        while (time < pagesCountFadeTime)
        {
            time += Time.deltaTime;
            normalizedTime = time / pagesCountFadeTime;

            pagesCount.color = Color.Lerp(Color.white, Color.clear, normalizedTime);

            yield return null;
        }

        pagesCount.color = Color.clear;
    }

    private static Vector2 reticleBigSize = new Vector2(50, 50);
    private static Vector2 reticleSmallSize = new Vector2(25, 25);
    public static void SetReticleSize(bool interactable)
    {
        instance.reticle.sizeDelta = interactable ? reticleBigSize : reticleSmallSize;
    }

    public void OnPressed_Exit()
    {
        LoadingManager.LoadScene("01-Menu");
    }
}