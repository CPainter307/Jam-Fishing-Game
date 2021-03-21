using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuSounds : MonoBehaviour
{
    public AudioClip cycleSound;
    public AudioClip backSound;
    public AudioClip selectSound;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Vertical"))
        {
            PlaySound(0);
            GetComponent<AudioSource>().Play();
        }
    }
    public void PlaySound(int soundNumber)
    {
        AudioSource audio = gameObject.GetComponent<AudioSource>();
        switch (soundNumber)
        {
            case 0:
                audio.clip = cycleSound;
                break;
            case 1:
                audio.clip = backSound;
                break;
            case 2:
                audio.clip = selectSound;
                break;
            default:
                print("ERROR: Invalid sound #");
                break;
        }
    }
}
