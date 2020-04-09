using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace XGameKit.Core
{
    public static class XUtilities
    {
        //保证路径存在
        public static void MakePathExist(string assetPath)
        {
            var directory = Path.GetDirectoryName(assetPath);
            if (Directory.Exists(directory))
                return;
            Directory.CreateDirectory(directory);
        }
    }
}
