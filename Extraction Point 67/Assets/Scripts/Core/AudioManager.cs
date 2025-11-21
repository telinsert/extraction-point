using UnityEngine;
using System;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Collection")]
    public Sound[] sounds;

    [Header("Settings")]
    public AudioSource musicSource;
    public AudioSource sfxSource2D; 

    [Header("Performance Settings")]
    public int poolSize = 40;

    private Dictionary<string, Sound> soundDictionary;
    private List<AudioSource> sfxPool; 

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 1. Initialize Dictionary
        soundDictionary = new Dictionary<string, Sound>();
        foreach (Sound s in sounds)
        {
            if (!soundDictionary.ContainsKey(s.name))
            {
                soundDictionary.Add(s.name, s);
            }
        }

        // 2. Create the Pool
        sfxPool = new List<AudioSource>();
        GameObject poolHolder = new GameObject("SFX_Pool");
        poolHolder.transform.SetParent(this.transform);

        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = new GameObject("PooledAudioSource_" + i);
            obj.transform.SetParent(poolHolder.transform);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 1f; 
            source.rolloffMode = AudioRolloffMode.Linear; 
            source.minDistance = 2f;
            source.maxDistance = 30f; 

            obj.SetActive(false); 
            sfxPool.Add(source);
        }
    }

    public void PlayMusic(string name)
    {
        if (!soundDictionary.ContainsKey(name)) return;
        Sound s = soundDictionary[name];

        if (musicSource.isPlaying && musicSource.clip == s.clip) return;

        musicSource.clip = s.clip;
        musicSource.volume = s.volume;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        if (!soundDictionary.ContainsKey(name)) return;
        Sound s = soundDictionary[name];

        sfxSource2D.pitch = s.pitch * UnityEngine.Random.Range(0.95f, 1.05f);
        sfxSource2D.PlayOneShot(s.clip, s.volume);
    }

    public void PlaySFXAtPosition(string name, Vector3 position)
    {
        if (!soundDictionary.ContainsKey(name)) return;
        Sound s = soundDictionary[name];

        AudioSource source = GetFreeSource();

        if (source != null)
        {
            source.transform.position = position;
            source.gameObject.SetActive(true);

            source.clip = s.clip;
            source.volume = s.volume;
            source.pitch = s.pitch * UnityEngine.Random.Range(0.9f, 1.1f);
            source.spatialBlend = s.spatialBlend; 

            source.Play();

            StartCoroutine(DisableSourceAfterTime(source, s.clip.length));
        }
    }

    private AudioSource GetFreeSource()
    {
        foreach (AudioSource s in sfxPool)
        {
            if (!s.gameObject.activeInHierarchy)
            {
                return s;
            }
        }
        
        return null;
    }

    private System.Collections.IEnumerator DisableSourceAfterTime(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);
        source.Stop();
        source.gameObject.SetActive(false);
    }
}
