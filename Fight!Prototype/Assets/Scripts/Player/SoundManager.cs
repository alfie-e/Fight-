using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]

public class SoundManager : MonoBehaviour
{

    public AudioSource sound;

    // Start is called before the first frame update
    void Start()
    {
        sound = this.GetComponent<AudioSource>();
    }

    //plays sound from called function

    void Step(float volume = 1f)
    {
        sound.volume = volume;
        sound.time = 0.02f;
        sound.pitch = Random.Range(0.85f, 1.15f);
        sound.PlayOneShot((AudioClip)Resources.Load("Audio/Footsteps/Footstep00"));
    }

    void Swing()
    {
        sound.volume = 0.1f;
        sound.time = 0.0f;
        sound.pitch = Random.Range(0.85f, 1.15f);
        sound.PlayOneShot((AudioClip)Resources.Load("Audio/Weapon/Swing1"));
    }

    void Hit(string audiosauce)
    {
        sound.volume = 0.1f;
        sound.time = 0.0f;
        sound.pitch = Random.Range(0.9f, 1.1f);
        sound.PlayOneShot((AudioClip)Resources.Load(audiosauce));
    }
}
