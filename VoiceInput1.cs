using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.CognitiveServices.Speech;
using TMPro;
using Microsoft.CognitiveServices.Speech.Audio;

public class VoiceInput1 : MonoBehaviour
{


    //public Text outputText;
    public TextMeshProUGUI outputText;
    public Button startRecordButton;

    // PULLED OUT OF BUTTON CLICK
    SpeechRecognizer recognizer;
    public static SpeechConfig config;

    private object threadLocker = new object();
    private bool speechStarted = false; //checking to see if you've started listening for speech
    private string message;

    private bool micPermissionGranted = false;

    private void RecognizingHandler(object sender, SpeechRecognitionEventArgs e)
    {
        lock (threadLocker)
        {
            message = e.Result.Text;
            Debug.Log("Message is check" + message);
            outputText.text = message;
        }
    }
    
    public async void ButtonClick()
    {

        Debug.Log("button clicked check");
        
        if (speechStarted)
        {
            
            await recognizer.StopContinuousRecognitionAsync().ConfigureAwait(false); // this stops the listening when you click the button, if it's already on
            lock (threadLocker)
            {
                speechStarted = false;
            }
        }
        else
        {
            await recognizer.StartContinuousRecognitionAsync().ConfigureAwait(false); // this will start the listening when you click the button, if it's already off
            lock (threadLocker)
            {
                speechStarted = true;
            }
        }

    }

    
    async static void InvokeSpeechtoText()
    {
        
        using var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        using var speechRecognizer = new SpeechRecognizer(config, audioConfig);

        Debug.Log("Speak into your microphone.");
        var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
        OutputSpeechRecognitionResult(speechRecognitionResult);
        Debug.Log("Start finished");
    }

    async static void InvokeTexttoSpeech(string text)
    {
        

        // The language of the voice that speaks.
        //config.SpeechSynthesisVoiceName = "en-US-JennyNeural";
        config.SpeechSynthesisVoiceName = "en-GB-HollieNeural"

        using (var speechSynthesizer = new SpeechSynthesizer(config))
        {
            // Get text from the console and synthesize to the default speaker.
            Debug.Log("Enter some text that you want to speak >");
            

            var speechSynthesisResult = await speechSynthesizer.SpeakTextAsync(text);
            OutputSpeechSynthesisResult(speechSynthesisResult, text);
        }


    }

    static void OutputSpeechSynthesisResult(SpeechSynthesisResult speechSynthesisResult, string text)
    {
        switch (speechSynthesisResult.Reason)
        {
            case ResultReason.SynthesizingAudioCompleted:
                Debug.Log($"Speech synthesized for text: [{text}]");
                break;
            case ResultReason.Canceled:
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechSynthesisResult);
                Debug.Log($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Debug.Log($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    Debug.Log($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
            default:
                break;
        }
    }

    static void OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
    {
        switch (speechRecognitionResult.Reason)
        {
            case ResultReason.RecognizedSpeech:
                Debug.Log($"RECOGNIZED: Text={speechRecognitionResult.Text}");
                break;
            case ResultReason.NoMatch:
                Debug.Log($"NOMATCH: Speech could not be recognized.");
                break;
            case ResultReason.Canceled:
                var cancellation = CancellationDetails.FromResult(speechRecognitionResult);
                Debug.Log($"CANCELED: Reason={cancellation.Reason}");

                if (cancellation.Reason == CancellationReason.Error)
                {
                    Debug.Log($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                    Debug.Log($"CANCELED: ErrorDetails={cancellation.ErrorDetails}");
                    Debug.Log($"CANCELED: Did you set the speech resource key and region values?");
                }
                break;
        }
    }

    void Start()
    {
        outputText.text = "START";
        startRecordButton.onClick.AddListener(ButtonClick);
        //config = SpeechConfig.FromSubscription("202cf995a7444ff688cac426e2354aa7", "eastus");
        config = SpeechConfig.FromSubscription("f4734b50ee64422eb2d3e9bdc2a21fba", "eastus");
        
        config.SpeechRecognitionLanguage = "en-US";

        InvokeSpeechtoText();
        InvokeTexttoSpeech("Check the voice - Test Test");

        recognizer = new SpeechRecognizer(config);
        recognizer.Recognizing += RecognizingHandler;
        Debug.Log("Start finished");
    }

    void Update()
    {

        lock (threadLocker)
        {
            

            if (outputText != null)
            {
                outputText.text = message;
            }
        }
    }
    /*
    // The microphone object - Prior trial 
    
    private SpeechRecognizer microphone;

    // The callback function that is called when a phrase is recognized
    private void OnPhraseRecognized(PhraseRecognizedEventArgs e)
    {
        // Do something with the recognized phrase

    }

    // Start is called when the game starts or when a scene is loaded
    void Start()
    {
        // Create a new microphone object
        microphone = new SpeechRecognizer(this);

        // Set the callback function
        microphone.OnPhraseRecognized += OnPhraseRecognized;

        // Start listening for voice input
        microphone.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if any phrases have been recognized
        if (microphone.HasPhrasesRecognized)
        {
            // Get the first recognized phrase
            PhraseRecognizedEventArgs e = microphone.GetPhrasesRecognized()[0];

            // Do something with the recognized phrase
        }
    }*/
}


