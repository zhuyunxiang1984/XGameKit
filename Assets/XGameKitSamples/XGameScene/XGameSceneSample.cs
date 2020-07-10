using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.GameScene;
using XGameKit.GameSceneSample;
using Sirenix.OdinInspector;

public class XGameSceneSample : MonoBehaviour
{
    [HorizontalGroup, LabelText("场景"), LabelWidth(50)]
    public string SceneName;

    [HorizontalGroup, Button("切换")]
    void TestEnterScene()
    {
        m_sceneManager.EnterScene(SceneName);
    }

    protected XGameSceneManager m_sceneManager;
    private void Start()
    {
        m_sceneManager = new XGameSceneManager();
        m_sceneManager.AddScene(EnumSceneName.scene1, new XGameSceneTest());
        m_sceneManager.AddScene(EnumSceneName.scene2, new XGameSceneTest());
        m_sceneManager.AddScene(EnumSceneName.scene3, new XGameSceneTest());
        m_sceneManager.AddScene(EnumSceneName.scene4, new XGameSceneTest());
        m_sceneManager.AddScene(EnumSceneName.scene5, new XGameSceneTest());
        m_sceneManager.AddScene(EnumSceneName.scene6, new XGameSceneTest());
        m_sceneManager.AddScene(EnumSceneName.scene7, new XGameSceneTest());
        m_sceneManager.AddScene(EnumSceneName.scene8, new XGameSceneTest());
        m_sceneManager.AddScene(EnumSceneName.scene9, new XGameSceneTest());

        m_sceneManager.SetLink(EnumSceneName.scene1, EnumSceneName.scene3);
        m_sceneManager.SetLink(EnumSceneName.scene3, EnumSceneName.scene5);
        m_sceneManager.SetLink(EnumSceneName.scene5, EnumSceneName.scene7);
        m_sceneManager.SetLink(EnumSceneName.scene7, EnumSceneName.scene9);

        m_sceneManager.SetLink(EnumSceneName.scene2, EnumSceneName.scene4);
        m_sceneManager.SetLink(EnumSceneName.scene4, EnumSceneName.scene6);
        m_sceneManager.SetLink(EnumSceneName.scene6, EnumSceneName.scene8);

        m_sceneManager.EnterScene(EnumSceneName.scene1);
    }
    private void Update()
    {
        m_sceneManager.Tick(Time.deltaTime);
    }
}