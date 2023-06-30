using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] clips;

    private AudioSource source;
    private float sourceVolume;
    private bool fading;





    private void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = clips[0];
        sourceVolume = source.volume;
    }

    private void Start()
    {
        source.Play();
    }

    private void FixedUpdate()
    {
        if (source.time > source.clip.length - 5f && fading == false)
            ToggleClip();
    }

    private void ToggleClip()
    {
        fading = true;
        StartCoroutine(FadeOut(source, 4f));
    }

    private IEnumerator FadeOut(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;
 
        while (audioSource.volume > 0)
        {
            audioSource.volume -= sourceVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioSource.clip = source.clip == clips[0] ? clips[1] : clips[0];
        audioSource.volume = sourceVolume;

        yield return new WaitForSeconds(1f);
        StartCoroutine(FadeIn(source, 4f));
    }
 
    private IEnumerator FadeIn(AudioSource audioSource, float FadeTime)
    {
        float startVolume = 0.2f;
 
        audioSource.volume = 0;
        audioSource.Play();
 
        while (audioSource.volume < sourceVolume)
        {
            audioSource.volume += startVolume * Time.deltaTime / FadeTime;
 
            yield return null;
        }
 
        audioSource.volume = sourceVolume;
        fading = false;
    }
}