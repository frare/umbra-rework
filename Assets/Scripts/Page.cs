using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
    public static int layer = 12;

    [SerializeField] private string _content;
    public string content { get { return _content; } private set { _content = value; } }



    public void Collect()
    {
        LevelManager.PageCollected();

        gameObject.SetActive(false);
    }
}