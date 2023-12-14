using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public static class JsonUtilityExt
{
    /// <summary>
    /// Breaks a json array into a string array.
    /// e.g. "[{}, {}, {}]" -> ["{}", "{}", "{}"]
    /// </summary>
    /// <param name="json">the json string that should be broken into an array</param>
    /// <returns></returns>
    public static string[] BreakArray(string json)
    {
        // check if json is an array
        json = json.Trim();
        if (json.StartsWith("[") == false || json.EndsWith("]") == false)
            throw new System.Exception($"[JsonUtilityExt.BreakArray] json string is not an array");

        // remove the brackets
        json = json.Substring(1, json.Length - 2);

        // get the locations of all the commas (ignore commas inside of objects and arrays)
        List<int> commaIndecies = new List<int>();
        int bracketCount = 0;
        int braceCount = 0;
        bool inString = false;
        for (int i = 0; i < json.Length; i++)
        {
            if (json[i] == '[' && !inString)
                bracketCount++;
            else if (json[i] == ']' && !inString)
                bracketCount--;
            else if (json[i] == '{' && !inString)
                braceCount++;
            else if (json[i] == '}' && !inString)
                braceCount--;
            else if (json[i] == '"' && !inString && (i == 0 || json[i - 1] != '\\'))
                inString = true;
            else if (json[i] == '"' && inString && (i == 0 || json[i - 1] != '\\'))
                inString = false;
            else if (json[i] == ',' && bracketCount == 0 && braceCount == 0 && !inString)
                commaIndecies.Add(i);
            else if (json[i] == ':' && bracketCount == 0 && braceCount == 0 && !inString)
                throw new System.Exception($"[JsonUtilityExt.BreakArray] json string contains a colon (:) outside of an object\nindex {i}. Index 10 in: \"{json.Substring(i > 10 ? i - 10 : 0, json.Length > i + 20 ? 20 : json.Length)}\"\n{json}\n");
        }

        // split the json string into an array of strings
        string[] split = new string[commaIndecies.Count + 1];
        int lastCommaIndex = -1;
        for (int i = 0; i < commaIndecies.Count; i++)
        {
            split[i] = json.Substring(lastCommaIndex + 1, commaIndecies[i] - lastCommaIndex - 1);
            lastCommaIndex = commaIndecies[i];
        }
        split[split.Length - 1] = json.Substring(lastCommaIndex + 1, json.Length - lastCommaIndex - 1);

        return split;
    }

    /// <summary>
    /// Takes a string that has escaped characters and unescapes them.
    /// e.g. "{\"foo\":\"bar\"}" -> {"foo":"bar"}
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static string Unescape(string json)
    {
        Regex regex = new Regex(@"\\[\\\""\n\r\t\b\f]");

        // trim leading and trailing quotes
        if (json.StartsWith("\"") && json.EndsWith("\""))
            json = json.Substring(1, json.Length - 2);

        // replace all escaped characters
        return regex.Replace(json, match =>
        {
            switch (match.Value)
            {
                case "\\\\":
                    return "\\";
                case "\\\"":
                    return "\"";
                case "\\n":
                    return "\n";
                case "\\r":
                    return "\r";
                case "\\t":
                    return "\t";
                case "\\b":
                    return "\b";
                case "\\f":
                    return "\f";
                default:
                    return match.Value;
            }
        });
    }

    public static string Escape(string json)
    {
        Regex regex = new Regex(@"[\\\""\n\r\t\b\f]");

        // replace all escaped characters
        return regex.Replace(json, match =>
        {
            switch (match.Value)
            {
                case "\\":
                    return "\\\\";
                case "\"":
                    return "\\\"";
                case "\n":
                    return "\\n";
                case "\r":
                    return "\\r";
                case "\t":
                    return "\\t";
                case "\b":
                    return "\\b";
                case "\f":
                    return "\\f";
                default:
                    return match.Value;
            }
        });
    }
}
