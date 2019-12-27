using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Debug log manager
public class MongoDBLogManager : LogManager
{

    MonoBehaviour monoBehaviourObject;
    private string myApiKey;

    Hashtable postHeader;
    private List<PendingCall> pendingCalls;
    private UnityWebRequestAsyncOperation currRequest;
    

    private struct PendingCall
    {
        public UnityWebRequest www;
        public Func<string, int> yieldedReaction;
        public PendingCall(UnityWebRequest www, Func<string, int> yieldedReaction)
        {
            this.yieldedReaction = yieldedReaction;
            this.www = www;
            www.SetRequestHeader("Content-Type", "application/json"); //in order to be recognized by the mongo server
        }
    }
    private IEnumerator ExecuteCall(UnityWebRequestAsyncOperation currConnection, PendingCall call)
    {
        //Debug.Log("number of pending calls: " + pendingCalls.Count);
        yield return currConnection;
        currConnection = call.www.SendWebRequest();
        yield return currConnection;
        Debug.Log("remote call error code returned (no return means no error): "+ call.www.error);
        if (call.yieldedReaction != null)
        {
            call.yieldedReaction(call.www.downloadHandler.text);
        }
        pendingCalls.Remove(call);
        yield return currConnection;
    }

    public override void InitLogs(MonoBehaviour monoBehaviourObject)
    {
        this.monoBehaviourObject = monoBehaviourObject;
        myApiKey = "skgyQ8WGQIP6tfmjytmcjzlgZDU2jWBD";
        pendingCalls = new List<PendingCall>();
    }

    private UnityWebRequest ConvertEntityToPostRequest(Dictionary<string,string> entity, string database, string collection)
    {
        string url = "https://api.mlab.com/api/1/databases/" + database + "/collections/" + collection + "?apiKey=" + myApiKey;

        string entityJson = StringifyDictionaryForLogs(entity);
        byte[] formData = System.Text.Encoding.UTF8.GetBytes(entityJson);
        UnityWebRequest www = UnityWebRequest.Post(url, entityJson);
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(formData);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        return www;
    }
    private UnityWebRequest ConvertEntityToGetRequest(string database, string collection, string query)
    {
        string url = "https://api.mlab.com/api/1/databases/" + database + "/collections/" + collection + "?apiKey=" + myApiKey + query;
        UnityWebRequest www = UnityWebRequest.Get(url);
        return www;
    }
    private UnityWebRequest ConvertEntityToPutRequest(System.Object entity, string database, string collection, string query)
    {
        string url = "https://api.mlab.com/api/1/databases/" + database + "/collections/" + collection + "?apiKey=" + myApiKey + query;

        string entityJson = JsonUtility.ToJson(entity);
        UnityWebRequest www = UnityWebRequest.Put(url, entityJson);
        return www;
    }


    public override IEnumerator WriteToLog(string database, string table, Dictionary<string,string> argsNValues)
    {
        PendingCall call = new PendingCall(ConvertEntityToPostRequest(argsNValues, database, table), null);
        pendingCalls.Add(call);
        yield return monoBehaviourObject.StartCoroutine(ExecuteCall(currRequest, call));
        pendingCalls.Remove(call);
    }

    public override IEnumerator GetFromLog(string database, string table, Func<string, int> yieldedReactionToGet)
    {
        Debug.Log("database: " + database + " ; " + table + " ; ");
        yield return null;
    }

    //public override IEnumerator GetLastSessionConditionFromLog(Func<string, int> yieldedReactionToGet)
    //{
    //   string query = "&s={\"_id\": -1}&l=1"; //query which returns the last game result
    //   PendingCall call = new PendingCall(ConvertEntityToGetRequest(databaseName, "gameresultslog", query), 
    //        delegate (string lastGameEntry){
    //            string lastConditionString = "";

    //            lastGameEntry = "{ \"results\": " + lastGameEntry + "}";
    //            DataEntryGameResultLogQueryResult lastGameEntriesObject = JsonUtility.FromJson<DataEntryGameResultLogQueryResult>(lastGameEntry);
    //            if (lastGameEntriesObject.results.Count > 0)
    //            {
    //                lastConditionString = ((DataEntryGameResultLog)(lastGameEntriesObject.results[lastGameEntriesObject.results.Count - 1])).condition;
    //            }
    //            yieldedReactionToGet(lastConditionString);
    //            return 0;
    //        }
    //    );
    //    pendingCalls.Add(call);
    //    yield return GameGlobals.monoBehaviourFunctionalities.StartCoroutine(ExecuteCall(currRequest, call));
    //    pendingCalls.Remove(call);
    //}

    public override IEnumerator EndLogs()
    {
        while (pendingCalls.Count > 0)
        {
            yield return null;
        }
        Debug.Log("Log Closed.");
    }
}

