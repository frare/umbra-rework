using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance { get; private set; }

    public static bool loading { get { return instance.isLoading; } private set { instance.isLoading = value; } }
    private bool isLoading;

    [Header("References")]
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform loadingIcon;

    [Header("Attributes")]
    [SerializeField] private float slideTime;
    [SerializeField] private float minimumDuration;
    [SerializeField] private float iconRotationSpeed;

    private Coroutine coroutineSlide;





    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(this.gameObject);

        canvas.gameObject.SetActive(true);
        panel.anchoredPosition = new Vector2(canvas.rect.width, 0f);
        canvas.gameObject.SetActive(false);
        isLoading = false;
    }

    private void Update()
    {
        loadingIcon.localEulerAngles += new Vector3(0f, 0f, iconRotationSpeed * Time.deltaTime);
    }



    public static void LoadScene(string sceneName)
    {
        if (!loading) 
        {
            instance.StartCoroutine(instance.Coroutine_LoadScene(sceneName));
        }
    }

    private IEnumerator Coroutine_LoadScene(string sceneName)
    {
        SetVisibility(true);
        yield return new WaitForSecondsRealtime(slideTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoad.isDone) yield return null;

        yield return new WaitForSecondsRealtime(minimumDuration);
        yield return new WaitForSecondsRealtime(slideTime);
        SetVisibility(false);
    }

    private void SetVisibility(bool active)
    {
        if (coroutineSlide != null) StopCoroutine(coroutineSlide);
        coroutineSlide = StartCoroutine(Coroutine_SetActive(active));
    }

    private IEnumerator Coroutine_SetActive(bool active)
    {
        if (active) canvas.gameObject.SetActive(true);

        float time = 0f;
        Vector2 initialPosition = panel.anchoredPosition;
        Vector2 targetPosition = active ? Vector2.zero : new Vector2(canvas.rect.width, 0);
        while (time < slideTime)
        {
            time += Time.unscaledDeltaTime;
            panel.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, time / slideTime);
            yield return null;
        }
        panel.anchoredPosition = targetPosition;

        if (!active) canvas.gameObject.SetActive(false);
        this.enabled = active;
        isLoading = active;
    }
}
