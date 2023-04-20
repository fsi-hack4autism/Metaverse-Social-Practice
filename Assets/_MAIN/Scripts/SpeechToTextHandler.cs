using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class SpeechToTextHandler : MonoBehaviour
{
    [SerializeField] private string Url;
    [SerializeField] private string Key;
    public UnityEvent OnText = new UnityEvent();

    public void Convert(AudioClip clip)
    {
        byte[] wav = WavUtility.FromAudioClip(clip);
        System.IO.File.WriteAllBytes(Application.dataPath + "/out.wav", wav);
    }
}
