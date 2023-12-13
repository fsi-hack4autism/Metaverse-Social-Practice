using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private RectTransform progressbar;
    public void SetProgress(float value)
    {
        Debug.Log("[LoadingBar] Setting progress to " + value);
        // set progress bar's anchor x max to value and update width
        progressbar.anchorMax = new Vector2(value, progressbar.anchorMax.y);
        progressbar.sizeDelta = new Vector2(0, 0);
    }

    public void SetProgressScene(float value)
    {
        if (value < 0.9f)
            value = Mathf.Lerp(0, 0.9f, Mathf.InverseLerp(0.87f, 0.9f, value));
    }
}
