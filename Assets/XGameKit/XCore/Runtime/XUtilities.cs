using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XGameKit.Core
{
    public static class XUtilities
    {
        //保证路径存在
        public static void MakePathExist(string fullPath)
        {
            string directory = string.Empty;
            var ext = Path.GetExtension(fullPath);
            if (string.IsNullOrEmpty(ext))
            {
                directory = fullPath;
            }
            else
            {
                directory = Path.GetDirectoryName(fullPath);
            }
            if (Directory.Exists(directory))
                return;
            Directory.CreateDirectory(directory);
        }
    }
}
