using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightVision : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float cooldown;
    [SerializeField, ColorUsage(true, true)] private Color boostedColor;

    private bool isActive;
    private bool inCooldown;
    private Color defaultColor;
    private UnityEngine.Rendering.Volume volume;



    private void Awake()
    {
        defaultColor = RenderSettings.ambientLight;
        volume = GetComponent<UnityEngine.Rendering.Volume>();
    }

    private void Start()
    {
        SetEnabled(false);
    }

    private void SetEnabled(bool enabled)
    {
        isActive = enabled;
        volume.weight = enabled ? 1 : 0;
        RenderSettings.ambientLight = enabled ? boostedColor : defaultColor;

        if (enabled)
        {
            StartCoroutine(AutoDisable());
            StartCoroutine(Cooldown());
        }
    }

    public void TryToEnable()
    {
        if (isActive || inCooldown) return;

        SetEnabled(true);
    }

    private IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(duration);

        SetEnabled(false);
    }

    private IEnumerator Cooldown()
    {
        inCooldown = true;

        yield return new WaitForSeconds(cooldown);

        inCooldown = false;
    }
}