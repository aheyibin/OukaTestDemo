/*******************************************************************
** file: 
** copyright: 
** creater:沉尘 
** date:2017/4/18
** version: 
** des: TPF的传递参数
** use: demo
** modify:             
*******************************************************************/
using System.Collections.Generic;
using System;

namespace TPFSDK
{

    public class TPFSdkInfo
    {
        private Dictionary<string, object> _attMap = null;
        public TPFSdkInfo()
        {
            if (null == _attMap) _attMap = new Dictionary<string, object>();
            //SetData("data_ins_key", ins_key_count.ToStrng());
            //ins_key_count++;
        }

        public TPFSdkInfo(string _in_data)
        {
            if (null == _attMap)
                _attMap = new Dictionary<string, object>();
            JsonToData(_in_data);
            _attMap.Add(TPFParamKey.SDK_JSON_DATA, _in_data);
        }

        /// <summary>
        /// 内部浅拷贝使用
        /// </summary>
        /// <param name="data"></param>
        internal TPFSdkInfo(ref Dictionary<string, object> data)
        {
            _attMap = data;
        }

        internal bool IsEmpty()
        {
            if(_attMap == null || _attMap.Count == 0)
            {
                return true;
            }
            return false;
        }

        internal TPFSdkInfo(int errorCode)
        {
            if (null == _attMap) _attMap = new Dictionary<string, object>();
            SetData(TPFParamKey.ERROR_CODE, errorCode);
        }

        //[XLua.BlackList]
		public void copyData(TPFSdkInfo _in_data)
        {
            JsonToData(_in_data.ToJson());
        }

		//[XLua.BlackList]
		public Dictionary<string, object> attMap()
        {
            return _attMap;
        }

		//[XLua.BlackList]
		public bool ContainsKey(string key)
        {
            return _attMap.ContainsKey(key);
        }


        /**设置一个boolean值*/
        public void SetData(string attName, bool boolValue)
        {
            if (boolValue)
                this.SetData(attName, "1");
            else
                this.SetData(attName, "0");
        }
        /**设置一个int值*/
        public void SetData(string attName, int intValue)
        {
            if (null == _attMap) _attMap = new Dictionary<string, object>();

            _attMap[attName] = intValue;
        }

        /// <summary>
        /// 设置一个string值
        /// </summary>
        public void SetData(string attName, string attValue)
        {
            if (null == _attMap) _attMap = new Dictionary<string, object>();

            if (_attMap.ContainsKey(attName))
                _attMap[attName] = attValue;
            else
                _attMap.Add(attName, attValue);

        }

        //设置一个long
        public void SetData(string attName, long numl)
        {
            if (null == _attMap) _attMap = new Dictionary<string, object>();

            if (_attMap.ContainsKey(attName))
                _attMap[attName] = numl;
            else
                _attMap.Add(attName, numl);
        }

        public void SetData(string attName, List<object> attValue)
        {
            if (null == _attMap) _attMap = new Dictionary<string, object>();

            if (_attMap.ContainsKey(attName))
                _attMap[attName] = attValue;
            else
                _attMap.Add(attName, attValue);
        }
        //设置一个map
        public void SetData(string attName, Dictionary<string,string> soMap)
        {
            if (null == _attMap) _attMap = new Dictionary<string, object>();

            if (_attMap.ContainsKey(attName))
                _attMap[attName] = soMap;
            else
                _attMap.Add(attName, soMap);
        }
        /**
         * GET String Data 
         * 
         */
        public string GetData(string attName)
        {
            if (null == _attMap) _attMap = new Dictionary<string, object>();
            string outStr = "";

            if (_attMap.ContainsKey(attName))
                outStr = _attMap[attName].ToString();

            return outStr;

        }

        public object GetDataObject(string attName)
        {
            if (null == _attMap) return null;
            
            if (_attMap.ContainsKey(attName))
                return _attMap[attName];

            return null;
        }
		/***
         * Get int data
         * 
         */
		//[XLua.BlackList]
		public int GetInt(string attName)
        {
            object obj = GetDataObject(attName);
            return Convert.ToInt32(obj);
        }

        /***
         * get bool data ; 0 is false else is true
         */
        //[XLua.BlackList]
        public bool GetBool(string attName)
        {
            int value = GetInt(attName);
            if (1 == value)
                return true;
            else
                return false;
        }

        public long GetLong(string attName)
        {
            object longObj = GetDataObject(attName);
            return Convert.ToInt64(longObj);
        }
		public string ToJson()
        {
            if (null == _attMap) _attMap = new Dictionary<string, object>();

            string outStr = MiniJSON.Json.Serialize(_attMap);
            return outStr;
        }

		//[XLua.BlackList]
		public void JsonToData(string _in_data)
        {
            if (null == _attMap) _attMap = new Dictionary<string, object>();
            _attMap.Clear();
            if (string.IsNullOrEmpty(_in_data))
                _in_data = "{}";

            _attMap = MiniJSON.Json.Deserialize(_in_data) as Dictionary<string, object>;

        }
    }

}
