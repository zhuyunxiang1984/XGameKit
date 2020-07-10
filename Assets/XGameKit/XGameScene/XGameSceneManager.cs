using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.GameScene
{
    public class XGameSceneManager
    {
        public enum EnumState
        {
            None = 0,

        }

        //激活的场景节点
        protected Stack<XGameSceneNode> m_ActiveNodes = new Stack<XGameSceneNode>();

        //场景数据
        protected Dictionary<string, XGameSceneNode> m_dictSceneNodes = new Dictionary<string, XGameSceneNode>();

        //进入节点列表
        protected List<XGameSceneNode> m_EnterNodes = new List<XGameSceneNode>();

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
            foreach (var node in m_ActiveNodes)
            {
                node.scene.TickScene(elapsedTime);
            }
        }
        //跳转场景
        public void EnterScene(string name)
        {
            //检测场景是否存在
            if (!m_dictSceneNodes.ContainsKey(name))
                return;

            Debug.Log($"=== 跳转场景 ===");

            //查找共用父节点
            _FindEnterScenes(m_dictSceneNodes[name], ref m_EnterNodes);

            string t = string.Empty;
            for (int i = 0; i < m_EnterNodes.Count; ++i)
            {
                if (i > 0)
                {
                    t += " - ";
                }
                var temp = m_EnterNodes[i];
                if (temp == null)
                    t += "Root";
                else
                    t += temp?.scene.Name;
            }
            Debug.Log($"场景链 {t}");

            //依次退出场景直到共用节点
            while (m_ActiveNodes.Count > 0)
            {
                var node = m_ActiveNodes.Peek();
                if (node == m_EnterNodes[0])
                    break;
                node.scene.LeaveScene();
                m_ActiveNodes.Pop();
            }
            //依次预载资源
            for (int i = 1; i < m_EnterNodes.Count; ++i)
            {
            }
            //依次进入场景
            for (int i = 1; i < m_EnterNodes.Count; ++i)
            {
                var node = m_EnterNodes[i];
                node.scene.EnterScene();
                m_ActiveNodes.Push(node);
            }
        }

        //查找切换到节点node，进入的节点列表
        protected void _FindEnterScenes(XGameSceneNode node, ref List<XGameSceneNode> enterScenes)
        {
            enterScenes.Clear();

            while (node != null)
            {
                enterScenes.Insert(0, node);
                if (m_ActiveNodes.Contains(node))
                    return;
                if (node.parent == null)
                {
                    enterScenes.Insert(0, null);
                    return;
                }
                node = node.parent;
            }
        }

    }

    
}