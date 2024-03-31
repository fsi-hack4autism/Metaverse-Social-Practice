using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using UnityEngine.Events;

public class AzureInterface : MonoBehaviour
{
    public int AudioSampleRate = 16000;

    public UnityEvent<string> OnAudioTranscribed = new UnityEvent<string>();
    public UnityEvent<string> OnChatMessageReceived = new UnityEvent<string>();
    public UnityEvent<VisemedAudio> OnAudioReceived = new UnityEvent<VisemedAudio>();
    public UnityEvent<string> OnAssessmentReceived = new UnityEvent<string>();

    // ==========	THREADING QUEUES	==========
    // Unity isn't thread safe, so we need to use a queue
    // to handle events and callbacks from the speech service

    private Queue<System.Action> _callbackQueue = new Queue<System.Action>();
    private object _callbackQueueLock = new object();

    private Queue<VisemedAudio> _visemedAudioQueue = new Queue<VisemedAudio>();
    private object _visemedAudioQueueLock = new object();

    private string _transcription = "";
    private string _assessment = "";


    // ==========	INPUT HANDLING	==========

    private MicrophoneControls input = null;

    private void Awake()
    {
        input = new MicrophoneControls();
    }


    /* ===== INPUT ===== */

    private void OnEnable()
    {
        input.Enable();
        input.Microphone.Record.performed += Record_performed;
    }

    private void OnDisable()
    {
        input.Disable();
        input.Microphone.Record.performed -= Record_performed;
    }

    private void Record_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        bool value = obj.ReadValueAsButton();
        Debug.Log(value);
        if (value)
        {
            SpeechToText();
        }
        else
        {
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_visemedAudioQueue.Count > 0)
        {
            lock (_visemedAudioQueueLock)
            {
                OnAudioReceived.Invoke(_visemedAudioQueue.Dequeue());
            }
        }

        if (_callbackQueue.Count > 0)
        {
            lock (_callbackQueueLock)
            {
                _callbackQueue.Dequeue().Invoke();
            }
        }

        if (_transcription.Length > 0)
        {
            OnAudioTranscribed.Invoke(_transcription);
            _transcription = "";
        }

        if (_assessment.Length > 0)
        {
            OnAssessmentReceived.Invoke(_assessment);
            _assessment = "";
        }
    }

    public async void TextToSpeech(string text, string voice = "en-US-JennyNeural", System.Action<VisemedAudio> callback = null)
    {
        var stream = Microsoft.CognitiveServices.Speech.Audio.AudioOutputStream.CreatePullStream();
        var speechConfig = SpeechConfig.FromSubscription(env.Get("SPEECH_KEY"), env.Get("SPEECH_REGION"));
        var audioConfig = AudioConfig.FromStreamOutput(stream);

        // The language of the voice that speaks.
        speechConfig.SpeechSynthesisVoiceName = voice;

        using (var speechSynthesizer = new SpeechSynthesizer(speechConfig, audioConfig))
        {
            string visemesFromAzure = "";
            byte[] audioFromAzure = new byte[0];

            speechSynthesizer.VisemeReceived += (s, e) =>
            {
                visemesFromAzure += $"time: {e.AudioOffset / 10000}ms, viseme id: {e.VisemeId}.\n";
            };
            speechSynthesizer.SynthesisCompleted += (s, e) =>
            {
                audioFromAzure = e.Result.AudioData;
            };

            // Perform synthesis.
            SpeechSynthesisResult speechResult = await speechSynthesizer.SpeakTextAsync(text);
            stream.Dispose();

            // convert to visemed audio and add to queue
            VisemedAudio visemedAudio = new VisemedAudio();
            visemedAudio.LoadAudio(audioFromAzure, 1, AudioSampleRate);
            visemedAudio.LoadRawVisemes(visemesFromAzure);
            visemedAudio.audio.name = text;
            lock (_visemedAudioQueueLock)
            {
                _visemedAudioQueue.Enqueue(visemedAudio);
            }

            // invoke callback
            if (callback != null)
            {
                lock (_callbackQueueLock)
                {
                    _callbackQueue.Enqueue(() => callback.Invoke(visemedAudio));
                }
            }
        }
    }

    public async void SpeechToText()
    {
        var speechConfig = SpeechConfig.FromSubscription(env.Get("SPEECH_KEY"), env.Get("SPEECH_REGION"));
        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

        using (var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig))
        {
            var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
            _transcription = speechRecognitionResult.Text;
        }
    }

    public void SendToChatbot(string userInput, Persona.ConversationComment[] conversation)
    {
        StartCoroutine(impl_SendToChatbot(userInput, conversation));
    }

    public IEnumerator impl_SendToChatbot(string userInput, Persona.ConversationComment[] conversation)
    {
        UnityWebRequest req = new UnityWebRequest(env.Get("OPENAI_URL"), "POST");
        req.SetRequestHeader("Authorization", $"Bearer {env.Get("OPENAI_KEY")}");
        req.SetRequestHeader("Content-Type", "application/json");

        // build body
        OpenAIBody body = new OpenAIBody();
        body.model = "gpt-3.5-turbo";
        body.messages = conversation;
        string json = JsonUtility.ToJson(body);
        Debug.Log($"[AzureInterface] ({body.model}) Sending: {json}");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        // send request
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        yield return req.SendWebRequest();

        // handle response
        if (req.responseCode == 200)
        {
            string response = req.downloadHandler.text;
            Debug.Log(response);
            OpenAIResponse openAIResponse = JsonUtility.FromJson<OpenAIResponse>(response);
            OnChatMessageReceived.Invoke(openAIResponse.choices[0].message.content); // invoke event
        }
        else
        {
            Debug.Log(req.error);
        }
    }

    public async void Assess(string input)
    {
        // return 0.5f;
    }

    [System.Serializable]
    public struct OpenAIBody
    {
        public string model;
        public Persona.ConversationComment[] messages;
    }

    [System.Serializable]
    public struct OpenAIResponse
    {
        public string id;
        public long created;
        public OpenAIChoice[] choices;
    }

    [System.Serializable]
    public struct OpenAIChoice
    {
        public int index;
        public Persona.ConversationComment message;
        public string finish_reason;
    }
}

