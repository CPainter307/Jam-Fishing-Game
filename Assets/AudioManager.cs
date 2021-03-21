using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip ambience;
    AudioSource source;

    private void Start()
    {
        source = GetComponent<AudioSource>();

        source.clip = ambience;
        source.Play();
    }
}
