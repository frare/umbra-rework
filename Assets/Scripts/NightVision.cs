using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class NightVision : MonoBehaviour
{
    [SerializeField] private float duration;
    [SerializeField] private float cooldown;
    [SerializeField] private float stunDuration;
    [SerializeField, ColorUsage(true, true)] private Color boostedColor;
    [SerializeField, Range(0f, 50f)] private int boostedIntensity;

    private bool isActive;
    private bool inCooldown;
    private Color defaultColor;
    private float defaultIntensity;
    private Transform mainCamera;
    private Volume volume;
    private Vignette vignette;
    private ColorAdjustments colorAdjustments;
    private FilmGrain filmGrain;

    public delegate void OnEnemyCaught(float duration);
    public event OnEnemyCaught onEnemyCaught;



    private void Awake()
    {
        defaultColor = RenderSettings.ambientLight;
        volume = GetComponent<Volume>();
        volume.profile.TryGet<Vignette>(out vignette);
        volume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
        volume.profile.TryGet<FilmGrain>(out filmGrain);
        mainCamera = Camera.main.transform;
    }

    private void Start()
    {
        SetEnabled(false);
    }

    private void Update()
    {
        if (isActive)
        {
            RaycastHit hit;
            if (Physics.SphereCast(mainCamera.position, .5f, Enemy.position - mainCamera.position, out hit, 50f, 
                1 << EnemyManager.layer | 1 << 7, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.gameObject.layer == EnemyManager.layer) onEnemyCaught?.Invoke(stunDuration);
            }
        }
    }

    private void SetEnabled(bool enabled)
    {
        isActive = enabled;
        volume.enabled = enabled;
        vignette.smoothness = new ClampedFloatParameter(.3f, 0f, 1f, true);
        colorAdjustments.colorFilter = new ColorParameter(Color.green, true);
        filmGrain.intensity = new ClampedFloatParameter(.5f, 0f, 1f, true);
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
        yield return new WaitForSeconds(duration - .5f);

        float time = 0f;
        float normalizedTime = 0f;
        while (time < .5f)
        {
            time += Time.deltaTime;
            normalizedTime = time / .5f;

            vignette.intensity = new ClampedFloatParameter(Mathf.Lerp(.5f, 0f, normalizedTime), 0f, 1f, true);
            colorAdjustments.colorFilter = new ColorParameter(Color.Lerp(Color.green, Color.white, normalizedTime), true);
            filmGrain.intensity = new ClampedFloatParameter(Mathf.Lerp(.5f, 0f, normalizedTime), 0f, 1f, true);
            RenderSettings.ambientIntensity = Mathf.Lerp(boostedIntensity, defaultIntensity, normalizedTime);
            RenderSettings.fogEndDistance = Mathf.Lerp(50f, 25f, normalizedTime);

            yield return null;
        }

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