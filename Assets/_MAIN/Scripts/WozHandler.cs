using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

public class WozHandler : MonoBehaviour
{
    [SerializeField] private Response[] Responses;
    private int _index = -1;
    private MicrophoneControls _input = null;



    private void Awake()
    {
        _input = new MicrophoneControls();

        for (int i = 0; i < Responses.Length; i++)
        {
            Responses[i].LoadVisemeFile();
            Debug.Log($"[Woz] loaded {Responses[i].visemes.Length} visemes");
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




    [System.Serializable]
    public struct Response
    {
        public AudioClip audio;
        public Streamer.Viseme[] visemes;
        public string visemeFile;
        public Persona source;

        public void LoadVisemeFile()
        {
            /*
            Parse the following format into a Viseme array:

            Viseme event received: audio offset: 50.0ms, viseme id: 0.
            Viseme event received: audio offset: 100.0ms, viseme id: 19.
            */
            string visemeString = Resources.Load<TextAsset>(visemeFile).text;
            string[] lines = visemeString.Split("\n");

            List<Streamer.Viseme> visemes = new List<Streamer.Viseme>();
            foreach (string line in lines)
            {
                //Debug.Log($"[Woz] parsing line\n{line}");
                // get numbers in line
                var numberMatches = Regex.Matches(line, @"[0-9.]+");
                //Debug.Log($"[Woz] got {numberMatches.Count} numbers in line");
                //Debug.Log($"[Woz] {numberMatches[0].Value} and {numberMatches[1].Value}");
                float time = float.Parse(numberMatches[0].Value) / 1000; // convert milliseconds to seconds
                int viseme = int.Parse(numberMatches[1].Value.Trim('.'));
                visemes.Add(new Streamer.Viseme()
                {
                    time = time,
                    viseme = viseme
                });
            }

            this.visemes = visemes.ToArray();
        }
    }
}
