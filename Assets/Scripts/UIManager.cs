using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public void OnPressed_Exit()
    {
        LoadingManager.LoadScene("01-Menu");
    }
}