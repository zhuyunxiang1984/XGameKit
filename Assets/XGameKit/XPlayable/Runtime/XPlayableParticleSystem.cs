using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XGameKit.Core
{
    public class XPlayableParticleSystem : XPlayableBase
    {
        public GameObject AppointParticleSystemRoot;

        private GameObject _ParticleSystemRoot;
        private ParticleSystem[] _ParticleSystems;
        protected override string _GetPlayName()
        {
            return $"{PlayableName}-{_ParticleSystemRoot.name}";
        }

        protected override float _GetPlayTime(float time)
        {
            float result = 0f;
            if (_ParticleSystems != null)
            {
                foreach (var ps in _ParticleSystems)
                {
                    Debug.Log($"{ps.name} {ps.main.duration}");
                    result = Mathf.Max(result, ps.main.duration);
                }
            }
            return result;
        }

        protected override void _OnInit()
        {
            if (AppointParticleSystemRoot != null)
            {
                _ParticleSystemRoot = AppointParticleSystemRoot;
            }
            else
            {
                _ParticleSystemRoot = gameObject;
            }
            _ParticleSystems = _ParticleSystemRoot.GetComponentsInChildren<ParticleSystem>();
            _ParticleSystemRoot.SetActive(false);
        }

        protected override void _OnPlay()
        {
            if (_ParticleSystems != null)
            {
                foreach (var ps in _ParticleSystems)
                {
                    ps.Clear();
                    ps.Stop();
                    ps.Play();
                }
            }
            _ParticleSystemRoot.SetActive(true);
        }

        protected override void _OnStop()
        {
            _ParticleSystemRoot.SetActive(false);
        }

        protected override void _OnUpdate(float curTime, float maxTime)
        {
        }
    }
}
