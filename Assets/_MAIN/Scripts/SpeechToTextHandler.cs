using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class SpeechToTextHandler : MonoBehaviour
{
    [SerializeField] private string Url;
    [SerializeField] private string Key;
    public UnityEvent<string> OnText = new UnityEvent<string>();
    public UnityEvent OnError = new UnityEvent();

    public void Convert(AudioClip clip)
    {
        // convert AudioClip to Wav file
        byte[] wav = WavUtility.FromAudioClip(clip);
        // System.IO.File.WriteAllBytes(Application.persistentDataPath + "/stt.wav", wav);

        // Send audio to Azure
        StartCoroutine(AzureStt(wav));
    }

    private IEnumerator AzureStt(byte[] audio)
    {
        /*
        audio_file=@'YourAudioFile.wav'

        curl --location --request POST \
        "https://${SPEECH_REGION}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US" ^
        --header "Ocp-Apim-Subscription-Key: ${SPEECH_KEY}" ^
        --header "Content-Type: audio/wav" ^
        --data-binary $audio_file
        */

        var req = new UnityWebRequest(Url, "POST");
        req.uploadHandler = new UploadHandlerRaw(audio);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Ocp-Apim-Subscription-Key", Key);
        req.SetRequestHeader("Content-Type", "audio/wav");
        yield return req.SendWebRequest();

        if (req.responseCode != 200)
        {
            Debug.LogError(req.error);
            OnError.Invoke();
        }
        else
        {
            Debug.Log("Form upload complete!");
            Debug.Log(req.downloadHandler.text);
            SttResponse res = JsonUtility.FromJson<SttResponse>(req.downloadHandler.text);
            OnText.Invoke(res.DisplayText);
        }
    }

    private struct SttResponse
    {
        public string RecognitionStatus;
        public string DisplayText;
        public int Offset;
        public int Duration;
    }
}
