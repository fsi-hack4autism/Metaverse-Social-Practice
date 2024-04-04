using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerformanceManager : MonoBehaviour
{
    [SerializeField] private FPS FpsSettings;
    private void Awake()
    {
        Application.targetFrameRate = FpsSettings.TargetFps;
        QualitySettings.vSyncCount = 60 / FpsSettings.TargetFps;
    }

    void Update()
    {
        FpsSettings.deltaTime += (Time.unscaledDeltaTime - FpsSettings.deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        if (FpsSettings.DisplayOnScreen) ShowFpsOnScreen();
    }

    private void ShowFpsOnScreen()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, h - h * 2 / 100, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.5f, 1.0f);

        float msec = FpsSettings.deltaTime * 1000.0f;
        float fps = 1.0f / FpsSettings.deltaTime;
        string text = string.Format("{0:0.0} ms - {1:0.} fps (target {1:0.})", msec, fps, Application.targetFrameRate);

        GUI.Label(rect, text, style);
    }




    [System.Serializable]
    public struct FPS
    {
        public int TargetFps;
        public bool DisplayOnScreen;
        [HideInInspector]
        public float deltaTime;
    }
}
