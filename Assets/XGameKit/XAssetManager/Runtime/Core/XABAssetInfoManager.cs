using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XAssetManager
{
    public class XABAssetInfoManager
    {
        //下载列表
        public List<string> ltDownload  { get; protected set; } = new List<string>();
        //删除列表
        public List<string> ltExpiredX  { get; protected set; } = new List<string>();

        public XFileList server_filelist { get; protected set; }
        public XFileList client_filelist { get; protected set; }
        public XFileList stream_filelist { get; protected set; }
        
        protected Dictionary<string, EnumFileLocation> m_dictLocation = new Dictionary<string, EnumFileLocation>();

        public void ClearLocation()
        {
            m_dictLocation.Clear();
        }
        public void SetLocation(string fileName, EnumFileLocation location)
        {
            if (m_dictLocation.ContainsKey(fileName))
                m_dictLocation[fileName] = location;
            else
                m_dictLocation.Add(fileName, location);
        }

        public void SetServerFileList(XFileList filelist)
        {
            server_filelist = filelist;
        }
        public void SetClientFileList(XFileList filelist)
        {
            client_filelist = filelist;
        }
        public void SetStreamFileList(XFileList filelist)
        {
            stream_filelist = filelist;
        }
    }
}