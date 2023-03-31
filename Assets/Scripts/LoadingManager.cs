using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance { get; private set; }

    public static bool loading { get { return instance.isLoading; } private set { instance.isLoading = value; } }
    private bool isLoading;

    [Header("References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text textLoading;

    [Header("Attributes")]
    [SerializeField] private float slideTime;

    private Coroutine coroutineSlide;



    private void Awake()
    {
        if (instance == null ) instance = this;
        else Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);

        panel.anchoredPosition = new Vector2(1920f, 0f);
        isLoading = false;
    }

    private void Update()
    {
        textLoading.rectTransform.localEulerAngles += new Vector3(0f, 0f, Time.deltaTime * 2.5f);
        textLoading.rectTransform.localScale = new Vector3(1f + Mathf.Sin(Time.time) / 2, 1f + Mathf.Sin(Time.time) / 2, 0f);
    }



    public static void LoadScene(string sceneName)
    {
        if (!loading) instance.StartCoroutine(instance.Coroutine_LoadScene(sceneName));
    }

    private IEnumerator Coroutine_LoadScene(string sceneName)
    {
        isLoading = true;
        instance.enabled = true;
        SetVisibility(true);
        yield return new WaitForSeconds(slideTime);

        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        yield return new WaitForSeconds(slideTime);
        SetVisibility(false);
        instance.enabled = false;
        isLoading = false;
    }

    private void SetVisibility(bool active)
    {
        if (coroutineSlide != null) StopCoroutine(coroutineSlide);
        coroutineSlide = StartCoroutine(Coroutine_SetActive(active));
    }

    private IEnumerator Coroutine_SetActive(bool active)
    {
        if (active) panel.gameObject.SetActive(true);

        float time = 0f;
        Vector2 initialPosition = panel.anchoredPosition;
        Vector2 targetPosition = active ? Vector2.zero : new Vector2(1920, 0);
        while (time < slideTime)
        {
            time += Time.deltaTime;
            panel.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, time / slideTime);
            yield return null;
        }
        panel.anchoredPosition = targetPosition;

        if (!active) panel.gameObject.SetActive(false);
    }
}
