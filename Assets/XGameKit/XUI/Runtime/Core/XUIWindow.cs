using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.Core;
using XGameKit.XBehaviorTree;

namespace XGameKit.XUI
{
    public class XUIWindow
    {
        public void Reset()
        {
            
        }
        
        public enum EnumState
        {
            None = 0,
            Show,
            Hide,
            Remove,
        }
        //当前状态
        public EnumState CurState;
        //目标状态
        public EnumState DstState;
        //显示动画中
        public bool isShowAnimating;
        //关闭动画中
        public bool isHideAnimating;
        
        //uimanager
        public XUIManager uiManager { get; protected set; }
        public XUIParamBundle paramBundle { get; protected set; }
        //窗口名字
        public string name { get; protected set; }
        //窗口层级
        public int layer;
        //资源名字
        public string resName;
        //资源实例
        public GameObject gameObject;
        public XUIWindowMono mono;
        
        //所属canvas
        public Canvas canvas;
        //初始化参数
        public object initParam;
        //打开时间
        public float openTick;
        //缓存时间
        public float cacheTime;
        
        
        //行为
        protected XBTBehavior m_behavior = new XBTBehavior();

        //msgcache
        public List<XMessage> msgCacheList { get; protected set; }= new List<XMessage>();
        //msgmanager
        public XMsgManager MsgManager { get; protected set; } = new XMsgManager();
        
        //widgetlist
        //protected List<XUIWidget> m_widgets = new List<XUIWidget>();

        public void Init(XUIManager uiManager, XUIParamBundle paramBundle, string name, object param)
        {
            this.uiManager = uiManager;
            this.paramBundle = paramBundle;
            this.name = name;
            //临时
            resName = $"Assets/XGameKitSamples/XUI/Resources/{name}.prefab";
            initParam = param;
            CurState = EnumState.None;
            DstState = EnumState.None;

            //加载window行为
            var prefab = Resources.Load<GameObject>("XUIWindowBehavior");
            var root = XBTUtilities.ParseMono(prefab.GetComponent<XBTNodeMono>());
            m_behavior.Start(root, this);
        }
        public void Term()
        {
            m_behavior.Stop(this);
            uiManager = null;
        }
        public void Tick(float elapsedTime)
        {
            m_behavior.Update(this, elapsedTime);
        }
    }
}