using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : UnityEngine.Object
{
    private static T _instance;
    protected static T instance 
    { 
        get 
        { 
            return Instance();
        } 
    }

    private static T Instance()
    {
        if (_instance == null) _instance = FindObjectOfType<T>();
        return _instance;
    }
}