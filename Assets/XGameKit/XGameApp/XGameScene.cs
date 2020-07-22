using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.GameApp
{
    /// <summary>
    /// 游戏场景
    /// </summary>
    public abstract partial class XGameScene
    {       
        public string Name { get; protected set; }

        public void SetName(string name)
        {
            Name = name;
        }
        //需要预载的资源列表
        public virtual List<string> GetResList()
        {
            return null;
        }
        //
        //资源加载后
        public virtual void OnLoaded()
        {
        }
        //进入场景
        public virtual void EnterScene()
        {
            Debug.Log($"进入场景 {Name}");
        }
        //离开场景
        public virtual void LeaveScene()
        {
            Debug.Log($"离开场景 {Name}");
        }
        //Tick
        public virtual void TickScene(float elapsedTime)
        {

        }
    }

    public class XGameSceneNode
    {
        //父节点
        public XGameSceneNode parent;
        public List<XGameSceneNode> children = new List<XGameSceneNode>();
        public XGameScene scene { get; protected set; }

        public XGameSceneNode(XGameScene scene)
        {
            this.scene = scene;
        }
    }
}


