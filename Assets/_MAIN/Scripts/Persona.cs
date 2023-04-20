using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Streamer), typeof(AudioSource))]
public class Persona : MonoBehaviour
{
    [SerializeField] private string _preprompt;
    [SerializeField] private AudioSource _audioSource;

    public void RespondTo(string message)
    {
        StartCoroutine(impl_RespondTo(message));
    }

    private IEnumerator impl_RespondTo(string message)
    {
        yield break;

        // TODO: send message to ChatGPT

        // TODO: send text response from ChatGPT to Azure TTS

        // TODO: play audio from Azure TTS

        // TODO: play visemes from Azure TTS
    }

    public void Speak(AudioClip audio, Streamer.Viseme[] visemes)
    {

    }
}
