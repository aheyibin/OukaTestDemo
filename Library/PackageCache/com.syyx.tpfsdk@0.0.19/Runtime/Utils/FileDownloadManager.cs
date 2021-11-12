using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace TPFSDK.Utils
{
    public class FileDownloadManager : Singleton<FileDownloadManager>
    {
        //public static FileDownloadManager Instance;

        public bool FileExist(string path)
        {
            return File.Exists(path);
        }

        public bool CheckFileByMD5(string path, string md5)
        {
            if (FileExist(path))
            {
                var data = File.ReadAllBytes(path);
                return md5 == MD5Utils.GetMD5String(data);
            }

            return false;
        }

#if UNITY_2018_1_OR_NEWER
        public class BypassCertificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                //Simply return true no matter what
                return true;
            }
        }
#endif

        public void DownLoadFile(string url, string save, Action<float, string, string, string> onDownLoadResult, string md5 = null)
        {
            DownLoadTask task;
            if (tasks.TryGetValue(url, out task))
            {
                task.callback += onDownLoadResult;
            }
            else
            {
                task = new DownLoadTask()
                {
                    url = url,
                    savePath = save,
                    md5 = md5,
                    callback = onDownLoadResult,
                };

                var uwr = UnityWebRequest.Get(url);

#if UNITY_2018_1_OR_NEWER
                uwr.certificateHandler = new BypassCertificate();
#endif
                task.op = uwr.SendWebRequest();

                tasks.Add(url, task);
            }
        }


        struct DownLoadTask
        {
            public string url;
            public string savePath;
            public string md5;
            public Action<float, string, string, string> callback;
            public UnityWebRequestAsyncOperation op;
        }

        Dictionary<string, DownLoadTask> tasks = new Dictionary<string, DownLoadTask>();

        List<DownLoadTask> removedTask = new List<DownLoadTask>();

        private void Update()
        {
            foreach (var task in tasks.Values)
            {
                if (task.op.webRequest.isNetworkError || task.op.webRequest.isHttpError)
                {
                    task.callback(-1, task.url, task.savePath, "");
                    removedTask.Add(task);
                }
                else if (task.op.progress >= 1.0f || task.op.isDone || task.op.webRequest.isDone)
                {
                    var binData = task.op.webRequest.downloadHandler.data;
                    var md5 = MD5Utils.GetMD5String(binData);

                    if (string.IsNullOrEmpty(task.md5) && task.md5 != md5)
                    {
                        //Debug.LogError($"{task.url}-{task.savePath}-{task.md5} md5 check error! \n The real md5 is {md5}.");
                        Debug.LogErrorFormat("{0}-{1}-{2} md5 check error! \n The real md5 is {3}."
                            , task.url, task.savePath, task.md5, md5);

                        task.callback(-1, task.url, task.savePath, md5);
                        removedTask.Add(task);
                    }
                    else
                    {
                        var dir = Path.GetDirectoryName(task.savePath);

                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }

                        try
                        {
                            using (var fs = new FileStream(task.savePath, FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(binData, 0, binData.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception caught in process: {0}", ex);
                        }

                        task.callback(1, task.url, task.savePath, md5);
                        removedTask.Add(task);
                    }

                }
                else
                {
                    task.callback(task.op.progress, task.url, task.savePath, "");
                }

            }

            foreach (var task in removedTask)
            {
                tasks.Remove(task.url);
            }

            removedTask.Clear();
        }
    }
}
