using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class MicrophoneHandler : MonoBehaviour
{
    private AudioClip _audioClip;
    private bool _isRecording;
    public string Device { get; private set; }
    public bool isRecording
    {
        get { return this._isRecording; }
    }

    [Tooltip("Number of minutes an audio recording can last for")]
    public float MaxDuration = 10;
    public GameObject Visual;

    public UnityEvent<string> OnUserComment = new UnityEvent<string>();


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

        int _;
        int freq = -1;
        Microphone.GetDeviceCaps(this.Device, out _, out freq);
        this._audioClip = Microphone.Start(this.Device, false, (int)(this.MaxDuration * 60), freq);

        if (Visual) Visual.SetActive(true);
    }

    //private IEnumerator WhileRecording()
    //{
    //    float startTime = Time.time;

    //    // extend recording length as needed
    //    while (this._isRecording)
    //    {
    //        // wait until recording duration is almost finished
    //        yield return new WaitForSeconds(this._audioClip.length - 1);
    //        if (this._isRecording)
    //        {
    //            this._audioClip.
    //        }
    //    }
    //}

    public void StopRecording()
    {
        if (!this._isRecording) return;
        this._isRecording = false;

        Microphone.End(this.Device);
        CleanRecording(this._audioClip);
        StartCoroutine(SpeechToText(this._audioClip));

        if (Visual) Visual.SetActive(false);
    }

    private void CleanRecording(AudioClip clip)
    {

    }

    public void AudioClip_to_Wav(AudioClip clip)
    {
        Debug.Log($"[MicrophoneHandler] got audio clip of length {clip.length}");
        var source = this.gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
    }

    private IEnumerator SpeechToText(AudioClip audio)
    {
        yield break;
    }
}
