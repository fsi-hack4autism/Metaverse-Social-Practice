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

public class Conversation : MonoBehaviour
{
    public int AudioSampleRate = 16000;
    private string speechKey;
    private string speechRegion;

    public UnityEvent<VisemedAudio> OnAudioReceived = new UnityEvent<VisemedAudio>();
    public UnityEvent<string> OnTextReceived = new UnityEvent<string>();
    public UnityEvent<string> OnAssessmentReceived = new UnityEvent<string>();

    // ==========	THREADING QUEUES	==========
    // Unity isn't thread safe, so we need to use a queue
    // to handle events and callbacks from the speech service

    private Queue<System.Action> _callbackQueue = new Queue<System.Action>();
    private object _callbackQueueLock = new object();

    private Queue<VisemedAudio> _visemedAudioQueue = new Queue<VisemedAudio>();
    private object _visemedAudioQueueLock = new object();

    [SerializeField] private string textFromChat = "";
    private string assessment = "";

    // Start is called before the first frame update
    void OnEnable()
    {
        speechKey = env.Get("SPEECH_KEY");
        speechRegion = env.Get("SPEECH_REGION");
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

        if (textFromChat.Length > 0)
        {
            OnTextReceived.Invoke(textFromChat);
            textFromChat = "";
        }

        if (assessment.Length > 0)
        {
            OnAssessmentReceived.Invoke(assessment);
            assessment = "";
        }
    }

    public async void TextToSpeech(string text, string voice = "en-US-JennyNeural", System.Action<VisemedAudio> callback = null)
    {
        var stream = Microsoft.CognitiveServices.Speech.Audio.AudioOutputStream.CreatePullStream();
        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
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

    async void SpeechToText(byte[] speech)
    {
        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);

        // The language of the voice that speaks.
        speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";
        speechConfig.SpeechRecognitionLanguage = "en-US";
        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();

        using (var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig))
        {
            var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
            textFromChat = speechRecognitionResult.Text;
        }
    }

    async void ChatUp(string userInput)
    {
        // return "what's your name?";
    }

    async void Assess(string input)
    {
        // return 0.5f;
    }

    void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult)
    {
        switch (speechSynthesisResult.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                break;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
            default:
                break;
        }
    }


    IEnumerator postUnityWebRequest(string url)
    {
        ///<summary>
        /// Post using UnityWebRequest class
        /// </summary>
        var jsonString = "{\"Id\":3,\"Name\":\"Roy\"}";
        byte[] byteData = System.Text.Encoding.ASCII.GetBytes(jsonString.ToCharArray());

        UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST");
        unityWebRequest.uploadHandler = new UploadHandlerRaw(byteData);
        unityWebRequest.SetRequestHeader("Content-Type", "application/json");
        yield return unityWebRequest.SendWebRequest();

        if (unityWebRequest.responseCode != 200)
        {
            Debug.Log("Error: " + unityWebRequest.responseCode);
        }
        else
        {
            Debug.Log("Form upload complete! Status Code: " + unityWebRequest.responseCode);
        }
    }
    /*
async Task Converse()
{
    var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);

    // The language of the voice that speaks.
    speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";

    using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
    {
        speechSynthesizer.VisemeReceived += (s, e) =>
        {
            Console.WriteLine($"Viseme event received. Audio offset: " + $"{e.AudioOffset / 10000}ms, viseme id: {e.VisemeId}.");
            var animation = e.Animation;
        };


        string text = "";

        while (text != "bye")
        {
            // Get text from the console and synthesize to the default speaker.
            Console.WriteLine("Enter some text that you want to speak >");
            text = Console.ReadLine();

            var speechResult = await speechSynthesizer.SpeakTextAsync(text);
            var visemeResult = await speechSynthesizer.SpeakSsmlAsync(text);
            OutputSpeechSynthesisResult(speechResult, text);
        }
    }

    Console.WriteLine("Press any key to exit...");
    Console.ReadKey();
}
    */
}

