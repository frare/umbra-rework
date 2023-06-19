using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoFogCamera : MonoBehaviour
{
    private void OnPreRender() 
    {
        RenderSettings.fog = false;
    }

    private void OnPostRender() 
    {
        RenderSettings.fog = true;
    }
}