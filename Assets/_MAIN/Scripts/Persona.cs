using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Streamer), typeof(AudioSource))]
public class Persona : MonoBehaviour
{
    [SerializeField] private string _preprompt;
    public string Greeting = "Hello, I'm {name}";
    private List<ConversationComment> Conversation;


    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AzureInterface _azure;

    public GameObject Results;

    public UnityEvent OnApproach = new UnityEvent();
    public UnityEvent OnExit = new UnityEvent();

    public void RespondTo(string message)
    {
        // add user comment to conversation
        this.Conversation.Add(new ConversationComment()
        {
            speaker = ConversationComment.Speaker.User,
            persona = this,
            text = message
        });

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

    public void Speak(VisemedAudio audio)
    {
        Speak(audio.audio, audio.visemes);
    }

    public void Speak(AudioClip audio, Streamer.Viseme[] visemes)
    {
        Debug.Log($"[Persona] Speaking {audio.name}");
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
        // reset conversation
        this.Conversation = new List<ConversationComment>();

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

        // initiate conversation
        _azure.TextToSpeech(Greeting.Replace("{name}", this.name));
        this.Conversation.Add(new ConversationComment()
        {
            speaker = ConversationComment.Speaker.Persona,
            persona = this,
            text = Greeting.Replace("{name}", this.name)
        });
    }

    private IEnumerator EndConversation()
    {
        Results.SetActive(true);
        yield return null;
    }


    /// <summary>
    /// A comment made by a user/persona in a conversation
    /// </summary>
    [System.Serializable]
    public struct ConversationComment
    {
        public enum Speaker
        {
            User,
            Persona
        }
        public Speaker speaker;
        public Persona persona;
        public string text;
    }
}
