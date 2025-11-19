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
    public AudioSource sfxSource2D; // For UI

    [Header("Performance Settings")]
    public int poolSize = 40; // How many 3D sounds can play at once?

    private Dictionary<string, Sound> soundDictionary;
    private List<AudioSource> sfxPool; // The pool of reusable sources

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
            source.spatialBlend = 1f; // Default to 3D
            source.rolloffMode = AudioRolloffMode.Linear; // Cheaper than Logarithmic
            source.minDistance = 2f;
            source.maxDistance = 30f; // Sounds stop being heard after 30 units

            obj.SetActive(false); // Start disabled
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

    // --- OPTIMIZED 3D SOUND ---
    public void PlaySFXAtPosition(string name, Vector3 position)
    {
        if (!soundDictionary.ContainsKey(name)) return;
        Sound s = soundDictionary[name];

        // 1. Find a free AudioSource in the pool
        AudioSource source = GetFreeSource();

        if (source != null)
        {
            // 2. Move it to the position
            source.transform.position = position;
            source.gameObject.SetActive(true);

            // 3. Configure it
            source.clip = s.clip;
            source.volume = s.volume;
            source.pitch = s.pitch * UnityEngine.Random.Range(0.9f, 1.1f);
            source.spatialBlend = s.spatialBlend; // 1.0 for 3D, 0.0 for 2D

            // 4. Play
            source.Play();

            // 5. Start a coroutine to disable it when done (returning it to pool)
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
        // If all are busy, we just don't play the sound (better than lagging)
        // Or you could temporarily expand the pool here if strictly necessary.
        return null;
    }

    private System.Collections.IEnumerator DisableSourceAfterTime(AudioSource source, float time)
    {
        yield return new WaitForSeconds(time);
        source.Stop();
        source.gameObject.SetActive(false);
    }
}
