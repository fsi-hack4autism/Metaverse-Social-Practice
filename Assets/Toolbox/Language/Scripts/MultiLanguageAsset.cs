using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiLanguageAsset : MultiLanguageElement
{
    [SerializeField]
    public List<LanguageManager.LanguageString> Content = new List<LanguageManager.LanguageString>();
    private LanguageManager.LanguageString currentContent;
    public UnityEvent<string> OnLanguageChanged = new UnityEvent<string>();


    override protected void ApplyElement(LanguageManager.LanguageElement element)
    {
        //Debug.Log("ApplyElement: Asset - " + element.GetType());
        OnLanguageChanged.Invoke((element as LanguageManager.LanguageString).text);
    }

    protected override void HandleLanguageChanged(LanguageManager.Language language)
    {
        // Debug.Log("[MultiLanguageAsset] HandleLanguageChanged: " + language.ToString() + "\n" + this.Content.Count + " elements");
        for (int i = 0; i < Content.Count; i++)
        {
            if (Content[i].language == language)
            {
                currentContent = Content[i];
                // Debug.Log("[MultiLanguageText] HandleLanguageChanged: " + currentContent.GetType());
                ApplyElement(currentContent);
                return;
            }
        }
    }
}
