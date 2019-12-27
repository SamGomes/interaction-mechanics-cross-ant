using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class LogManager
{
    public string StringifyDictionaryForLogs(Dictionary<string,string> dict)
    {
        string result = "{";
        List<string> dictKeys = new List<string>(dict.Keys);
        for (int keyI=0; keyI < dictKeys.Count; keyI++)
        {
            string key = dictKeys[keyI];

            result += " "+ key + ": \"" + dict[key] + "\"";
            if(keyI < dictKeys.Count - 1)
            {
                result += ",";
            }
            else
            {
                result += " }";

            }
        }
        return result;
    }

    public abstract void InitLogs(MonoBehaviour monoBehaviourObject);
    public abstract IEnumerator WriteToLog(string database, string table, Dictionary<string, string> argsNValues);
   
    public abstract IEnumerator GetFromLog(string database, string table, Func<string,int> yieldedReactionToGet);
    public abstract IEnumerator EndLogs();
}
