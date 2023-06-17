using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [SerializeField, ReadOnly] private int pagesTotalCount;
    [SerializeField, ReadOnly] private int pagesCollectedCount;



    private void Awake()
    {
        instance = this;

        pagesTotalCount = FindObjectsOfType<Page>().Length;
    }

    private void OnValidate()
    {
        pagesTotalCount = FindObjectsOfType<Page>().Length;
    }

    public static void PageCollected()
    {
        instance.pagesCollectedCount++;

        UIManager.UpdatePagesCount(instance.pagesCollectedCount, instance.pagesTotalCount);

        if (instance.pagesCollectedCount >= instance.pagesTotalCount)
            Debug.Log("All pages collected");
    }
}