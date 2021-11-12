using System;
using System.Collections.Generic;
using Msg;
using ProtoBuf;
using TPFSDK;
using UnityEngine;

public class MsgHandleMgr : MonoSingleton<MsgHandleMgr>
{
    //回调
    public delegate void ServerResponseDelegate(IExtensible res);

    //回调函数字典
    private Dictionary<string, ServerResponseDelegate>
        m_msgHandleDic = new Dictionary<string, ServerResponseDelegate>();
    
    //消息队列
    private Queue<MsgData> m_msgQueue = new Queue<MsgData>();
    
    //消息集合,判重
    private HashSet<string> m_msgSet = new HashSet<string>();
    
    //检查错误
    private bool m_isCheckError = false;

    //是否有消息
    public bool HaveMsg()
    {
        return m_msgQueue.Count > 0;
    }
    
    //添加消息到消息字典
    public void AddMsg(IExtensible msg, bool canRepeat = false)
    {
        var msgId = msg.GetType().Name;

        if (m_msgSet.Contains(msgId) && !canRepeat)
        {
            return;
        }

        var msgData = new MsgData(msgId, msg);

        m_msgQueue.Enqueue(msgData);
        m_msgSet.Add(msgId);
    }
    
    //添加消息到消息字典  非标准的msgId  一般是和其他部门特殊定义的 例如激活码相关
    public void AddMsg(string msgId, IExtensible msg)
    {
        if (m_msgSet.Contains(msgId))
        {
            return;
        }

        var msgData = new MsgData(msgId, msg);
        
        m_msgQueue.Enqueue(msgData);
        m_msgSet.Add(msgId);
    }

    //注册回调
    public void RegisterHandle(string msgId,ServerResponseDelegate msgDelegate)
    {
        m_msgHandleDic[msgId] = msgDelegate;
    }

    //处理消息
    public void HandleMsg(string msgId, IExtensible res)
    {
        //合法性判断
        if (!IsValid(msgId))
        {
            Debug.LogError($"handle msg error msgId:{msgId}");
            return;
        }

        //handle判断
        if (!m_msgHandleDic.TryGetValue(msgId, out var msgDelegate))
        {
            Debug.LogError($"msg {msgId} don't have handle");
            return;
        }
        
        msgDelegate.Invoke(res);
        
        //出队
        var msg = m_msgQueue.Dequeue();
        msg.ResetState();
        m_msgSet.Remove(msgId);
    }
    
    //处理消息错误
    public void HandleMsgError(string msgId,IExtensible msg)
    {
        if (!IsValid(msgId))
        {
            Debug.LogError($"handle error msg error msgId:{msgId}");
            return;
        }

        //重置发送状态
        var msgData = m_msgQueue.Peek();
        msgData.ResetState();

        if (msg == null)
        {
            RetryTips(msgData, $"time out: GameLoopMgr.Instance.TimeStamp");
            return;
        }

        //显示错误码
        if (msg is ServerErrorMsg errorMsg)
        {
            Debug.LogError(errorMsg.Info + errorMsg.errorCode);

            switch ((ServerErrorMsg.ErrorType)errorMsg.errorCode)
            {
                //未激活
                case ServerErrorMsg.ErrorType.NoActivation:
                {
                    //NotActiveTips();
                    break;
                }
                //未开服
                case ServerErrorMsg.ErrorType.NoOpenServer:
                {
                    //NotOpenServerTips();
                    break;
                }
                //token错误,需要重新登录
                case ServerErrorMsg.ErrorType.Error:
                {
                    TokenErrorTips();
                    break;
                }

                default:
                {
                    RetryTips(msgData, errorMsg.Info);
                    break;
                }
            }
        }
    }

