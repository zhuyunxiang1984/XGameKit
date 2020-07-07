using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


namespace XGameKit.Core
{

    // A behaviour that is attached to a playable
    public class PlayableBehaviourPostEvent : PlayableBehaviour
    {
        public string EventName;
        public string EventPara;
        public XPlayableTimeline mono;

        // Called when the owning graph starts playing
        public override void OnGraphStart(Playable playable)
        {
        }

        // Called when the owning graph stops playing
        public override void OnGraphStop(Playable playable)
        {
        }

        // Called when the state of the playable is set to Play
        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            if (string.IsNullOrEmpty(EventName))
                return;
            XDebug.Log(XPlayableConst.Tag, $"触发事件 name:{EventName} param:{EventPara}");
            mono?.PostEvent(EventName, EventPara);
        }

        // Called when the state of the playable is set to Paused
        public override void OnBehaviourPause(Playable playable, FrameData info)
        {

        }

        // Called each frame while the state is set to Play
        public override void PrepareFrame(Playable playable, FrameData info)
        {

        }
    }

}
