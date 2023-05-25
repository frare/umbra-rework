using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private Image visorCooldown;



    private void Awake()
    {
        instance = this;

        visorCooldown.fillAmount = 0f;
    }

    public static void UpdateVisorCooldown(float normalizedTime)
    {
        instance.visorCooldown.fillAmount = normalizedTime;
    }

    public void OnPressed_Exit()
    {
        LoadingManager.LoadScene("01-Menu");
    }
}