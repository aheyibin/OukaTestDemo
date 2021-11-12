using System;
using System.Security.Cryptography;
using System.Text;

namespace TPFSDK.Utils
{
    public static class MD5Utils
    {
        public static void PrintByteArray(byte[] bytes)
        {
            var sb = new StringBuilder("new byte[] { ");
            foreach (var b in bytes)
            {
                sb.Append(b + ", ");
            }
            sb.Append("}");
            //UnityEngine.Debug.Log(sb.ToString());
        }

        public static string GetMD5(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var data = System.Text.Encoding.UTF8.GetBytes(input);
            var md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            var destString = "";
            for (var i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        public static string GetMD5String(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var data = System.Text.Encoding.UTF8.GetBytes(input);
            //UnityEngine.Debug.Log(string.Format("GetMD5GetUTF8[{0}]-[{1}]", input, data));

            PrintByteArray(data);

            var md5Data = md5.ComputeHash(data, 0, data.Length);
            //UnityEngine.Debug.Log(string.Format("GetMD5Get-ComputeHash[{0}]", md5Data));

            md5.Clear();

            var destString = "";
            for (var i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');

            //UnityEngine.Debug.Log(string.Format("GetMD5Get-md5string[{0}]", destString));
            return destString;
        }

        public static byte[] GetMD5Hash(byte[] input)
        {

            var md5 = new MD5CryptoServiceProvider();
            var data = input;//System.Text.Encoding.UTF8.GetBytes(input);
            //UnityEngine.Debug.LogFormat("GetMD5GetUTF8[{0}]-[{1}]", input, data);

            PrintByteArray(data);

            var md5Data = md5.ComputeHash(data, 0, data.Length);
            PrintByteArray(md5Data);
            return md5Data;
        }

        public static byte[] GetMD5Hash(string input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var data = System.Text.Encoding.UTF8.GetBytes(input);
            //UnityEngine.Debug.Log(string.Format("GetMD5GetUTF8[{0}]-[{1}]", input, data));

            PrintByteArray(data);

            var md5Data = md5.ComputeHash(data, 0, data.Length);
            PrintByteArray(md5Data);
            return md5Data;

        }

        public static string GetMD5String(byte[] input)
        {
            var md5 = new MD5CryptoServiceProvider();
            var md5Data = md5.ComputeHash(input, 0, input.Length);

            var destString = "";
            for (var i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');

            return destString;
        }
    }
}