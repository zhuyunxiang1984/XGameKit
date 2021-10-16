using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;

public class ResourceDescription
{
    private Dictionary<string, List<string>> _dictBundleOfDependencies = new Dictionary<string, List<string>>();
    
    private Dictionary<string, List<string>> _dictBundleOfAssetList = new Dictionary<string, List<string>>();
    private Dictionary<string, string> _dictAssetOfBundle = new Dictionary<string, string>();
    
    private Dictionary<string, List<string>> _dictAtlasOfSpriteList = new Dictionary<string, List<string>>();
    private Dictionary<string, string> _dictSpriteOfAtlas = new Dictionary<string, string>();

    public new string ToString()
    {
        var content = string.Empty;
        content += "---ResourceDescription--------------------------------------- \n";
        content += "---BundleOfDependencies--- \n";
        foreach (var pairs in _dictBundleOfDependencies)
        {
            content += $"{pairs.Key} \n";
            foreach (var value in pairs.Value)
            {
                content += $"  |__{value} \n";
            }
        }
        content += "---BundleOfAssetList--- \n";
        foreach (var pairs in _dictBundleOfAssetList)
        {
            content += $"{pairs.Key} \n";
            foreach (var value in pairs.Value)
            {
                content += $"  |__{value} \n";
            }
        }
        content += "---AtlasOfSpriteList--- \n";
        foreach (var pairs in _dictAtlasOfSpriteList)
        {
            content += $"{pairs.Key} \n";
            foreach (var value in pairs.Value)
            {
                content += $"  |__{value} \n";
            }
        }
        return content;
    }

    public void Reset()
    {
        _dictBundleOfDependencies.Clear();
        
        _dictBundleOfAssetList.Clear();
        _dictAssetOfBundle.Clear();
        
        _dictAtlasOfSpriteList.Clear();
        _dictSpriteOfAtlas.Clear();
    }

    public void AddDependencies(string bundleName, List<string> dependencies)
    {
        if (_dictBundleOfDependencies.ContainsKey(bundleName))
        {
            XDebug.LogError($"AddDependency {bundleName} 已经存在");
            return;
        }
        _dictBundleOfDependencies.Add(bundleName, dependencies);
    }
    
    public void AddAsset(string assetName, string bundleName)
    {
        if (_dictAssetOfBundle.ContainsKey(assetName))
        {
            XDebug.LogError($"AddAsset {assetName} 已经存在 bundleName:{_dictAssetOfBundle[assetName]}");
            return;
        }
        _dictAssetOfBundle.Add(assetName, bundleName);
    }
    
    public void AddSprite(string spriteName, string atlasName)
    {
        if (_dictSpriteOfAtlas.ContainsKey(spriteName))
        {
            XDebug.LogError($"AddSprite {spriteName} 已经存在 atlasName:{_dictSpriteOfAtlas[atlasName]}");
            return;
        }
        _dictSpriteOfAtlas.Add(spriteName, atlasName);
    }
    
    public string GetBundleByAsset(string assetName)
    {
        if (_dictAssetOfBundle.ContainsKey(assetName))
            return _dictAssetOfBundle[assetName];
        return string.Empty;
    }
    public List<string> GetDependencies(string bundleName)
    {
        if (_dictBundleOfDependencies.ContainsKey(bundleName))
            return _dictBundleOfDependencies[bundleName];
        return null;
    }

    public string GetAtlasBySprite(string spriteName)
    {
        if (_dictSpriteOfAtlas.ContainsKey(spriteName))
            return _dictSpriteOfAtlas[spriteName];
        return string.Empty;
    }

    public byte[] Serialize()
    {
        OnSerialize();
        var writer = new XStreamBinaryWriter();

        WriteData(writer, _dictBundleOfDependencies);
        WriteData(writer, _dictBundleOfAssetList);
        WriteData(writer, _dictAtlasOfSpriteList);
        
        return writer.GetData();
    }

    public void Deserialize(byte[] data)
    {
        Reset();
        var reader = new XStreamBinaryReader();
        reader.SetBytes(data);
        
        ReadData(reader, _dictBundleOfDependencies);
        ReadData(reader, _dictBundleOfAssetList);
        ReadData(reader, _dictAtlasOfSpriteList);

        OnDeserialize();
    }

    private void OnSerialize()
    {
        ConvertData(_dictAssetOfBundle, _dictBundleOfAssetList);
        ConvertData(_dictSpriteOfAtlas, _dictAtlasOfSpriteList);
    }

    private void OnDeserialize()
    {
        ConvertData(_dictBundleOfAssetList, _dictAssetOfBundle);
        ConvertData(_dictAtlasOfSpriteList, _dictSpriteOfAtlas);
    }

    private void ConvertData(Dictionary<string, List<string>> from, Dictionary<string, string> to)
    {
        to.Clear();
        foreach (var pairs in from)
        {
            var key = pairs.Key;
            var list = pairs.Value;

            foreach (var value in list)
            {
                if (to.ContainsKey(key))
                {
                    continue;
                }
                to.Add(value, key);
            }
        }
    }
    private void ConvertData(Dictionary<string, string> from, Dictionary<string, List<string>> to)
    {
        to.Clear();
        foreach (var pairs in from)
        {
            var key = pairs.Key;
            var value = pairs.Value;

            if (to.ContainsKey(value))
            {
                to[value].Add(key);
            }
            else
            {
                to.Add(value, new List<string>() {key});
            }
        }
    }

    private void WriteData(XStreamBinaryWriter writer, Dictionary<string, List<string>> data)
    {
        var dataCount = (short) data.Count;
        writer.WriteInt16(dataCount);
        foreach (var pairs in data)
        {
            var key = pairs.Key;
            var list = pairs.Value;
            var count = (short) list.Count;
            
            writer.WriteString(key);
            writer.WriteInt16(count);
            for (int i = 0; i < count; ++i)
            {
                writer.WriteString(list[i]);
            }

        }
    }

    private void ReadData(XStreamBinaryReader reader, Dictionary<string, List<string>> data)
    {
        var dataCount = reader.ReadInt16();
        for (int i = 0; i < dataCount; ++i)
        {
            var key = reader.ReadString();
            var count = reader.ReadInt16();

            List<string> list = null;
            if (data.ContainsKey(key))
            {
                list = data[key];
            }
            else
            {
                list = new List<string>();
                data.Add(key, list);
            }
            for (int j = 0; j < count; ++j)
            {
                var value = reader.ReadString();
                list.Add(value);
            }
        }
    }
}
