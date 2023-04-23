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

            var parts = Regex.Split(lines[i], "=");
            string key = parts[0].Trim();
            string value = parts[1].Trim().Trim('"');

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
        }
    }

    public static string Get(string key)
    {
        if (_config.ContainsKey(key))
            return _config[key];
        else
            return null;
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
}
