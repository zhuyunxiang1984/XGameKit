using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BundleData
{
    public enum EnumState
    {
        None = 0,
        WaitToLoad, //等待加载
        Loading,    //加载中
        Completed,  //加载完成
    }
    private EnumState _state;
    private AssetBundle _loadedBundle;
    private int _referenceCount;

    public void SetState(EnumState state)
    {
        _state = state;
    }
    public EnumState GetState()
    {
        return _state;
    }
    public void SetBundle(AssetBundle bundle)
    {
        _loadedBundle = bundle;
    }
    public AssetBundle Retain()
    {
        ++_referenceCount;
        return _loadedBundle;
    }
    public void Release()
    {
        --_referenceCount;
    }
    public int GetReferenceCount()
    {
        return _referenceCount;
    }



}
