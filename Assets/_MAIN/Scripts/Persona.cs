using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Streamer), typeof(AudioSource))]
public class Persona : MonoBehaviour
{
    [SerializeField] private string _preprompt;
    [SerializeField] private AudioSource _audioSource;

    public WozHandler.Response Greeting;
    public GameObject Results;

    public UnityEvent OnApproach = new UnityEvent();
    public UnityEvent OnExit = new UnityEvent();

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
        var audioSource = GetComponent<AudioSource>();
        audioSource.clip = audio;
        audioSource.Play();
        GetComponent<Streamer>().PlayVisemes(visemes);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            StartCoroutine(BeginConversation(other.transform));
            OnApproach.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            StartCoroutine(EndConversation());
            OnExit.Invoke();
        }
    }

    private IEnumerator BeginConversation(Transform player)
    {
        // face user
        float duration = 1f;
        float startTime = Time.time;
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position, Vector3.up);
        while (Time.time - startTime < duration)
        {
            float t = (Time.time - startTime) / duration;
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            yield return new WaitForFixedUpdate();
        }


        // play audio
        Greeting.LoadVisemeFile();
        Speak(Greeting.audio, Greeting.visemes);
    }

    private IEnumerator EndConversation()
    {
        Results.SetActive(true);
        yield return null;
    }
}
