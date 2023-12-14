using UnityEngine;
using UnityEngine.Events;
namespace tlsharkey.Toolbox.Environment
{
    public class EnvToggleDependency : MonoBehaviour
    {
        [Tooltip("The true or false event will be triggered when this environment variable changes (and on initialisation).")]
        [SerializeField] private string envVariable = "ENABLE_ME";
        public UnityEvent onEnvToggledTrue;
        public UnityEvent onEnvToggledFalse;

        private void OnEnable()
        {
            if (env.Has(envVariable) && env.Get(envVariable) == "true")
            {
                onEnvToggledTrue.Invoke();
            }
            else
            {
                onEnvToggledFalse.Invoke();
            }

            env.SubscribeTo(envVariable, HandleEnvValueUpdate);
        }

        private void OnDisable()
        {
            env.UnsubscribeFrom(envVariable, HandleEnvValueUpdate);
        }

        private void HandleEnvValueUpdate(string value)
        {
            if (value == "true")
            {
                onEnvToggledTrue.Invoke();
            }
            else
            {
                onEnvToggledFalse.Invoke();
            }
        }
    }
}