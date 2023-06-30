using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private TMP_Text pagesCount;
    [SerializeField] private float pagesCountFadeTime;
    [SerializeField] private Image sprintBar;
    [SerializeField] private Image gameLoseScreen;
    [SerializeField] private Image gameWinScreen;

    private Coroutine pagesCountFade;





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

    public static void UpdateSprintBar(float normalizedAmount)
    {
        if (instance == null) return;

        instance.sprintBar.fillAmount = normalizedAmount;
    }

    public static void ShowGameOverScreen()
    {
        if (instance == null) return;

        instance.gameLoseScreen.gameObject.SetActive(true);
    }

    public static void ShowGameWinScreen()
    {
        if (instance == null) return;
        
        instance.gameWinScreen.gameObject.SetActive(true);
    }

    public void OnPressed_Exit()
    {
        LoadingManager.LoadScene("01-Menu");

        Cursor.lockState = CursorLockMode.None;
    }

    public void OnPressed_Retry()
    {
        LevelManager.GameRestart();

        Cursor.lockState = CursorLockMode.Locked;
    }
}