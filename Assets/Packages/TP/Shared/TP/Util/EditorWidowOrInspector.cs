using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OpenAi
{
    public class EditorWidowOrInspector<T> : Editor
    {
        public delegate void Callback(T response=default);

        public Callback OnUpdate = null;

        public bool isEditorWindow;
        
        public Object InternalTarget
        {
            set
            {
                // Hack to set target since Editor class doesn't provide an interface to do so. 
                var targetField = typeof(Editor).GetField("m_Targets", BindingFlags.Instance | BindingFlags.NonPublic);
                targetField.SetValue(this, new []{value});
                isEditorWindow = true;
            }
            get
            {
                try
                {
                    var targetField = typeof(Editor).GetField("m_Targets", BindingFlags.Instance | BindingFlags.NonPublic);
                    Object[] targets = (Object[])targetField.GetValue(this);
                    if (targets == null || targets.Length == 0)
                    {
                        return null;
                    }
                    return targets[0];
                    
                    // return target;
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(exception.Message);
                    return null;
                }
            }
        }
    }
}