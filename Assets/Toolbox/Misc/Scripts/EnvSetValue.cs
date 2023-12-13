using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tlsharkey.Toolbox.Environment
{
    public class EnvSetValue : MonoBehaviour
    {
        public string key;
        public string value = "true";
        public bool setOnEnable = false;
        public bool unsetOnDisable = false;

        private void OnEnable()
        {
            if (setOnEnable)
                Set();
        }

        private void OnDisable()
        {
            if (unsetOnDisable)
                env.SetRuntimeKey(key, "");
        }

        public void Set()
        {
            env.SetRuntimeKey(key, value);
        }
    }
}