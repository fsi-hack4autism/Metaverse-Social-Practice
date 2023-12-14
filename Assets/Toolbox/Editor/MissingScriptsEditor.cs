
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.SceneManagement; //3
using System.Linq;

public class MissingScriptsEditor : Editor
{
    [MenuItem("Tools/Missing References/Find Missing Scripts")]
    static void SelectGameObjects()
    {
        //Get the current scene and all top-level GameObjects in the scene hierarchy
        List<Scene> scenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).isLoaded)
                scenes.Add(SceneManager.GetSceneAt(i));
        }
        List<GameObject> rootObjects = new List<GameObject>();
        foreach (var scene in scenes)
        {
            var roots = scene.GetRootGameObjects();
            rootObjects.AddRange(roots);
            foreach (var root in roots)
            {
                rootObjects.AddRange(GetChildObjects(root));
            }
        }

        List<Object> objectsWithDeadLinks = new List<Object>();
        foreach (GameObject g in rootObjects)
        {
            //Get all components on the GameObject, then loop through them 
            Component[] components = g.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                Component currentComponent = components[i];

                //If the component is null, that means it's a missing script!
                if (currentComponent == null)
                {
                    //Add the sinner to our naughty-list
                    objectsWithDeadLinks.Add(g);
                    Selection.activeGameObject = g;
                    Debug.Log(g + " has a missing script!");
                    break;
                }
            }
        }
        if (objectsWithDeadLinks.Count > 0)
        {
            //Set the selection in the editor
            Selection.objects = objectsWithDeadLinks.ToArray();
        }
        else
        {
            Debug.Log("No GameObjects in '" + string.Join(", ", scenes.Select(scene => scene.name)) + "' have missing scripts! Yay!");
        }
    }

    [MenuItem("Tools/Missing References/Find Missing References")]
    static void FindMissingReferences()
    {
        //Get the current scene and all top-level GameObjects in the scene hierarchy
        List<Scene> scenes = new List<Scene>();
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).isLoaded)
                scenes.Add(SceneManager.GetSceneAt(i));
        }
        List<GameObject> rootObjects = new List<GameObject>();
        foreach (var scene in scenes)
        {
            var roots = scene.GetRootGameObjects();
            rootObjects.AddRange(roots);
            foreach (var root in roots)
            {
                rootObjects.AddRange(GetChildObjects(root));
            }
        }

        List<Object> objectsWithDeadLinks = new List<Object>();
        foreach (GameObject g in rootObjects)
        {
            Component[] components = g.GetComponents<Component>();
            foreach (Component c in components)
            {
                if (c == null)
                    continue;
                SerializedObject so = new SerializedObject(c);
                var sp = so.GetIterator();
                while (sp.NextVisible(true))
                {
                    if (sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        if (sp.objectReferenceValue == null && sp.objectReferenceInstanceIDValue != 0)
                        {
                            objectsWithDeadLinks.Add(g);
                            Selection.activeGameObject = g;
                            Debug.Log(g + " has a missing reference!");
                            break;
                        }
                    }
                }
            }
        }

        if (objectsWithDeadLinks.Count > 0)
        {
            //Set the selection in the editor
            Selection.objects = objectsWithDeadLinks.ToArray();
        }
        else
        {
            Debug.Log("No GameObjects in '" + string.Join(", ", scenes.Select(scene => scene.name)) + "' have missing references! Yay!");
        }
    }

    private static List<GameObject> GetChildObjects(GameObject parent)
    {
        List<GameObject> childObjects = new List<GameObject>();
        foreach (Transform child in parent.transform)
        {
            childObjects.Add(child.gameObject);
            childObjects.AddRange(GetChildObjects(child.gameObject));
        }
        return childObjects;
    }
}
