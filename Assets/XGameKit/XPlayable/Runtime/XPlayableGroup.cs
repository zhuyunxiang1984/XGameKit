using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public class XPlayableGroup : MonoBehaviour
    {
        private const string Unnamed = "Unnamed";
    
        private Dictionary<string, List<XPlayableBase>> _Playables = new Dictionary<string, List<XPlayableBase>>();
        private void Awake()
        {
            _Playables.Clear();
            var monos = gameObject.GetComponentsInChildren<XPlayableBase>();
            if (monos != null)
            {
                foreach (var mono in monos)
                {
                    var playableName = mono.PlayableName;
                    if (string.IsNullOrEmpty(playableName))
                    {
                        playableName = Unnamed;
                    }
                    if (_Playables.ContainsKey(playableName))
                    {
                        _Playables[playableName].Add(mono);
                    }
                    else
                    {
                        _Playables.Add(playableName, new List<XPlayableBase>(){mono});
                    }
                }
            }
        }

        public void Play(string playableName, Action OnComplete = null, float time = 0, XPlayableBase.EnumPlayMode mode = XPlayableBase.EnumPlayMode.Once)
        {
            if (!_Playables.ContainsKey(playableName))
                return;
            foreach (var playable in _Playables[playableName])
            {
                playable.Play(OnComplete, time, mode);
            }
        }

        public void Stop(string playableName)
        {
            if (!_Playables.ContainsKey(playableName))
                return;
            foreach (var playable in _Playables[playableName])
            {
                playable.Stop();
            }
        }
        public void PlayAll()
        {
            foreach (var playables in _Playables.Values)
            {
                foreach (var playable in playables)
                {
                    playable.Play();
                }
            }
        }
    }
}