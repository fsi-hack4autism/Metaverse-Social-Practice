using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(TMP_Text))]
public class MultiLanguageText : MultiLanguageElement
{
    [SerializeField]
    public List<LanguageManager.LanguageString> Content = new List<LanguageManager.LanguageString>();
    protected LanguageManager.LanguageString currentContent;

    private void OnEnable()
    {
        HandleLanguageChanged(LanguageManager.language);
    }

    protected override void HandleLanguageChanged(LanguageManager.Language language)
    {
        Debug.Log("[MultiLanguageText] HandleLanguageChanged: " + language.ToString() + "\n" + this.Content.Count + " elements");
        for (int i = 0; i < Content.Count; i++)
        {
            if (Content[i].language == language)
            {
                currentContent = Content[i];
                Debug.Log("[MultiLanguageText] HandleLanguageChanged: " + currentContent.GetType());
                ApplyElement(currentContent);
                return;
            }
        }
    }

    override protected void ApplyElement(LanguageManager.LanguageElement element)
    {
        Debug.Log("ApplyElement: Text - " + element.GetType());
        GetComponent<TMP_Text>().text = (element as LanguageManager.LanguageString).text;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(MultiLanguageText))]
public class MultiLanguageTextEditor : Editor
{
    private bool addingLanguage = false;
    private string language;
    private string text;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MultiLanguageText mlt = (MultiLanguageText)target;


        // Currently added language
        if (addingLanguage)
        {
            this.language = EditorGUILayout.TextField("Language", this.language);
            this.text = EditorGUILayout.TextField("Text", this.text);
        }

        // Add language button
        GUILayout.BeginHorizontal();
        if (addingLanguage)
        {
            if (GUILayout.Button("Save"))
            {
                addingLanguage = !addingLanguage;

                // check if mtl.Content is null
                if (mlt.Content == null)
                    mlt.Content = new List<LanguageManager.LanguageString>();

                // add new language
                LanguageManager.Language language = (LanguageManager.Language)System.Enum.Parse(typeof(LanguageManager.Language), this.language.ToLower());
                mlt.Content.Add(new LanguageManager.LanguageString(language, this.text));

                // reset values
                this.text = "";
                this.language = "";
            }

            if (GUILayout.Button("Cancel"))
                addingLanguage = !addingLanguage;
        }
        else
        {
            if (GUILayout.Button("Add Language"))
                addingLanguage = !addingLanguage;
        }
        GUILayout.EndHorizontal();

        // Update text object
        // UpdateElement();
    }

    private void UpdateElement()
    {
        MultiLanguageText mlt = (MultiLanguageText)target;
        if (mlt.Content == null)
            mlt.Content = new List<LanguageManager.LanguageString>();
        for (int i = 0; i < mlt.Content.Count; i++)
        {
            if (mlt.Content[i].language == LanguageManager.language)
            {
                mlt.GetComponent<TMP_Text>().text = mlt.Content[i].text;
                return;
            }
        }
    }
}
#endif