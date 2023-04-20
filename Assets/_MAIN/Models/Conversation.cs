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
    // This example requires environment variables named "SPEECH_KEY" and "SPEECH_REGION"
    const string speechKey = Environment.GetEnvironmentVariable("SPEECH_KEY");
    const string speechRegion = Environment.GetEnvironmentVariable("SPEECH_REGION");

    public UnityEvent<(byte[], string)> OnAudioReceived = new UnityEvent<(byte[], string)>();

    private byte[] returnFromAzure = new byte[0];
    private string visemesFromAzure = "";

    // Start is called before the first frame update
    void Start()
    {
        TextToSpeech("hello there");
    }

    // Update is called once per frame
    void Update()
    {
        if (returnFromAzure.Length > 0 && !visemesFromAzure.Equals(""))
        {
            OnAudioReceived.Invoke((returnFromAzure, visemesFromAzure));
            returnFromAzure = new byte[0];
            visemesFromAzure = "";
        }

    }

    async void TextToSpeech(string text)
    {
        var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);

        // The language of the voice that speaks.
        speechConfig.SpeechSynthesisVoiceName = "en-US-JennyNeural";

        using (var speechSynthesizer = new SpeechSynthesizer(speechConfig))
        {
            speechSynthesizer.VisemeReceived += (s, e) =>
            {
                visemesFromAzure = e.Animation;
            };

            // Get text from the console and synthesize to the default speaker.
            SpeechSynthesisResult speechResult = await speechSynthesizer.SpeakTextAsync(text);
            returnFromAzure = speechResult.AudioData;
        }
    }

    string SpeechToText(byte[] speech)
    {
        return "hi";
    }

    string ChatUp(string userInput)
    {
        return "what's your name?";
    }

    float grade(string input)
    {
        return 0.5F;
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
        yield return unityWebRequest.Send();

        if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
        {
            Debug.Log(unityWebRequest.error);
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

