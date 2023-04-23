using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

public class WozHandler : MonoBehaviour
{
    [SerializeField] private VisemedAudio[] Responses;
    private int _index = -1;
    private MicrophoneControls _input = null;



    private void Awake()
    {
        _input = new MicrophoneControls();

        for (int i = 0; i < Responses.Length; i++)
        {
            Responses[i].LoadVisemeFile();
        }
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Microphone.WozBackward.performed += GoBack;
        _input.Microphone.WozForward.performed += GoForward;
    }

    private void OnDisable()
    {
        _input.Disable();
        _input.Microphone.WozBackward.performed -= GoBack;
        _input.Microphone.WozForward.performed -= GoForward;
    }

    private void GoBack(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _index -= 1;
        if (_index < 0) _index = 0;
        Responses[_index].source.Speak(Responses[_index].audio, Responses[_index].visemes);
    }

    private void GoForward(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        _index += 1;
        if (_index >= Responses.Length) _index = Responses.Length - 1;
        Responses[_index].source.Speak(Responses[_index].audio, Responses[_index].visemes);
    }
}



[System.Serializable]
public struct VisemedAudio
{
    public AudioClip audio;
    public Streamer.Viseme[] visemes;
    public string visemeFile;
    public Persona source;

    public void LoadAudio(byte[] rawAudio, int channels, int frequency)
    {
        this.audio = AudioClip.Create("audio", rawAudio.Length / 2, channels, frequency, false);
        // convert bytes to floats
        float[] audioData = new float[rawAudio.Length / 2];
        for (int i = 0; i < audioData.Length; i++)
        {
            audioData[i] = (float)System.BitConverter.ToInt16(rawAudio, i * 2) / 32768f;
        }
        audio.SetData(audioData, 0);
    }

    public void LoadRawVisemes(string visemeString)
    {
        /*
        Parse the following format into a Viseme array:

        Viseme event received: audio offset: 50.0ms, viseme id: 0.
        Viseme event received: audio offset: 100.0ms, viseme id: 19.
        */
        string[] lines = Regex.Split(visemeString, "$", RegexOptions.Multiline);

        List<Streamer.Viseme> visemes = new List<Streamer.Viseme>();
        foreach (string line in lines)
        {
            // skip empty lines
            if (line.Trim().Equals("")) continue;
            // get numbers in line
            var numberMatches = Regex.Matches(line, @"[0-9.]+");
            float time = float.Parse(numberMatches[0].Value) / 1000; // convert milliseconds to seconds
            int viseme = int.Parse(numberMatches[1].Value.Trim('.'));
            // add viseme to list
            visemes.Add(new Streamer.Viseme()
            {
                time = time,
                viseme = viseme
            });
        }

        this.visemes = visemes.ToArray();
    }

    public void LoadVisemeFile()
    {
        string visemeString = Resources.Load<TextAsset>(visemeFile).text;
        this.LoadRawVisemes(visemeString);
    }
}