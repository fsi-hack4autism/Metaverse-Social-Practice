using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tlsharkey.Toolbox.Environment
{
    public class EnvDependency : MonoBehaviour
    {
        [Tooltip("If this environment variable is set to true, this object will be enabled. Otherwise, it will be disabled.")]
        [SerializeField] private string envVariable = "ENABLE_ME";
        [Tooltip("Enables the gameobject/event if the environment variable is set to false/non-existant. If this value is false, the gameobject/event will be enabled if the environment variable is set to true.")]
        [SerializeField] private bool reverse = false;
        public UnityEngine.Events.UnityEvent<bool> OnEnvChange = new UnityEngine.Events.UnityEvent<bool>();

        private void Awake()
        {
            env.SubscribeTo(envVariable, HandleEnvUpdate);
        }

        private void OnEnable()
        {
            if (env.Has(envVariable))
                HandleEnvUpdate(env.Get(envVariable));
            else
                HandleEnvUpdate("false");
        }

        private void HandleEnvUpdate(string value)
        {

            if (value == "true")
            {
                if (OnEnvChange.GetPersistentEventCount() > 0)
                {
                    if (reverse) OnEnvChange.Invoke(false);
                    else OnEnvChange.Invoke(true);
                }
                else
                {
                    if (reverse) gameObject.SetActive(false);
                    else gameObject.SetActive(true);
                }
            }
            else
            {
                if (OnEnvChange.GetPersistentEventCount() > 0)
                {
                    if (reverse) OnEnvChange.Invoke(true);
                    else OnEnvChange.Invoke(false);
                }
                else
                {
                    if (reverse) gameObject.SetActive(true);
                    else gameObject.SetActive(false);
                }
            }
        }
    }
}