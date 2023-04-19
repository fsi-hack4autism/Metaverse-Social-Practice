using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlatformHandler : MonoBehaviour
{

    public static Platform platform = Platform.Unknown;
    [SerializeField] private Platform _platform = Platform.Unknown; // For unity inspector


    [Header("Platform based conditions")]
    public UnityEvent IfDesktopVR = new UnityEvent();
    public UnityEvent IfMobileVR = new UnityEvent();
    public UnityEvent IfDesktop = new UnityEvent();
    public UnityEvent IfMobile = new UnityEvent();
    public UnityEvent IfWebGL = new UnityEvent();


    private void Awake()
    {
        // Sync instance and static platform variables
        if (this._platform != platform)
        {
            Debug.Log($"[PlatformManager] Aligning static and instance platform variables.\nStatic platform: {platform}\nInstance platform: {this._platform}");
            if (platform != Platform.Unknown)
                this._platform = platform;
            else
                platform = this._platform;
            Debug.Log($"[PlatformManager] Platform variables aligned.\nStatic platform: {platform}\nInstance platform: {this._platform}");
        }

        // If platform is unknown, auto detect it
        if (platform == Platform.Unknown && this._platform == Platform.Unknown)
        {
            AutoDetectPlatform();
        }
        OnPlatformSet();
    }
    private void AutoDetectPlatform()
    {
        // Mobile
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            // VR
            if (UnityEngine.XR.XRSettings.enabled)
            {
                platform = Platform.MobileVR;
            }
            // Non-VR
            else
            {
                platform = Platform.Mobile;
            }
        }
        // WebGL
        else if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            platform = Platform.WebGL;
        }
        // Desktop
        else
        {
            // VR
            if (UnityEngine.XR.XRSettings.enabled)
            {
                platform = Platform.DesktopVR;
            }
            // Non-VR
            else
            {
                platform = Platform.Desktop;
            }
        }

        this._platform = platform;
        Debug.Log("[PlatformManager] Platform auto detected: " + platform.ToString());
    }

    private void OnPlatformSet()
    {
        switch (platform)
        {
            case Platform.DesktopVR:
                IfDesktopVR.Invoke();
                break;
            case Platform.MobileVR:
                IfMobileVR.Invoke();
                break;
            case Platform.Desktop:
                IfDesktop.Invoke();
                break;
            case Platform.Mobile:
                IfMobile.Invoke();
                break;
            case Platform.WebGL:
                IfWebGL.Invoke();
                break;
            default:
                break;
        }

        //NudgeUser();
    }

    /// <summary>
    /// Makes the user take a small setup forward
    /// to engage physics system
    /// </summary>
    //public static void NudgeUser()
    //{
    //    var controller = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>();
    //    // raycast down to floor
    //    var ray = new Ray(controller.transform.position, Vector3.down);
    //    if (Physics.Raycast(ray, out var hit, 10f))
    //    {
    //        // if floor is found, move player to floor
    //        controller.transform.position = hit.point;
    //    }
    //}
}

public enum Platform
{
    Unknown,
    DesktopVR,
    MobileVR,
    Desktop,
    Mobile,
    WebGL
}
