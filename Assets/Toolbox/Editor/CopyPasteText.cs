using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class CopyPasteText : Editor
{

    /// <summary>
    /// Copies all of the text in all child objects of the selected object
    /// </summary>
    [MenuItem("Tools/Language/Copy Text/Json")]
    static void CopyTextInGameObject_Json()
    {
        List<SerializedText> texts = new List<SerializedText>();

        // Get all TMPro objects
        var parent = Selection.activeGameObject.transform;
        foreach (var text in parent.gameObject.GetComponentsInChildren<TMPro.TMP_Text>())
        {
            texts.Add(new SerializedText()
            {
                Id = GetObjectId(text),
                Text = text.text
            });
        };

        // Serialize and copy to clipboard
        string json = JsonUtility.ToJson(texts);
        EditorGUIUtility.systemCopyBuffer = json;
    }

    /// <summary>
    /// Pastes text from keyboard into all child objects of the selected object
    /// </summary>
    [MenuItem("Tools/Language/Paste Text/Json")]
    static void PasteTextInGameObject_Json()
    {
        // get clipboard text
        string json = EditorGUIUtility.systemCopyBuffer;
        List<SerializedText> texts = JsonUtility.FromJson<List<SerializedText>>(json);

        // Get all TMPro objects
        var parent = Selection.activeGameObject.transform;
        foreach (var text in parent.gameObject.GetComponentsInChildren<TMPro.TMP_Text>())
        {
            string before = text.text;
            // Find the text that matches the instance id
            int i = texts.IndexOf(texts.Where(x => x.Id == GetObjectId(text)).FirstOrDefault());
            if (i != -1)
            {
                text.text = texts[i].Text;
                Debug.Log($"Replaced text in {text.name} from\n\n{before}\n\nto\n\n{text.text}");
            }
            else
            {
                Debug.LogError($"Could not find text for {text.name}\n\n{before}");
            }
        };
    }

    /// <summary>
    /// Copies all of the text in all child objects of the selected object
    /// Serializes as plain text
    /// </summary>
    [MenuItem("Tools/Language/Copy Text/Plain Text")]
    static void CopyTextInGameObject_PlainText()
    {
        string serializedText = "";

        // Get all TMPro objects
        var parent = Selection.activeGameObject.transform;
        foreach (var text in parent.gameObject.GetComponentsInChildren<TMPro.TMP_Text>())
        {
            serializedText += $"[[{GetObjectId(text).ToString()}]]\n";
            serializedText += text.text + "\n\n[[---]]\n\n";
        };

        // Copy to clipboard
        EditorGUIUtility.systemCopyBuffer = serializedText;
    }

    /// <summary>
    /// Pastes text from keyboard into all child objects of the selected object
    /// </summary>
    [MenuItem("Tools/Language/Paste Text/Plain Text")]
    static void PasteTextInGameObject_PlainText()
    {
        // get clipboard text
        string serializedText = EditorGUIUtility.systemCopyBuffer;
        Debug.Log($"[PasteTextInGameObject_PlainText] {serializedText}");

        // convert all line endings to \n
        serializedText = serializedText.Replace("\r\n", "\n");
        serializedText = serializedText.Replace("\r", "\n");

        // Get all TMPro objects
        var parent = Selection.activeGameObject.transform;
        foreach (var text in parent.gameObject.GetComponentsInChildren<TMPro.TMP_Text>())
        {
            string before = text.text;
            // Find the text that matches the instance id
            string id = GetObjectId(text);
            int start = serializedText.IndexOf($"[[{id}]]");
            if (start != -1)
            {
                int end = serializedText.IndexOf("\n\n[[---]]\n\n", start);
                if (end != -1)
                {
                    text.text = serializedText.Substring(start + id.Length + 4, end - start - id.Length - 4);
                    Debug.Log($"Replaced text in {text.name} from\n\n{before}\n\nto\n\n{text.text}");
                }
            }
            else
            {
                Debug.LogError($"Could not find end of text for {text.name}\n\n{before}");
            }
        };
    }

    private static string GetObjectId(MonoBehaviour obj)
    {
        Transform target = obj.transform;
        string id = "";
        while (target != null)
        {
            id = target.GetSiblingIndex() + "/" + id;
            target = target.parent;
        }
        return id;
    }


    public struct SerializedText
    {
        public string Id;
        public string Text;
    }
}
