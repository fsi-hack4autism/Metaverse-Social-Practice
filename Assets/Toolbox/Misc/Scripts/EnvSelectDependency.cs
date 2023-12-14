using UnityEngine;
using UnityEngine.Events;

namespace tlsharkey.Toolbox.Environment
{
    public class EnvSelectDependency : MonoBehaviour
    {
        [System.Serializable]
        public struct Selection
        {
            public string envValue;
            public UnityEvent onActivate;
            public UnityEvent onDeactivate;
        }

        [Tooltip("The Selection event will be triggered when this environment variable changes (and on initialisation).")]
        [SerializeField] private string envVariable = "SELECT_ME";

        [SerializeField]
        [Tooltip("This selection is used if it matches or if no other selection matches")]
        private Selection defaultSelection;

        [SerializeField]
        [Tooltip("The selection with the envValue matching the value of the specified environment variable will be activated, and all others deactivated")]
        private Selection[] selections;

        private void OnEnable()
        {
            if (env.Has(envVariable))
            {
                HandleSelection(env.Get(envVariable));
            }
            else
            {
                HandleSelection(null);
            }

            env.SubscribeTo(envVariable, HandleSelection);
        }

        private void OnDisable()
        {
            env.UnsubscribeFrom(envVariable, HandleSelection);
        }

        private void HandleSelection(string envValue)
        {
            // Deactivate all selections
            foreach (var selection in selections)
            {
                selection.onDeactivate.Invoke();
            }
            defaultSelection.onDeactivate.Invoke();

            if (string.IsNullOrEmpty(envValue) || defaultSelection.envValue == envValue)
            {
                defaultSelection.onActivate.Invoke();
            }
            else
            {
                foreach (var selection in selections)
                {
                    if (selection.envValue == envValue)
                    {
                        selection.onActivate.Invoke();
                    }
                }
            }
        }
    }
}