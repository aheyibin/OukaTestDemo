using System;
using System.IO;
using Msg;
using ProtoBuf;
//using QF;
using TPFSDK;
using UnityEngine;
using UnityEngine.Networking;

public class ClientNetMgr : Singleton<ClientNetMgr>
{
    //包体
    private ClientRouterMsg m_router = new ClientRouterMsg();

    //消息列表
    
    //初始化
    public ClientNetMgr()
    {
        
    }

    //初始化 数据拉取
    public void Init()
    {
        //初始化必须首个
        //CommonNetMgr.Instance.ServerInitReq();
        //CommonNetMgr.Instance.ServerTimeReq();
        //TimeSaveNetMgr.Instance.TimeDataInfoReq(eTimeType.All);
        //MinisterNetMgr.Instance.MinisterInfoReq();
        //AchievementNetMgr.Instance.InfoReq();
        //CloudDataMgr.Instance.InitPlayBackData();
        //HomeNetMgr.Instance.HomeDataReq();
        //GuideNetMgr.Instance.GuideDataReq();
    }

    //发送请求
    public void SendRequest(string msgId, IExtensible msg, string url, string service)
    {
        m_router.Userid = "uid";
        m_router.Token = "token";
        m_router.serviceName = service;
        m_router.channlId = ITPFSdk.Instance.channelId;

        m_router.msgId = msgId;

        m_router.msgContent = msg.Serialize();

        //发出消息
        var webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);

        //貌似没有影响,先注释掉  webRequest.uploadHandler.contentType= "application/x-www-form-urlencoded";
        //webRequest.uploadHandler = new UploadHandlerRaw(m_router.msgContent);
        webRequest.uploadHandler = new UploadHandlerRaw(m_router.Serialize());
        webRequest.downloadHandler = new DownloadHandlerBuffer();

        webRequest.uri = new Uri(url + "/ClientMsgHead");

        //超时时间
        webRequest.timeout = 5;

        Debug.Log("send msgId:" + msgId);
        
        webRequest.SendWebRequest().completed += (result) =>
        {
            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.LogError($"msg: {msgId} error: {webRequest.error}");
                MsgHandleMgr.Instance.HandleMsgError(msgId, null);
                return;
            }

            Debug.LogError("back : " + msgId);
            
            webRequest.downloadHandler.data.Deserialize();
        };
    }
}

//消息扩展类型
public static class NetExtra
{
    //序列化
    public static byte[] Serialize(this IExtensible msg)
    {
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, msg);
            return ms.ToArray();
        }
    }

    //反序列化
    public static void Deserialize(this byte[] bytes)
    {
        try
        {
            ServerResRouterMsg msg;

            using (var ms = new MemoryStream(bytes))
            {
                msg = Serializer.Deserialize<ServerResRouterMsg>(ms);
            }

            if (msg == null)
            {
                return;
            }

            if (msg.msgContent == null)
            {
                MsgHandleMgr.Instance.HandleMsg(msg.msgId, null);
                return;
            }

            var type = Type.GetType($"Msg.{msg.msgType}");

            using (var ms = new MemoryStream(msg.msgContent))
            {
                var msgContext = Serializer.Deserialize(type, ms) as IExtensible;

                if (msg.msgType.Equals(nameof(ServerErrorMsg)))
                {
                    //处理错误消息
                    MsgHandleMgr.Instance.HandleMsgError(msg.msgId, msgContext);
                }
                else
                {
                    //正常处理业务消息
                    MsgHandleMgr.Instance.HandleMsg(msg.msgId, msgContext);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }
}
