using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace XGameKit.Core
{
    [RequireComponent(typeof(PlayableDirector))]
    public class XPlayableTimeline : XPlayableBase
    {
        private PlayableDirector m_director;

        //绑定列表
        private Dictionary<string, PlayableBinding> m_bindings = new Dictionary<string, PlayableBinding>();
        //触发事件
        private Action<string, string> m_events;

        //抛事件
        public void PostEvent(string name, string param)
        {
            m_events?.Invoke(name, param);
        }
        public void SetEvent(Action<string, string> value)
        {
            m_events = value;
        }
        protected override float _GetPlayTime(float time)
        {
            return (float)m_director.duration;
        }
        protected override void _OnInit()
        {
            m_director = GetComponent<PlayableDirector>();
            m_bindings.Clear();
            foreach (var output in m_director.playableAsset.outputs)
            {
                if (m_bindings.ContainsKey(output.streamName))
                    continue;
                //Debug.Log(output.streamName);
                m_bindings.Add(output.streamName, output);
            }
            m_director.timeUpdateMode = DirectorUpdateMode.Manual;
        }
        protected override void _OnPlay()
        {
            //开始的时候不用play，因为设置为manual控制，在update中手动evaluate
            m_director.Play();

            //这里强制把所有的timeline下的particlesystem都设置成autorandomseed，因为timeline默认会给一个随机种子
            //这里不用担心使用prefab的track，因为在开始播放时，会创建所有的prefab实例
            var particles = gameObject.GetComponentsInChildren<ParticleSystem>(true);
            foreach (var particle in particles)
            {
                particle.useAutoRandomSeed = true;
            }
        }
        protected override void _OnStop()
        {
            m_director.Stop();
        }
        protected override void _OnUpdate(float curTime, float maxTime)
        {
            m_director.time = m_director.duration * Mathf.Clamp01(curTime / maxTime);
            m_director.Evaluate();
            //XDebug.Log(XPlayableConst.Tag, $"Evaluate {m_director.time}");
        }


        public void SetBindingData(string name, UnityEngine.Object target)
        {
            if (!m_bindings.ContainsKey(name))
                return;
            m_director.SetGenericBinding(m_bindings[name].sourceObject, target);
        }
        public void MuteTrack(string name, bool mute)
        {
            if (!m_bindings.ContainsKey(name))
                return;
            (m_bindings[name].sourceObject as TrackAsset).muted = mute;
        }
    }
}