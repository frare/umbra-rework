using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager instance { get; private set; }

    public static bool loading { get { return instance.isLoading; } private set { instance.isLoading = value; } }
    private bool isLoading;

    [Header("References")]
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform panel;
    [SerializeField] private RectTransform minutesHandle;
    [SerializeField] private RectTransform hoursHandle;

    [Header("Attributes")]
    [SerializeField] private float slideTime;
    [SerializeField] private float minDuration;
    [SerializeField] private float minutesSpeed;
    [SerializeField] private float hoursSpeed;

    private Coroutine coroutineSlide;
    private bool timeForward = true;



    private void Awake()
    {
        if (instance == null ) instance = this;
        else Destroy(this.gameObject);
        DontDestroyOnLoad(gameObject);

        canvas.gameObject.SetActive(true);
        panel.anchoredPosition = new Vector2(canvas.rect.width, 0f);
        canvas.gameObject.SetActive(false);
        isLoading = false;
    }

    private void Update()
    {
        minutesHandle.localEulerAngles += new Vector3(0f, 0f, minutesSpeed * (timeForward ? -1f : 1f) * Time.deltaTime);
        hoursHandle.localEulerAngles += new Vector3(0f, 0f, hoursSpeed * (timeForward ? -1f : 1f) * Time.deltaTime);
    }



    public static void LoadScene(string sceneName, bool timeForward = true)
    {
        if (!loading) 
        {
            instance.timeForward = timeForward;
            instance.StartCoroutine(instance.Coroutine_LoadScene(sceneName));
        }
    }

    private IEnumerator Coroutine_LoadScene(string sceneName)
    {
        SetVisibility(true);
        yield return new WaitForSeconds(slideTime);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        while (!asyncLoad.isDone) yield return null;

        yield return new WaitForSeconds(minDuration);
        yield return new WaitForSeconds(slideTime);
        SetVisibility(false);
    }

    private void SetVisibility(bool active)
    {
        if (active)
        {
            minutesHandle.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 359f));
            hoursHandle.localEulerAngles = new Vector3(0f, 0f, Random.Range(0f, 359f));
        }

        if (coroutineSlide != null) StopCoroutine(coroutineSlide);
        coroutineSlide = StartCoroutine(Coroutine_SetActive(active));

        instance.enabled = active;
        isLoading = active;
    }

    private IEnumerator Coroutine_SetActive(bool active)
    {
        if (active) canvas.gameObject.SetActive(true);

        float time = 0f;
        Vector2 initialPosition = panel.anchoredPosition;
        Vector2 targetPosition = active ? Vector2.zero : new Vector2(canvas.rect.width, 0);
        while (time < slideTime)
        {
            time += Time.deltaTime;
            panel.anchoredPosition = Vector2.Lerp(initialPosition, targetPosition, time / slideTime);
            yield return null;
        }
        panel.anchoredPosition = targetPosition;

        if (!active) canvas.gameObject.SetActive(false);
    }
}
