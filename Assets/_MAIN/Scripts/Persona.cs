using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Streamer), typeof(AudioSource))]
public class Persona : MonoBehaviour
{
    public string Preprompt = "";
    public string Voice = "en-US-JennyNeural";
    public string Greeting = "Hello, I'm {name}";
    public string Role = "assistant";
    private List<ConversationComment> Conversation;


    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AzureInterface _azure;

    public GameObject Results;

    public UnityEvent OnApproach = new UnityEvent();
    public UnityEvent OnExit = new UnityEvent();

    /// <summary>
    /// Has the persona respond to a comment from the user
    /// </summary>
    /// <param name="message">the comment from the user</param>
    public void RespondTo(string message)
    {
        Debug.Log($"[Persona] ({name}) Responding to \"{message}\"");
        // add user comment to conversation
        this.Conversation.Add(new ConversationComment()
        {
            role = "user",
            persona = this,
            content = message
        });

        GetComponent<AzureInterface>().SendToChatbot(message, this.Conversation.ToArray());
    }

    /// <summary>
    /// Adds a chatbot response to the conversation history
    /// </summary>
    /// <param name="text">the text response from the chatbot</param>
    public void HandleChatbotResponse(string text)
    {
        Debug.Log($"[Persona] ({name}) responding with \"{text}\"");
        // add persona comment to conversation
        this.Conversation.Add(new ConversationComment()
        {
            role = this.Role,
            persona = this,
            content = text
        });

        // convert text to audio
        GetComponent<AzureInterface>().TextToSpeech(text, this.Voice);
    }

    /// <summary>
    /// Makes the chatbot speak and animate
    /// </summary>
    /// <param name="audio">the audio and viseme data to play</param>
    public void Speak(VisemedAudio audio)
    {
        Speak(audio.audio, audio.visemes);
    }

    /// <summary>
    /// Makes the chatbot speak and animate
    /// </summary>
    /// <param name="audio">the audio to play</param>
    /// <param name="visemes">the viseme data (facial animations) to play</param>
    public void Speak(AudioClip audio, Streamer.Viseme[] visemes)
    {
        Debug.Log($"[Persona] ({name}) Speaking \"{audio.name}\"");
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

    /// <summary>
    /// Makes the persona face the user and initiate the conversation
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    private IEnumerator BeginConversation(Transform player)
    {
        Debug.Log($"[Persona] ({name}) Beginning conversation with {player.name}");
        // reset conversation
        this.Conversation = new List<ConversationComment>();
        this.Conversation.Add(new ConversationComment()
        {
            role = "system",
            persona = this,
            content = Preprompt
        });

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
        _azure.TextToSpeech(Greeting.Replace("{name}", this.name), this.Voice);
        this.Conversation.Add(new ConversationComment()
        {
            role = this.Role,
            persona = this,
            content = Greeting.Replace("{name}", this.name)
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
        public string role;
        public Persona persona { get; set; }
        public string content;
    }
}
