using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TPFSDK
{
    public class TPFSdkEnvironment
    {
        Dictionary<string, object> jsonObject;
        string jsonStr = "{}";

        public TPFSdkEnvironment(string json)
        {
            Debug.Log(string.Format("new TPFSdkEnvironment {0}", json));
            if (json != null && json != "")
            {
                jsonStr = json;
            }

            jsonObject = MiniJSON.Json.Deserialize(jsonStr) as Dictionary<string, object>;
            if (jsonObject == null)
            {
                Debug.LogError("new TPFSdkEnvironment.jsonObject==null");
                jsonObject = new Dictionary<string, object>();
            }
        }

        public string environmentVariableConfig { get { return jsonStr; } }
        public object this[string key]
        {
            get
            {
                if (jsonObject.ContainsKey(key))
                {
                    return jsonObject[key];
                }
                return null;
            }
        }

        public string tpfProxyUrl { get { return this["tpfProxyUrl"] as string; } }
        public string logLevel { get { return this["logLevel"] as string; } }

    }
}