using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static int pagesCollected { get { return instance.pagesCollectedCount; } }
    public static bool isGameOver { get { return Time.timeScale > 0 ? false : true; } }

    public delegate void OnPageCollected();
    public static event OnPageCollected onPageCollected;

    [SerializeField, ReadOnly] private int pagesTotalCount;
    [SerializeField, ReadOnly] private int pagesCollectedCount;



    private void Awake()
    {
        instance = this;

        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;

        List<Page> pages = new List<Page>(FindObjectsOfType<Page>());
        while (pages.Count > 6) 
        {
            int randomIndex = Random.Range(0, pages.Count - 1);
            pages[randomIndex].gameObject.SetActive(false);
            pages.RemoveAt(randomIndex);
        }
        pagesTotalCount = FindObjectsOfType<Page>().Length;
    }

    private void OnValidate()
    {
        pagesTotalCount = FindObjectsOfType<Page>().Length;
    }

    public static void PageCollected()
    {
        instance.pagesCollectedCount++;
        onPageCollected?.Invoke();

        UIManager.UpdatePagesCount(instance.pagesCollectedCount, instance.pagesTotalCount);

        if (instance.pagesCollectedCount >= instance.pagesTotalCount)
            Debug.Log("All pages collected");
    }

    public static void GameOver()
    {
        Time.timeScale = 0f;

        UIManager.ShowGameOverScreen(); 

        Cursor.lockState = CursorLockMode.None;
    }

    public static void GameRestart()
    {
        Time.timeScale = 1f;

        LoadingManager.LoadScene("02-Game", false);
    }
}