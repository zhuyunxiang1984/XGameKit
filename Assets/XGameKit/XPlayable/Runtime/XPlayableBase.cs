using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using XGameKit.Core;
using UnityEngine;
using UnityEngine.Serialization;

namespace XGameKit.Core
{
    public abstract class XPlayableBase : MonoBehaviour
    {
#if UNITY_EDITOR
        [Button("播放"), HorizontalGroup("2")]
        void EditPlayAnim()
        {
            Play();
        }
        [Button("倒放"), HorizontalGroup("2")]
        void EditRevsAnim()
        {
            Revs();
        }
#endif
        //命名
        public string PlayableName;

        public enum EnumPlayMode
        {
            Once = 0, //播放一次
            OnceSample, //播放一次并保持在最后一帧
            Loop, //循环播放
        }

        private EnumPlayMode _PlayMode;
        private bool _IsPlaying;
        private float _PlayTime;
        private float _PlayTimeCounter;
        private bool _IsReverse;
        private Action _OnComplete;

        protected virtual string _GetPlayName()
        {
            return PlayableName;
        }

        protected abstract float _GetPlayTime(float time);
        protected abstract void _OnInit();
        protected abstract void _OnPlay();
        protected abstract void _OnStop();
        protected abstract void _OnUpdate(float curTime, float maxTime);

        private void Awake()
        {
            _OnInit();
        }

        private void OnDisable()
        {
            _StopInternal();
        }

        private void Update()
        {
            if (!_IsPlaying)
                return;
            _PlayTimeCounter += Time.deltaTime;
            //Debug.Log(_PlayTimeCounter + " " + _PlayTime);
            if (_PlayTimeCounter > _PlayTime)
                _PlayTimeCounter = _PlayTime;
            if (_PlayTime > 0f)
            {
                _UpdateInterval();
            }
            if (_PlayTimeCounter >= _PlayTime)
            {
                switch (_PlayMode)
                {
                    case EnumPlayMode.Loop:
                        _PlayTimeCounter = _IsReverse ? 1f : 0f;
                        break;
                    case EnumPlayMode.Once:
                        XDebug.Log(XPlayableConst.Tag, $"播放完成 {_GetPlayName()}");
                        _StopInternal();
                        _OnComplete?.Invoke();
                        break;
                    case EnumPlayMode.OnceSample:
                        XDebug.Log(XPlayableConst.Tag, $"播放完成 {_GetPlayName()}");
                        _OnComplete?.Invoke();
                        break;
                }
            }
        }

        private void _PlayInternal()
        {
            _IsPlaying = true;
            _PlayTimeCounter = 0f;
            _OnPlay();
            _UpdateInterval();
        }

        private void _StopInternal()
        {
            if (!_IsPlaying)
                return;
            _OnStop();
            _IsPlaying = false;
        }

        private void _UpdateInterval()
        {
            if (_IsReverse)
            {
                _OnUpdate(_PlayTime - _PlayTimeCounter, _PlayTime);
            }
            else
            {
                _OnUpdate(_PlayTimeCounter, _PlayTime);
            }
        }

        public void Play(Action OnComplete = null, float time = 0f, EnumPlayMode mode = EnumPlayMode.Once)
        {
            XDebug.Log(XPlayableConst.Tag, $"开始播放 {_GetPlayName()}");
            _PlayMode = mode;
            _PlayTime = _GetPlayTime(time);
            _IsReverse = false;
            _OnComplete = OnComplete;
            _PlayInternal();
        }

        public void Revs(Action OnComplete = null, float time = 0f, EnumPlayMode mode = EnumPlayMode.Once)
        {
            XDebug.Log(XPlayableConst.Tag, $"开始倒放 {_GetPlayName()}");
            _PlayMode = mode;
            _PlayTime = _GetPlayTime(time);
            _IsReverse = true;
            _OnComplete = OnComplete;
            _PlayInternal();
        }

        public void Stop()
        {
            _StopInternal();
        }
    }
}

