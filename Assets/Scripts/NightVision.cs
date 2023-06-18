using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightVision : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float cooldown;
    [SerializeField, ColorUsage(true, true)] private Color boostedColor;
    [SerializeField, Range(0f, 50f)] private int boostedIntensity;

    private bool isActive;
    private bool inCooldown;
    private Color defaultColor;
    private float defaultIntensity;
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
        RenderSettings.ambientIntensity = enabled ? boostedIntensity : defaultIntensity;
        RenderSettings.ambientLight = enabled ? boostedColor : defaultColor;
        RenderSettings.fogEndDistance = enabled ? 50f : 25f;

        if (enabled) StartCoroutine(AutoDisable());
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

        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        inCooldown = true;

        float time = cooldown;
        float normalizedTime = 1f;
        while (time >= 0f)
        {
            time -= Time.deltaTime;
            normalizedTime = time / cooldown;

            UIManager.UpdateVisorCooldown(normalizedTime);

            yield return null;
        }

        inCooldown = false;
    }
}