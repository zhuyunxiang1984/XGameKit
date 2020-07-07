using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace XGameKit.Core
{
    /*
     * 由于是使用playableasset自制的事情系统，所以定制的规则是事件的clip要么在开始时就触发，要么就是clip长度与timeline长度对齐（粘头或粘尾）
     * 因为timeline在Evaluate的时候，会因为time跨度大而跳过中间部分的clip，所以自制的事件要保证触发就必须（粘头或粘尾）
     */
    [System.Serializable]
    public class PlayableAssetPostEvent : PlayableAsset
    {
        public string EventName;
        public string EventPara;

        // Factory method that generates a playable based on this asset
        public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
        {
            var behavior = new PlayableBehaviourPostEvent();
            behavior.EventName = EventName;
            behavior.EventPara = EventPara;
            behavior.mono = go.GetComponent<XPlayableTimeline>();
            return ScriptPlayable<PlayableBehaviourPostEvent>.Create(graph, behavior);
        }
    }

}
