using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace tlsharkey.Toolbox.Environment
{
    public class EnvAudioSwitch : MonoBehaviour
    {

        [Tooltip("The env variable to respond to.")]
        [SerializeField] private string envVariable = "ENABLE_ME";
        [SerializeField] private EnvValue[] elms = new EnvValue[0];

        private void Awake()
        {
            if (env.Has(envVariable))
                HandleEnvUpdate(env.Get(envVariable));
            else
                HandleEnvUpdate("false");

            env.SubscribeTo(envVariable, HandleEnvUpdate);
        }

        private void HandleEnvUpdate(string value)
        {
            elms.Where(elm => elm.value == value).ToList().ForEach(elm =>
            {
                if (elm.OnSwitch.GetPersistentEventCount() > 0)
                    elm.OnSwitch.Invoke(true);
                else if (elm.clip != null)
                    GetComponent<AudioSource>().clip = elm.clip;
            });
        }

        [System.Serializable]
        public struct EnvValue
        {
            public string value;
            public AudioClip clip;
            public UnityEngine.Events.UnityEvent<bool> OnSwitch;
        }
    }
}