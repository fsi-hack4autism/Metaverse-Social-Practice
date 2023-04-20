using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MicrophoneHandler : MonoBehaviour
{
    private AudioClip _audioClip;
    private bool _isRecording;
    private float _startTime;
    public string Device { get; private set; }
    public bool isRecording
    {
        get { return this._isRecording; }
    }

    [Tooltip("Number of minutes an audio recording can last for")]
    public float MaxDuration = 10;
    public GameObject Visual;


    private MicrophoneControls input = null;

    private void Awake()
    {
        input = new MicrophoneControls();
        if (Visual) Visual.SetActive(false);
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
            StartRecording();
        }
        else
        {
            StopRecording();
        }
    }




    /* ===== MICROPHONE ===== */

    /// <summary>
    /// Immediately sets the default device
    /// And gets user permissions
    /// </summary>
    private void Start()
    {
        this.Device = Microphone.devices[0];
        StartCoroutine(GetPermissions());
    }

    private IEnumerator GetPermissions()
    {
        while (!Application.HasUserAuthorization(UserAuthorization.Microphone))
        {
            yield return Application.RequestUserAuthorization(UserAuthorization.Microphone);
        }
    }

    public void SetDevice(string device)
    {
        if (Microphone.devices.Contains(device) && !this._isRecording)
        {
            this.Device = device;
        }
        else
        {
            throw new System.Exception($"[MicrophoneHandler] Device '{device}' not found.");
        }
    }

    public void StartRecording()
    {
        if (this._isRecording) return;
        this._isRecording = true;
        this._startTime = Time.time;

        int _;
        int freq = -1;
        Microphone.GetDeviceCaps(this.Device, out _, out freq);
        this._audioClip = Microphone.Start(this.Device, false, (int)(this.MaxDuration * 60), freq);

        if (Visual) Visual.SetActive(true);
    }

    public void StopRecording()
    {
        if (!this._isRecording) return;
        this._isRecording = false;
        float stopTime = Time.time;

        Microphone.End(this.Device);
        CleanRecording(this._audioClip, stopTime - this._startTime);
        GetComponent<SpeechToTextHandler>().Convert(this._audioClip);

        if (Visual) Visual.SetActive(false);
    }

    /// <summary>
    /// Trims silence
    /// </summary>
    /// <param name="clip"></param>
    private void CleanRecording(AudioClip clip, float duration)
    {
        float threshold = 0.001f;
        int endIndex = -1;
        float[] data = new float[clip.samples];
        clip.GetData(data, 0);

        for (int i = data.Length - 1; i >= 0; i--)
        {
            if (Mathf.Abs(data[i]) > threshold)
            {
                endIndex = i;
                break;
            }
        }
        Debug.Log($"[MicrophoneHandler] trimming at {endIndex} / {data.Length}");

        float[] newData = new float[data.Length - endIndex];
        Buffer.BlockCopy(data, 0, newData, 0, newData.Length);
        clip.SetData(newData, 0);
    }
}
