using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public void OnPressed_Start()
    {
        LoadingManager.LoadScene("02-Game");
    }

    public void OnPressed_Options()
    {

    }

    public void OnPressed_Exit()
    {
        Application.Quit();
    }

    public void OnPressed_Fullscreen()
    {
        // Screen.fullScreen = !Screen.fullScreen;
        if (Screen.fullScreen) Screen.SetResolution(800, 600, FullScreenMode.Windowed);
        else Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow);
    }
}