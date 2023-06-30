using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    public static int pagesCollected { get { return instance.pagesCollectedCount; } }
    public static bool isGameOver { get { return Time.timeScale > 0 ? false : true; } }

    public delegate void OnPageCollected();
    public static event OnPageCollected onPageCollected;

    [SerializeField, ReadOnly] private int pagesTotalCount;
    [SerializeField, ReadOnly] private int pagesCollectedCount;



    private void Awake()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;

        List<Collectible> collectibles = new List<Collectible>(FindObjectsOfType<Collectible>());
        /*
        while (collectibles.Count > 6) 
        {
            int randomIndex = Random.Range(0, collectibles.Count - 1);
            collectibles[randomIndex].gameObject.SetActive(false);
            collectibles.RemoveAt(randomIndex);
        }
        */
        pagesTotalCount = FindObjectsOfType<Collectible>().Length;
    }

    private void OnValidate()
    {
        pagesTotalCount = FindObjectsOfType<Collectible>().Length;
    }

    public static void PageCollected()
    {
        if (instance == null) return;

        instance.pagesCollectedCount++;
        onPageCollected?.Invoke();

        if (instance.pagesCollectedCount == 1) FindObjectOfType<Enemy>(true).gameObject.SetActive(true);

        UIManager.UpdatePagesCount(instance.pagesCollectedCount, instance.pagesTotalCount / 2);
        Enemy.UpdateSpeed();
    }

    public static void GameEnd()
    {
        if (instance.pagesCollectedCount >= instance.pagesTotalCount / 2) GameWin();
        else GameLose();
    }

    private static void GameWin()
    {
        // stops everything, then start the final cutscene

        Time.timeScale = 0f;
        UIManager.ShowGameWinScreen();
        Cursor.lockState = CursorLockMode.None;
    }

    public static void GameLose()
    {
        Time.timeScale = 0f;
        UIManager.ShowGameOverScreen(); 
        Cursor.lockState = CursorLockMode.None;
    }

    public static void GameRestart()
    {
        Time.timeScale = 1f;

        LoadingManager.LoadScene("02-Game");
    }
}