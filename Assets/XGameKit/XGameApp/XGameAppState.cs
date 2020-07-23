using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using XGameKit.Core;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace XGameKit.GameApp
{
    //状态定义
    public enum EnumGameAppState
    {
        None = 0,
        ChangeScene,  //切换场景准备
        LeaveCurrScene, //退出场景链
        HandleAssets,   //处理场景资源（卸载，加载）
        EnterNextScene, //进入场景链
    }

    public abstract class XGameAppState
    {
        protected XGameApp m_app;

        public XGameAppState(XGameApp app)
        {
            m_app = app;
        }
        public abstract void OnEnter();
        public abstract void OnLeave();
        public abstract void OnTick(float elapsedTime);
    }

    public class XGameAppStateChangeNone : XGameAppState
    {
        public XGameAppStateChangeNone(XGameApp app) : base(app)
        {
        }
        public override void OnEnter()
        {
        }
        public override void OnLeave()
        {
        }
        public override void OnTick(float elapsedTime)
        {
            if (m_app.TargetNode != m_app.TopScene)
            {
                m_app.ChangeState(EnumGameAppState.ChangeScene);
                return;
            }
        }
    }

    public class XGameAppStateChangeScene : XGameAppState
    {
        public XGameAppStateChangeScene(XGameApp app) : base(app)
        {
        }
        public override void OnEnter()
        {
            //查找共用父节点
            m_app.FindChangeSceneList(m_app.TargetNode, m_app.EnterScenesList, m_app.LeaveScenesList);
            XDebug.Log(XGameApp.Tag, $"待退出场景 {m_app.ToString(m_app.LeaveScenesList)}");
            XDebug.Log(XGameApp.Tag, $"待进入场景 {m_app.ToString(m_app.EnterScenesList)}");

            //收集需要卸载的资源
            //收集需要加载的资源

            m_app.ChangeState(EnumGameAppState.LeaveCurrScene);
        }
        public override void OnLeave()
        {
        }
        public override void OnTick(float elapsedTime)
        {
        }
    }

    public class XGameAppStateLeaveCurrScene : XGameAppState
    {
        public XGameAppStateLeaveCurrScene(XGameApp app) : base(app)
        {
        }
        public override void OnEnter()
        {
            //依次退出场景直到共用节点
            foreach (var node in m_app.LeaveScenesList)
            {
                m_app.LeaveScene(node);
            }
            m_app.ChangeState(EnumGameAppState.HandleAssets);
        }
        public override void OnLeave()
        {
        }
        public override void OnTick(float elapsedTime)
        {
        }
    }
    public class XGameAppStateHandleAssets : XGameAppState
    {
        public XGameAppStateHandleAssets(XGameApp app) : base(app)
        {
        }
        public override void OnEnter()
        {
            m_app.ChangeState(EnumGameAppState.EnterNextScene);
        }
        public override void OnLeave()
        {
        }
        public override void OnTick(float elapsedTime)
        {
        }
    }
    public class XGameAppStateEnterNextScene : XGameAppState
    {
        protected AsyncOperation m_loadOperation;
        public XGameAppStateEnterNextScene(XGameApp app) : base(app)
        {
        }
        public override void OnEnter()
        {
            //加载unity场景
            var unityScene = string.Empty;
            foreach (var node in m_app.CurrScenes)
            {
                if (string.IsNullOrEmpty(node.scene.UnityScene))
                    continue;
                unityScene = node.scene.UnityScene;
            }
            if (m_app.EnterScenesList.Count > 0)
            {
                foreach (var node in m_app.EnterScenesList)
                {
                    if (string.IsNullOrEmpty(node.scene.UnityScene))
                        continue;
                    unityScene = node.scene.UnityScene;
                }
            }
            if (!string.IsNullOrEmpty(unityScene) && unityScene != m_app.CurrUnityScene)
            {
#if UNITY_EDITOR
                EditorSceneManager.LoadSceneInPlayMode($"Assets/XGameKitSamples/XGameScene/{unityScene}.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
                m_loadOperation = SceneManager.LoadSceneAsync(unityScene);
#endif
                m_app.SetCurrUnityScene(unityScene);

            }
            else
            {
                m_loadOperation = null;
            }
        }
        public override void OnLeave()
        {
        }
        public override void OnTick(float elapsedTime)
        {
            if (m_loadOperation == null || m_loadOperation.isDone)
            {
                //依次进入场景
                foreach (var node in m_app.EnterScenesList)
                {
                    m_app.EnterScene(node);
                }
                m_app.ChangeState(EnumGameAppState.None);
            }
        }
    }
}