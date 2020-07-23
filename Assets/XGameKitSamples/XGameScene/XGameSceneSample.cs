using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using XGameKit.GameApp;
using XGameKit.GameSceneSample;
using XGameKit.XAssetManager;
using Sirenix.OdinInspector;

public class XGameSceneSample : XGameApp
{
    [HorizontalGroup, LabelText("场景"), LabelWidth(50)]
    public string SceneName;

    [HorizontalGroup, Button("切换")]
    void TestEnterScene()
    {
        EnterScene(SceneName);
    }

    protected override void OnAwake()
    {
        base.OnAwake();

        var assetManager = XService.AddService<XAssetManager>();
        assetManager.Initialize();

    }

    protected override void OnStart()
    {
        base.OnStart();

        AddScene(EnumSceneName.scene1, new XGameSceneTest1());
        AddScene(EnumSceneName.scene2, new XGameSceneTest2());
        AddScene(EnumSceneName.scene3, new XGameSceneTest());
        AddScene(EnumSceneName.scene4, new XGameSceneTest());
        AddScene(EnumSceneName.scene5, new XGameSceneTest());
        AddScene(EnumSceneName.scene6, new XGameSceneTest());
        AddScene(EnumSceneName.scene7, new XGameSceneTest());
        AddScene(EnumSceneName.scene8, new XGameSceneTest8());
        AddScene(EnumSceneName.scene9, new XGameSceneTest9());

        SetLink(EnumSceneName.scene1, EnumSceneName.scene3);
        SetLink(EnumSceneName.scene3, EnumSceneName.scene5);
        SetLink(EnumSceneName.scene5, EnumSceneName.scene7);
        SetLink(EnumSceneName.scene7, EnumSceneName.scene9);

        SetLink(EnumSceneName.scene2, EnumSceneName.scene4);
        SetLink(EnumSceneName.scene4, EnumSceneName.scene6);
        SetLink(EnumSceneName.scene6, EnumSceneName.scene8);

        EnterScene(EnumSceneName.scene1);
    }
}