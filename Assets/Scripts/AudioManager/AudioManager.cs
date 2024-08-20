using UnityEngine.Audio;
using System;
using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{

	public static AudioManager instance;

    
	public AudioMixerGroup mixerGroup;

	public Sound[] sounds;
    public float fadeInTime;
    public float fadeOutTime;

    void Awake()
	{
        

        foreach (Sound s in sounds)
		{
			s.source = gameObject.AddComponent<AudioSource>();
            s.source.volume = s.volume;
            s.source.clip = s.clip;
			s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;

            s.source.outputAudioMixerGroup = s.mixerGroup;
		}
	}
    private void Start()
    {
        Play("MainTheme");
    }

    public AudioSource Find(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return null;
        }
        return s.source;
    }
    public void Play(string sound)
	{
		Sound s = Array.Find(sounds, item => item.name == sound);
		if (s == null)
		{
			Debug.LogWarning("Sound: " + name + " not found!");
			return;
		}

		s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
		s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

		s.source.Play();
	}

    public void Stop(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        s.source.volume = s.volume * (1f + UnityEngine.Random.Range(-s.volumeVariance / 2f, s.volumeVariance / 2f));
        s.source.pitch = s.pitch * (1f + UnityEngine.Random.Range(-s.pitchVariance / 2f, s.pitchVariance / 2f));

        s.source.Stop();
    }
    public void Change(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        //s.source.clip
        

        s.source.Stop();
    }
    public void FadeIn(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        StartCoroutine(FadeInIE(s.source, fadeInTime));
    }

    public void FadeOut(string sound)
    {
        Sound s = Array.Find(sounds, item => item.name == sound);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }

        StartCoroutine(FadeOutIE(s.source,fadeOutTime)); 
    }

    IEnumerator FadeInIE(AudioSource audioSource, float FadeTime)
    {
        audioSource.Play();
        float maxVolume = audioSource.volume;
        audioSource.volume = 0;

        while (audioSource.volume < maxVolume)
        {
            audioSource.volume += maxVolume * Time.deltaTime / FadeTime;

            yield return null;
        }



        audioSource.volume = maxVolume;
    }

    IEnumerator FadeOutIE(AudioSource audioSource, float FadeTime)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

            yield return null;
        }

        audioSource.Stop();
        
        audioSource.volume = startVolume;
    }

    

}
