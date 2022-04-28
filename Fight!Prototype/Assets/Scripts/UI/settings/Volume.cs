using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    public AudioMixer audioMixer;
    



    public void Start()
    {
        
        audioMixer.GetFloat("Master", out float volume);

    }


    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Master", volume);
    }
}

