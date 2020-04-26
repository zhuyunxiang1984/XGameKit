using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.XAssetManager
{
    public enum EnumLocation
    {
        Download, //下载目录
        StreamingAssetsStatic,//StreamingAssets跟包目录
        StreamingAssetsHotfix,//StreamingAssets热更目录
    }
    
    public enum EnumLoadState
    {
        None = 0,//未加载
        Loading, //加载中    
        Done,    //已完成
    }
}