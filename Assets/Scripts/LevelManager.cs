using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public static int pagesCollected { get { return instance.pagesCollectedCount; } }

    [SerializeField, ReadOnly] private int pagesTotalCount;
    [SerializeField, ReadOnly] private int pagesCollectedCount;

    public delegate void OnPageCollected();
    public static event OnPageCollected onPageCollected;



    private void Awake()
    {
        instance = this;

        List<Page> pages = new List<Page>(FindObjectsOfType<Page>());
        while (pages.Count > 6) pages.RemoveAt(Random.Range(0, pages.Count - 1));
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
}