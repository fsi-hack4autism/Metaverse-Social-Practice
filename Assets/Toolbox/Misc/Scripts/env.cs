using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.Linq;

public class env : MonoBehaviour
{
    [SerializeField] private TextAsset ConfigFile;
    private static Dictionary<string, string> _config = new Dictionary<string, string>();
    private Dictionary<string, string> _localconfig = new Dictionary<string, string>();
    private static Dictionary<string, List<System.Action<string>>> _subscribers = new Dictionary<string, List<System.Action<string>>>();

    private void Awake()
    {
        // check if config file exists
        if (ConfigFile == null)
        {
            // check if file exists in resources
            ConfigFile = Resources.Load<TextAsset>("env");
            if (ConfigFile == null)
            {
                Debug.LogError("[env] environment file not found. Please create a file called 'env.txt' in the Resources folder, or assign it in the inspector.");
                return;
            }
        }

        var lines = Regex.Split(ConfigFile.text, "$", RegexOptions.Multiline);
        for (int i = 0; i < lines.Length; i++)
        {
            // remove comments
            lines[i] = Regex.Replace(lines[i], "#.*$", "");
            if (lines[i].Trim().Length == 0) continue;

            var parts = lines[i].Split('=', 2, System.StringSplitOptions.None); // split on first '='
            string key = parts[0].Trim(' ', '\n', '\r');
            string value = parts[1].Trim(' ', '\n', '\r');
            // handle quoted values
            if (value.StartsWith('"') && value.EndsWith('"'))
            {
                value = value.Substring(1, value.Length - 2);
            }

            // class config
            if (!_config.ContainsKey(key))
            {
                _config.Add(key, value);
            }
            else
            {
                _config[key] = value;
                Debug.LogWarning($"[env] key {key} already exists, overwriting");
            }

            // local config
            if (!_localconfig.ContainsKey(key))
            {
                _localconfig.Add(key, value);
            }
            else
            {
                _localconfig[key] = value;
            }

            HandleCallback(key, value);
        }
    }

    public static bool Has(string key)
    {
        return _config.ContainsKey(key);
    }

    public static bool ContainsKey(string key)
    {
        return _config.ContainsKey(key);
    }

    public static string Get(string key)
    {
        if (_config.ContainsKey(key))
            return _config[key];
        else
            return null;
    }

    public static string SetRuntimeKey(string key, string value)
    {
        if (_config.ContainsKey(key))
        {
            _config[key] = value;
            HandleCallback(key, value);
            return value;
        }
        else
        {
            _config.Add(key, value);
            HandleCallback(key, value);
            return value;
        }
    }

    public string this[string key]
    {
        get
        {
            if (_localconfig.ContainsKey(key))
                return _localconfig[key];
            else if (_config.ContainsKey(key))
                return _config[key];
            else
                return null;
        }
    }

    /// <summary>
    /// Subscribes a callback function to changes in a variable.
    /// </summary>
    /// <param name="variableName">the variable to subscribe to</param>
    /// <param name="callback">the function that will be called if/when the variable changes</param>
    public static void SubscribeTo(string variableName, System.Action<string> callback)
    {
        // create list if it doesn't exist
        if (!_subscribers.ContainsKey(variableName))
        {
            _subscribers.Add(variableName, new List<System.Action<string>>());
        }

        // add to list
        _subscribers[variableName].Add(callback);
    }

    /// <summary>
    /// Unsubscribes a callback function from changes in a variable.
    /// </summary>
    /// <param name="variableName"></param>
    /// <param name="callback"></param>
    public static void UnsubscribeFrom(string variableName, System.Action<string> callback)
    {
        // remove from list
        if (_subscribers.ContainsKey(variableName))
        {
            _subscribers[variableName].Remove(callback);
        }

        // remove list if empty
        if (_subscribers[variableName].Count == 0)
        {
            _subscribers.Remove(variableName);
        }
    }

    private static void HandleCallback(string key, string value)
    {
        if (_subscribers.ContainsKey(key))
        {
            foreach (var callback in _subscribers[key])
            {
                try
                {
                    callback(value);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[env] error in callback for {key}: {e}");
                }
            }
        }
    }
}
