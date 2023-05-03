using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightVisionManager : MonoBehaviour
{
    public static NightVisionManager instance;

    [SerializeField] private bool isActive;
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color boostedColor;

    private UnityEngine.Rendering.Volume volume;



    private void Awake()
    {
        if (NightVisionManager.instance == null) NightVisionManager.instance = this;
        else Destroy(this.gameObject);

        volume = GetComponent<UnityEngine.Rendering.Volume>();
    }

    private void Start()
    {
        SetEnabled(false);
    }

    public static void SetEnabled(bool enabled)
    {
        instance.isActive = enabled;
        instance.volume.weight = enabled ? 1 : 0;
        RenderSettings.ambientLight = enabled ? instance.boostedColor : instance.defaultColor;
    }

    public static void Toggle()
    {
        SetEnabled(!instance.isActive);
    }
}