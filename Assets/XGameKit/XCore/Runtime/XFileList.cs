using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace XGameKit.Core
{
    [System.Serializable]
    public class XFileList
    {
        public const string Tag = "XFileList";
        public const string FILE_LIST_NAME = "_filelist.json";
#if UNITY_EDITOR
        //创建filelist
        public static bool CreateFileList(string path)
        {
            if (!XUtilities.IsFolder(path) || !Directory.Exists(path))
                return false;
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            if (files.Length < 1)
                return false;
            
            try
            {
                var result = new XFileList();
                foreach (var file in files)
                {
                    if(file.Contains(FILE_LIST_NAME))
                        continue;
                    FileInfo sysFileInfo = new FileInfo(file);
                
                    var fileInfo = new XFileInfo();
                    fileInfo.name = sysFileInfo.Name;
                    fileInfo.path = file.Replace(path + "\\", string.Empty);
                    var fileStream = sysFileInfo.OpenRead();
                    fileInfo.md5 = ToMD5(fileStream);
                    fileStream.Close();
                    fileInfo.length = sysFileInfo.Length;
                    result.m_dictFileInfo.Add(fileInfo.name, fileInfo);
                }
                XDebug.Log(Tag, result.ToLog());
                var json = JsonUtility.ToJson(result);
                var filelistPath = $"{path}/{FILE_LIST_NAME}";
                File.WriteAllText(filelistPath, json);
                return true;
            }
            catch (Exception e)
            {
                XDebug.LogError(Tag, e.ToString());
                
            }
            return false;
        }
#endif
        //读取filelist
        public static XFileList LoadFileList(string path)
        {
            if (!XUtilities.IsFolder(path) || !Directory.Exists(path))
                return null;
            var filelistPath = $"{path}/{FILE_LIST_NAME}";
            try
            {
                var fileData = XUtilities.ReadFile(filelistPath);
                var content = Encoding.UTF8.GetString(fileData);
                var result = JsonUtility.FromJson<XFileList>(content);
                XDebug.Log(Tag, result.ToLog());
                return result;
            }
            catch (Exception e)
            {
                XDebug.LogError(Tag, $"读取文件清单失败 {filelistPath}\n{e.ToString()}");
            }
            return null;
        }
        //保存filelist
        public static bool SaveFileList(string path, XFileList fileList)
        {
            if (!XUtilities.IsFolder(path) || !Directory.Exists(path))
                return false;
            var filelistPath = $"{path}/{FILE_LIST_NAME}";
            try
            {
                var json = JsonUtility.ToJson(fileList);
                XUtilities.SaveFile(filelistPath, json);
                XDebug.Log(Tag, json);
                return true;
            }
            catch (Exception e)
            {
                XDebug.LogError(Tag, $"保存文件清单失败 {filelistPath}\n{e.ToString()}");
            }
            return false;
        }
        
        //生成md5
        public static string ToMD5(FileStream fs)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] array = md5.ComputeHash(fs);
            string text = string.Empty;
            foreach (var tmp in array)
            {
                text += tmp.ToString("x2");
            }
            return text;
        }
        
        [System.Serializable]
        public class XFileInfo
        {
            public string name;
            public string path;
            public string md5;
            public long length;//byte
        }
        [System.Serializable]
        public class XFileInfoDictWrapper : XSerialDictionary<string, XFileInfo>
        {
        }
        [SerializeField]
        protected XFileInfoDictWrapper m_dictFileInfo = new XFileInfoDictWrapper();

        public Dictionary<string, XFileInfo> GetDatas()
        {
            return m_dictFileInfo.datas;
        }
        public XFileInfo GetFileInfo(string name)
        {
            if (m_dictFileInfo.ContainsKey(name))
                return m_dictFileInfo[name];
            return null;
        }
        public void AddFileInfo(string name, XFileInfo fileInfo)
        {
            if (m_dictFileInfo.ContainsKey(name))
            {
                Debug.LogError($"已经存在fileinfo了 {name}");
                return;
            }
            m_dictFileInfo.Add(name, fileInfo);
        }

        public void DelFileInfo(string name)
        {
            if (!m_dictFileInfo.ContainsKey(name))
                return;
            m_dictFileInfo.Remove(name);
        }

        public string ToLog()
        {
            var text = "==文件清单==\n";
            foreach (var fileInfo in m_dictFileInfo.datas.Values)
            {
                text += $"{fileInfo.name}\t{fileInfo.path}\t{fileInfo.md5}\t{fileInfo.length}\n";
            }
            return text;
        }
    }


}
