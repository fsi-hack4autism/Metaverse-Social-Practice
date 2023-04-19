using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class PlatformHandler : MonoBehaviour
{

    public static Platform platform = Platform.Unknown;
    [SerializeField] private Platform _platform = Platform.Unknown; // For unity inspector

    [SerializeField] public PlatformController[] Controllers;

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

    /// <summary>
    /// Attempts to automatically set the platform
    /// Calls the event for the platform
    /// Looks through platform controllers for the right controller
    /// </summary>
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

    /// <summary>
    /// Handles enabling and disabling platform controllers
    /// And calls appropriate functions when the platform changes
    /// </summary>
    private void OnPlatformSet()
    {
        // Disable all controllers
        foreach (var c in Controllers)
        {
            c.controller.SetActive(false);
        }

        // Set controller active for platform and invoke event
        switch (platform)
        {
            case Platform.DesktopVR:
                //Controllers.Where(c => c.isVR && c.platforms.Contains(Platform.Desktop)).ToArray()[0].controller.SetActive(true);
                IfDesktopVR.Invoke();
                break;
            case Platform.MobileVR:
                //Controllers.Where(c => c.isVR && c.platforms.Contains(Platform.Mobile)).ToArray()[0].controller.SetActive(true);
                IfMobileVR.Invoke();
                break;
            case Platform.Desktop:
                //Controllers.Where(c => !c.isVR && c.platforms.Contains(Platform.Desktop)).ToArray()[0].controller.SetActive(true);
                IfDesktop.Invoke();
                break;
            case Platform.Mobile:
                //Controllers.Where(c => !c.isVR && c.platforms.Contains(Platform.Mobile)).ToArray()[0].controller.SetActive(true);
                IfMobile.Invoke();
                break;
            case Platform.WebGL:
                //Controllers.Where(c => !c.isVR && c.platforms.Contains(Platform.WebGL)).ToArray()[0].controller.SetActive(true);
                IfWebGL.Invoke();
                break;
            default:
                break;
        }
    }

    [System.Serializable]
    public struct PlatformController
    {
        public GameObject controller;
        public Platform[] platforms;
        public bool isVR;
    }
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
