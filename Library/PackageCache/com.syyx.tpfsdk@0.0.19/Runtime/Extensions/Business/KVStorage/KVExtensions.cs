using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TPFSDK.Utils;
using UnityEngine;

namespace TPFSDK.Business
{

    public static class KVExtensions 
    {
        #region public interface
        public static void KVSaveGameInfo(this ITPFSdk sdk, Dictionary<string, string> data, long version, TPFSdkEventDelegate callback)
        {
            if (!s_IsKVInfoValid)
            {
                KVAuth();
                var command = new KVCommand(KVOperation.SAVE, data, version, callback);
                EnqueueKVCommand(command);
                return;
            }

            var request = new KVSaveRequest(data, version, callback);
            request.Post();
        }

        public static void KVBatchSaveGameInfo(this ITPFSdk sdk, Dictionary<string, string> data, long version, TPFSdkEventDelegate callback)
        {
            if (!s_IsKVInfoValid)
            {
                KVAuth();
                var command = new KVCommand(KVOperation.BATCH_SAVE, data, version, callback);
                EnqueueKVCommand(command);
                return;
            }

            var request = new KVBatchSaveRequest(data, version, callback);
            request.Post();
        }

        public static void KVQueryGameInfo(this ITPFSdk sdk, List<string> data, TPFSdkEventDelegate callback)
        {
            if (!s_IsKVInfoValid)
            {
                KVAuth();
                var command = new KVCommand(KVOperation.QUERY, data, 0, callback);
                EnqueueKVCommand(command);
                return;
            }

            var request = new KVQueryRequest(data, callback);
            request.Post();
        }

        public static void KVRemoveGameInfo(this ITPFSdk sdk, List<string> data, TPFSdkEventDelegate callback)
        {
            if (!s_IsKVInfoValid)
            {
                KVAuth();
                var command = new KVCommand(KVOperation.REMOVE, data, 0, callback);
                EnqueueKVCommand(command);
                return;
            }

            var request = new KVRemoveRequest(data, callback);
            request.Post();
        }
        #endregion

        #region implementation details
        internal const string KV_STORAGE_PROVIDERID      = "700023";
        internal const string KV_STORAGE_VERIFY_MSGID    = "700023001";
        internal const string KV_STORAGE_ONCESAVE_MSGID  = "700023002";
        internal const string KV_STORAGE_BATCHSAVE_MSGID = "700023004";
        internal const string KV_STORAGE_QUERY_MSGID     = "700023003";
        internal const string KV_STORAGE_QUERYALL_MSGID  = "700023005";
        internal const string KV_STORAGE_REMOVE_MSGID    = "700023006";

        internal enum TPFKVStorageCode
        {
            /// <summary>
            /// 操作成功
            /// </summary>
            Success = 200,

            /// <summary>
            /// token错误
            /// </summary>
            SystemError = 1040,
        }

        internal enum KVOperation 
        {
            SAVE, BATCH_SAVE, QUERY, REMOVE,
        }

        internal class KVCommand 
        {
            public KVCommand(KVOperation operation, object data, long version, TPFSdkEventDelegate cb)
            {
                this.operation = operation;
                this.data = data;
                this.version = version;
                this.cb = cb;
            } 

            public KVOperation operation;
            public object data;
            public long version;
            public TPFSdkEventDelegate cb;
        }
        private static List<KVCommand> s_KVCommands = new List<KVCommand>();

        private static bool s_IsKVInfoValid = false;
        private static string s_KVToken = string.Empty;
        private static Dictionary<string, object> s_KVDataHeader = new Dictionary<string, object>();
        
        static KVExtensions()
        {
            CoreExtensions.Subscribe(CoreExtensions.CoreEvent.LOGIN, Invalidate);
        }

        private static void Invalidate()
        {
            s_IsKVInfoValid = false;
        }

        internal static void UpdateKVInfo(Dictionary<string, object> header, string kvToken)
        {
            s_KVDataHeader = header;
            s_KVToken = kvToken;
            s_IsKVInfoValid = true;
        }

        internal static void EnqueueKVCommand(KVCommand command) 
        {
            s_KVCommands.Add(command);
        }

        /// <summary>
        /// 鉴权失败时，通知所有鉴权时缓存起来的所有 kv 操作失败
        /// </summary>
        /// <param name="data"></param>
        internal static void NotifyAllQueuedCommands(Dictionary<string, object> data)
        {
            for(int i = 0; i < s_KVCommands.Count; i++)
            {
                var command = s_KVCommands[i];
                // 这里是鉴权失败时的返回，里面应该只包含 errCode 和 errMsg 可以放心浅拷贝
                var copyData = new Dictionary<string, object>(data);
                command.cb.BusinessInvoke(ref copyData);
            }
            s_KVCommands.Clear();
        }

        /// <summary>
        /// 鉴权成功时，执行鉴权时缓存起来的所有 kv 操作
        /// </summary>
        internal static void ExcuteAllQueuedCommands() 
        {
            for (int i = 0; i < s_KVCommands.Count; i++)
            {
                var command = s_KVCommands[i];
                DispatchKVCommand(command);
            }
            s_KVCommands.Clear();
        }

        private static void DispatchKVCommand(KVCommand command)
        {
            switch (command.operation) 
            {
                case KVOperation.SAVE:
                    KVSaveGameInfo(ITPFSdk.Instance, (Dictionary<string, string>)command.data, command.version, command.cb);
                    break;
                case KVOperation.BATCH_SAVE:
                    KVBatchSaveGameInfo(ITPFSdk.Instance, (Dictionary<string, string>)command.data, command.version, command.cb);
                    break;
                case KVOperation.QUERY:
                    KVQueryGameInfo(ITPFSdk.Instance, (List<string>)command.data, command.cb);
                    break;
                case KVOperation.REMOVE:
                    KVRemoveGameInfo(ITPFSdk.Instance, (List<string>)command.data, command.cb);
                    break;
                default:
                    Debug.LogError($"[TPFSDK] Unknown kv operation {command.operation}");
                    break;
            }
        }

        internal static void GetKVHttpRspMeta(IDictionary<string, object> json_dict, out int errCode, out string errMsg)
        {
            var meta = (Dictionary<string, object>)json_dict["meta"];
            errCode = meta.ContainsKey("code") ? Convert.ToInt32(meta["code"]) : 0;
            errMsg = meta.ContainsKey("msg") ? (string)meta["msg"] : "";
        }

        internal static Dictionary<string, object> WarpKVBody(string bodyKey, object bodyValue)
        {
            if (string.IsNullOrEmpty(bodyKey))
            {
                return new Dictionary<string, object>{{ "header", s_KVDataHeader }};
            }
            Dictionary<string, object> body = new Dictionary<string, object>
            {
                { "header", s_KVDataHeader },
                { bodyKey , bodyValue      }
            };
            return body;
        }

        private static void KVAuth() 
        {
            var request = new KVAuthRequest();
            request.Post();
        }

        #endregion
    }

}
