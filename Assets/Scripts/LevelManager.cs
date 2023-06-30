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

        List<Collectible> collectibles = new List<Collectible>(FindObjectsOfType<Collectible>());
        while (collectibles.Count > 6) 
        {
            int randomIndex = Random.Range(0, collectibles.Count - 1);
            collectibles[randomIndex].gameObject.SetActive(false);
            collectibles.RemoveAt(randomIndex);
        }
        pagesTotalCount = FindObjectsOfType<Collectible>().Length;
    }

    private void OnValidate()
    {
        pagesTotalCount = FindObjectsOfType<Collectible>().Length;
    }

    public static void PageCollected()
    {
        instance.pagesCollectedCount++;
        onPageCollected?.Invoke();

        UIManager.UpdatePagesCount(instance.pagesCollectedCount, instance.pagesTotalCount);

        if (instance.pagesCollectedCount >= instance.pagesTotalCount)
        {
            GameWin();
        }            
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