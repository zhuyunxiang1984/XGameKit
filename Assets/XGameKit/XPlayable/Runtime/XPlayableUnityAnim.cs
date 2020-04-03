using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.Core
{
    public class XPlayableUnityAnim : XPlayableBase
    {
        public Animation AppointAnimation;
        public string AnimationName;

        private Animation _Animation;
        private AnimationState _AnimationState;

        protected override string _GetPlayName()
        {
            return $"{PlayableName}-{AnimationName}";
        }

        protected override float _GetPlayTime(float time)
        {
            if (time > 0)
                return time;
            if (_AnimationState != null)
                return _AnimationState.length;
            return 0;
        }

        protected override void _OnInit()
        {
            if (AppointAnimation != null)
            {
                _Animation = AppointAnimation;
            }
            else
            {
                _Animation = GetComponent<Animation>();
            }
            if (_Animation != null)
            {
                _AnimationState = _Animation[AnimationName];
            }
            if (_AnimationState == null)
            {
                XDebug.Log(XPlayableConst.Tag, $"没有找到动画 {AnimationName} {gameObject.name}");
            }
        }

        protected override void _OnPlay()
        {
            if (_AnimationState == null)
                return;
            _AnimationState.normalizedTime = 0f;
            _AnimationState.speed = 0f;
            _Animation.Play(AnimationName);
        }

        protected override void _OnStop()
        {
            _Animation.Stop();
        }

        protected override void _OnUpdate(float curTime, float maxTime)
        {
            if (_AnimationState == null)
                return;
            _AnimationState.normalizedTime = Mathf.Clamp01(curTime / maxTime);
        }
    }
}

