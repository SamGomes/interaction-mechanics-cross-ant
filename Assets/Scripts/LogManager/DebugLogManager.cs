using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Debug log manager
public class DebugLogManager : LogManager
{
    public override void InitLogs(MonoBehaviour monoBehaviourObject)
    {
        Debug.Log("Log Initialzed.");
    }
    public override IEnumerator WriteToLog(string database, string table, Dictionary<string, string> argsNValues) {
        Debug.Log("database: " + database + " ; " + table + " ; " + argsNValues.ToString());
        yield return null;
    }


    public override IEnumerator GetFromLog(string database, string table, Func<string, int> yieldedReactionToGet) {
        Debug.Log("database: " + database + " ; " + table + " ; ");
        yield return null;
    }

    public override IEnumerator EndLogs()
    {
        Debug.Log("Log Closed.");
        yield return null;
    }
}

