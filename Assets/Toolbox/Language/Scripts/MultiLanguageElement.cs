using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class MultiLanguageElement : MonoBehaviour
{
    protected virtual void Awake()
    {
        if (LanguageManager.Instance)
        {
            //Debug.Log("[MultiLanguageElement] Awake: " + this.GetType());
            LanguageManager.Instance.AddElement(this);
            LanguageManager.OnLanguageChanged(HandleLanguageChanged);
            HandleLanguageChanged(LanguageManager.language);
        }
    }

    private void OnEnable()
    {
        this.CheckForUpdates();
    }

    protected virtual void OnDestroy()
    {
        if (LanguageManager.Instance)
            LanguageManager.Instance.RemoveElement(this);
    }

    public void CheckForUpdates()
    {
        if (LanguageManager.Instance)
            HandleLanguageChanged(LanguageManager.language);
        else
            StartCoroutine(CheckForUpdatesNextFrame());
    }

    private IEnumerator CheckForUpdatesNextFrame()
    {
        yield return null;
        this.CheckForUpdates();
    }

    protected abstract void HandleLanguageChanged(LanguageManager.Language language);

    protected abstract void ApplyElement(LanguageManager.LanguageElement element);
}
