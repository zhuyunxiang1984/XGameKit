using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.GameApp
{
    public class XGameApp : MonoBehaviour
    {
        public const string Tag = "XGameApp";

        //场景数据
        protected Dictionary<string, XGameSceneNode> m_dictSceneNodes = new Dictionary<string, XGameSceneNode>();
        //状态
        protected Dictionary<EnumGameAppState, XGameAppState> m_dictStates = new Dictionary<EnumGameAppState, XGameAppState>();
        protected XGameAppState m_currState;
        protected EnumGameAppState m_currStateEnum;
        protected EnumGameAppState m_nextStateEnum;


        //当前场景栈
        public List<XGameSceneNode> CurrScenes { get; protected set; } = new List<XGameSceneNode>();
        public XGameSceneNode TopScene
        {
            get
            {
                if (CurrScenes.Count < 1)
                    return null;
                return CurrScenes[CurrScenes.Count - 1];
            }
        }

        //进入节点列表
        protected List<XGameSceneNode> m_EnterNodes = new List<XGameSceneNode>();

        //切换到目标场景节点
        public XGameSceneNode TargetNode { get; protected set; }
            
        //待离开的场景列表
        public List<XGameSceneNode> LeaveScenesList { get; protected set; } = new List<XGameSceneNode>();
        //待进入的场景列表
        public List<XGameSceneNode> EnterScenesList { get; protected set; } = new List<XGameSceneNode>();

        private void Awake()
        {
            m_dictStates.Add(EnumGameAppState.None, new XGameAppStateChangeNone(this));
            m_dictStates.Add(EnumGameAppState.ChangeScene, new XGameAppStateChangeScene(this));
            m_dictStates.Add(EnumGameAppState.LeaveCurrScene, new XGameAppStateLeaveCurrScene(this));
            m_dictStates.Add(EnumGameAppState.HandleAssets, new XGameAppStateHandleAssets(this));
            m_dictStates.Add(EnumGameAppState.EnterNextScene, new XGameAppStateEnterNextScene(this));
        }
        private void Start()
        {
            m_currState = m_dictStates[EnumGameAppState.None];
            m_currStateEnum = EnumGameAppState.None;
            m_nextStateEnum = EnumGameAppState.None;

            OnStart();
        }
        private void Update()
        {
            Tick(Time.deltaTime);
        }
        protected virtual void OnStart()
        {

        }

        public void ChangeState(EnumGameAppState state)
        {
            m_nextStateEnum = state;
        }

        public void AddScene(string name, XGameScene scene)
        {
            if (m_dictSceneNodes.ContainsKey(name) || scene == null)
                return;
            scene.SetName(name);
            var node = new XGameSceneNode(scene);
            m_dictSceneNodes.Add(name, node);
        }
        public void SetLink(string parentName, string childName)
        {
            if (!m_dictSceneNodes.ContainsKey(childName) ||
                !m_dictSceneNodes.ContainsKey(parentName))
                return;
            var child = m_dictSceneNodes[childName];
            var parent = m_dictSceneNodes[parentName];
            child.parent = parent;
            parent.children.Add(child);
        }


        public void Tick(float elapsedTime)
        {
            foreach (var node in CurrScenes)
            {
                node.scene.TickScene(elapsedTime);
            }
            if (m_currStateEnum != m_nextStateEnum)
            {
                var temp = m_nextStateEnum;
                m_currState.OnLeave();
                m_currState = m_dictStates[temp];
                m_currState.OnEnter();
                m_currStateEnum = temp;
            }
            m_currState.OnTick(elapsedTime);
        }
        //跳转场景
        public void EnterScene(string name)
        {
            //检测场景是否存在
            if (!m_dictSceneNodes.ContainsKey(name))
                return;

            Debug.Log($"=== 跳转场景 === {name}");
            TargetNode = m_dictSceneNodes[name];
        }

        //查找切换到节点node，进入的节点列表
        public void FindChangeSceneList(XGameSceneNode target, List<XGameSceneNode> enterScenes, List<XGameSceneNode> leaveScenes)
        {
            //查找待进入的场景列表
            enterScenes.Clear();
            while (target != null)
            {
                if (CurrScenes.Contains(target))
                    break;
                enterScenes.Add(target);
                target = target.parent;
            }
            enterScenes.Reverse();

            //查找待退出的场景列表
            leaveScenes.Clear();
            if (target == null)
            {
                leaveScenes.AddRange(CurrScenes);
            }
            else
            {
                var flag = false;
                foreach (var node in CurrScenes)
                {
                    if (node == target)
                    {
                        flag = true;
                        continue;
                    }
                    if (flag)
                    {
                        leaveScenes.Add(node);
                    }
                }
            }
            leaveScenes.Reverse();

        }
        public void EnterScene(XGameSceneNode node)
        {
            node.scene.EnterScene();
            CurrScenes.Add(node);
        }
        public void LeaveScene(XGameSceneNode node)
        {
            node.scene.LeaveScene();
            int index = CurrScenes.Count - 1;
            if (node == CurrScenes[index])
            {
                CurrScenes.RemoveAt(index);
            }
        }

        public string ToString(List<XGameSceneNode> nodes)
        {
            string text = string.Empty;
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (i > 0)
                {
                    text += " - ";
                }
                text += nodes[i].scene.Name;
            }
            return text;
        }

        
    }

    
}