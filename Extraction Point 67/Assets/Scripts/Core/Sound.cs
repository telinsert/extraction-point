

using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;

    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 0.75f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    [Tooltip("0.0 for 2D sound, 1.0 for 3D sound.")]
    [Range(0f, 1f)]
    public float spatialBlend = 0f;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
