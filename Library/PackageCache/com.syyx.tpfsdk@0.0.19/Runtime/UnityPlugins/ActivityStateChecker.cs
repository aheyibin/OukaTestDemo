using System;
using System.Collections;
using System.Collections.Generic;
using TPFSDK.Utils;
using UnityEngine;

namespace TPFSDK
{
    public class ActivityStateChecker : MonoBehaviour
    {
        static ActivityStateChecker _instance;
        public static ActivityStateChecker Instance
        {
            get
            {
                if (_instance == null)
                {
                    var go = new GameObject("ActivityStateChecker");
                    _instance = go.AddComponent<ActivityStateChecker>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }


        class activInfo
        {
            public int appId;
            public int id;
            public string roleId;
            public string serverId;
            public Action<string> callback;
            public float interval;
            public float timer;



            string _jsonString;
            public string jsonString
            {
                get
                {
                    if (_jsonString == null)
                    {
                        var jsonDic = new Dictionary<string, object>()
                        {
                            {"appId", appId},
                            {"id", id},
                            {"roleId", roleId},
                            {"serverId", serverId},
                        };

                        _jsonString = MiniJSON.Json.Serialize(jsonDic);
                    }
                    return _jsonString;
                }
            }
        }

        Dictionary<string, activInfo> activInfoMap = new Dictionary<string, activInfo>();


        string appId = "";
        string appKey = "";
        string providerId = "700011";
        string appSecret = "";
        string msgId = "7013";
        string tpfProxyUrl = "";

        string initSession = System.Guid.NewGuid().ToString();
        public void Init(string appId, string appKey, string appSecret, string tpfProxyUrl)
        {
            this.appId = appId;
            this.appKey = appKey;
            this.appSecret = appSecret;
            this.tpfProxyUrl = tpfProxyUrl;

            initSession = System.Guid.NewGuid().ToString();
            activInfoMap.Clear();
        }

        const float defualtInterval = 60.0f;

        public void RegisterStateNotifyCallback(int appId, int id, string roleId, string serverId, Action<string> callback, float checkInterval = 60.0f)
        {
            var key = string.Format("{0}-{1}-{2}-{3}", appId, id, roleId, serverId);
            Debug.LogError("RegisterStateNotifyCallback[" + key + "]");
            activInfoMap[key] = new activInfo()
            {
                appId = appId,
                id = id,
                roleId = roleId,
                serverId = serverId,
                callback = callback,
                interval = checkInterval,
                timer = 0.0f,
            };
        }

        public void UnregisterStateNotifyCallback(int appId, int id, string roleId, string serverId)
        {
            var key = string.Format("{0}-{1}-{2}-{3}", appId, id, roleId, serverId);
            if (activInfoMap.ContainsKey(key))
            {
                activInfoMap.Remove(key);
            }
        }

        public void CheckState(int appId, int id, string roleId, string serverId, Action<string> callback)
        {

            var key = string.Format("{0}-{1}-{2}-{3}", appId, id, roleId, serverId);
            Debug.LogError("CheckState[" + key + "]");

            var jsonDic = new Dictionary<string, object>()
                        {
                            {"appId", appId},
                            {"id", id},
                            {"roleId", roleId},
                            {"serverId", serverId},
                        };
            var bodyJson = MiniJSON.Json.Serialize(jsonDic);
            var header = TPFHttpUtils.GetTPFHttpHeader(this.appId, appKey, providerId, appSecret, msgId, bodyJson);

            var lastInitSession = initSession;

            Debug.LogError("CheckState: " + bodyJson);
            TPFHttpUtils.Post(tpfProxyUrl, header, bodyJson, (code, data) =>
            {
                if (callback == null || lastInitSession != initSession)
                    return;

                if (code != 200)
                {
                    Debug.LogError("CheckState callback return code:" + code);
                }
                else
                {
                    var json = System.Text.Encoding.UTF8.GetString(data);
                    Debug.LogError("CheckAcitivityState result: " + json);
                    callback(json);
                }
            });
        }

        private void Start()
        {
            //InvokeRepeating("CheckAcitivityState", 0.0f, 5.0f);
        }

        private void Update()
        {
            CheckAcitivityState();
            //TPFSDK.Timer.ReTimerManager.Instance.Dispatch();
        }

        void CheckAcitivityState()
        {
            foreach (var info in activInfoMap.Values)
            {
                info.timer += Time.unscaledDeltaTime;
                if (info.timer < info.interval)
                {
                    continue;
                }

                info.timer = 0;
                var bodyJson = info.jsonString;
                var header = TPFHttpUtils.GetTPFHttpHeader(appId, appKey, providerId, appSecret, msgId, bodyJson);
                var callback = info.callback;
                var lastInitSession = initSession;

                Debug.LogError("CheckState: " + bodyJson);
                TPFHttpUtils.Post(tpfProxyUrl, header, bodyJson, (code, data) =>
                {

                    if (callback == null || lastInitSession != initSession)
                        return;

                    if (code != 200)
                    {
                        Debug.LogError("CheckState callback return code:" + code);
                    }
                    else
                    {
                        var json = System.Text.Encoding.UTF8.GetString(data);
                        Debug.LogError("CheckAcitivityState result: " + json);
                        callback(json);
                    }
                });
            }
        }
    }
}