    //重发
    private void RetryTips(MsgData msgData,string errorMsg)
    {
        //消息可以继续重发,不提示
        if (msgData.CanSend)
        {
            return;
        }
        
        m_isCheckError = true;

        var descText = "TextConfigMgr.Instance.GetText(TextType.TIPS_TEXT_NET_ERROR)";
        var leftText = "TextConfigMgr.Instance.GetText(TextType.TIPS_TEXT_BACK_START)";
        var rightText = "TextConfigMgr.Instance.GetText(TextType.TIPS_TEXT_RETRY_CONNECT)";

        //需要配置文件才开放
        if (Debug.unityLogger.filterLogType == LogType.Log)
        {
            descText += errorMsg;
        }

        //弹出错误提示
        //GameUtils.ShowPopUI(descText, leftText, rightText
        //    ,
        //    () =>
        //    {
                m_msgQueue.Clear();
                m_msgSet.Clear();
                    
        //        UIMgr.OpenPanel<LoadingUI>(UILevel.Forward,
        //            new LoadingUIData() {LoadingAction = eLoadingAction.BackStart});
                    
        //        CommonPopUI.Close();
                    
                m_isCheckError = false;
                    
        //    }, () =>
        //    {
        //        msgData.Retry();
        //        m_isCheckError = false;
        //        CommonPopUI.Close();

        //    }, true);
    }

    //token失效
    private void TokenErrorTips()
    {
        m_isCheckError = true;
        
        //var desc = "111";
        //var btnText = "222";

        Debug.LogError("token失效");

        //弹出错误提示
        //GameUtils.ShowPopUI(desc, btnText,
        //    () =>
        //    {
                m_msgQueue.Clear();
                m_msgSet.Clear();

        //        UIMgr.OpenPanel<LoadingUI>(UILevel.Forward,
        //            new LoadingUIData() {LoadingAction = eLoadingAction.BackStart});

        //        CommonPopUI.Close();
                m_isCheckError = false;
        //    }, false);
    }

    //是否是合法消息
    private bool IsValid(string msgId)
    {
        if (m_msgQueue.Count == 0)
        {
            return false;
        }
        
        var msg = m_msgQueue.Peek();

        return msg.m_msgId.Equals(msgId);
    }

    //检查和发送消息
    private void CheckSendMsg()
    {
        //错误检查中
        if (m_isCheckError)
        {
            return;
        }
        
        //是否有待发送的消息
        if (m_msgQueue.Count == 0)
        {
            return;
        }

        var msgData = m_msgQueue.Peek();

        //在等待
        if (msgData.waitingRes)
        {
            return;
        }
        
        //发送
        msgData.Send();
    }

    private void Update()
    {
        CheckSendMsg();
    }
}

//消息数据
public class MsgData
{
    //是否在等待回复
    public bool waitingRes { private set; get; }
    
    //消息ID
    public string m_msgId { private set; get; }

    //是否可以继续尝试
    public bool CanSend => m_remainCnt > 0;
    
    //消息体
    private IExtensible m_msg;

    //剩余尝试次数
    private int m_remainCnt;
    
    //目标url
    private string m_url;
    
    //目标service
    private string m_serice;
    
    //尝试次数
    private readonly int retryCnt = 1;

    //初始化
    public MsgData(string msgId,IExtensible msg)
    {
        m_msgId = msgId;
        
        m_msg = msg;
        
        m_remainCnt = retryCnt;

        waitingRes = false;

        //地址 发到服务器的地址
        m_url = "http://127.0.0.1:31506";
        //m_url = "127.0.0.1:31506";
        
        //服务名
        m_serice = "world-service:31001";
    }

    //发送
    public void Send()
    {
        m_remainCnt--;

        waitingRes = true;

        ClientNetMgr.Instance.SendRequest(m_msgId, m_msg, m_url, m_serice);

        //CommonNetMgr.Instance.RecordSendTime();

        //GameUtils.SetRequestState(true);
    }
    
    //重试
    public void Retry()
    {
        m_remainCnt = retryCnt;

        waitingRes = false;
    }
    
    //重置状态
    public void ResetState()
    {
        waitingRes = false;

        //GameUtils.SetRequestState(false);
    }
}
