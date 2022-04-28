using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Volume : MonoBehaviour
{
    public AudioMixer audioMixer;

 
   

    public void Start()
    {
        audioMixer.GetFloat("Master", 0);

    }


    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Master", volume);
    }
}

