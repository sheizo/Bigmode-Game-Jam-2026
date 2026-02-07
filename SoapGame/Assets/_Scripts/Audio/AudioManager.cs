using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using System;
using System.Linq;

public class AudioManager : Singleton<AudioManager>
{
    [Range(0f, 1f)]
    public float MusicVolume = 1f;

    [Range(0f, 1f)]
    public float SFXVolume = 1f;

    public Sound[] soundEffects;
    public Sound[] music;

    private Sound[] sounds;
    
    private string _currentMusic = " ";

    protected override void Awake()
    {
        base.Awake();

        DontDestroyOnLoad(gameObject);

        sounds = soundEffects.Concat(music).ToArray();

        foreach (Sound s in sounds) //grabs each sound and changes them accordingly on awake
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip[0];

            s.source.outputAudioMixerGroup = s.audioGroup;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }


    public void Play(string name)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        if (s == null)
        {
            print(s + " sound not found");
            return;
        }

        s.source.Play();
    }

    public void PlayOneShot(string name)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        if (s == null)
        {
            print(s + " sound not found");
            return;
        }


        s.source.PlayOneShot(s.clip[UnityEngine.Random.Range(0, s.clip.Length)]);
    }

    public void PlayOneShot(string name, Vector2 pitchBounds)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        if (s == null)
        {
            print(s + " sound not found");
            return;
        }
        s.source.pitch = UnityEngine.Random.Range(pitchBounds.x, pitchBounds.y);

        s.source.PlayOneShot(s.clip[UnityEngine.Random.Range(0, s.clip.Length)]);

        s.source.volume = s.volume;
    }
    
    public void PlayOneShot(string name, float soundScale)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        if (s == null)
        {
            print(s + " sound not found");
            return;
        }


        s.source.PlayOneShot(s.clip[UnityEngine.Random.Range(0, s.clip.Length)], soundScale);
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        if (s == null)
        {
            print(s + " sound not found");
            return;
        }

        s.source.Stop();
    }

    public void Stop(string name, float fadeDuration)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        if (s == null)
        {
            print(s + " sound not found");
            return;
        }

        StartCoroutine(FadeOutAndStop(s, fadeDuration));
    }

    private IEnumerator FadeOutAndStop(Sound s, float fadeDuration)
    {
        float startVolume = s.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            s.source.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }


        s.source.Stop();
    }


    public void ToggleMute(string name, bool pause)
    {
        Sound s = Array.Find(sounds, s => s.name == name);

        if (s == null)
        {
            print(s + " sound not found");
            return;
        }

        

        if (pause == true) s.source.volume = 0f;
        else s.source.volume = s.volume;
    }

    public void PlayMusic(string music)
    {
        if (_currentMusic != " ")
        {
            Stop(_currentMusic);
        }
        Play(music);
        _currentMusic = music;
    }

    
}