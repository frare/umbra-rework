using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    public static int layer = 12;



    public void Collect()
    {
        LevelManager.PageCollected();

        gameObject.SetActive(false);
    }
}