using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XAssetManager;

public class XAssetManagerDemo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        XAssetDescription description = new XAssetDescription();
        //
        description.AddAsset("asset1", "bundle_a");
        description.AddAsset("asset2", "bundle_a");
        description.AddAsset("asset3", "bundle_a");
        
        description.AddAsset("asset4", "bundle_b");
        description.AddAsset("asset5", "bundle_b");
        description.AddAsset("asset6", "bundle_b");
        
        description.AddDependencies("bundle_b", new List<string>(){"bundle_a"});
        
        description.AddSprite("img1", "atlas1");
        description.AddSprite("img2", "atlas1");
        description.AddSprite("img3", "atlas2");

        var data = description.Serialize();
        XDebug.Log("GGYY", description.ToString());

        var directory = $"{Application.dataPath}/../Documents";
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        File.WriteAllText($"{directory}/ResourceDescription.txt", Convert.ToBase64String(data));

        var content = File.ReadAllText($"{directory}/ResourceDescription.txt");
        description.Deserialize(Convert.FromBase64String(content));
        XDebug.Log("GGYY", description.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
