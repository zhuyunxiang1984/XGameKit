﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XGameKit.GameApp;

namespace XGameKit.GameSceneSample
{
    public static class EnumSceneName
    {
        public const string scene1 = "scene1";
        public const string scene2 = "scene2";
        public const string scene3 = "scene3";
        public const string scene4 = "scene4";
        public const string scene5 = "scene5";
        public const string scene6 = "scene6";
        public const string scene7 = "scene7";
        public const string scene8 = "scene8";
        public const string scene9 = "scene9";
    }

    public class XGameSceneTest : XGameScene
    {
    }

    public class XGameSceneTest1 : XGameScene
    {
        public override string UnityScene => "SampleScene1";
    }
    public class XGameSceneTest2 : XGameScene
    {
        public override string UnityScene => "SampleScene2";
    }
    public class XGameSceneTest8 : XGameScene
    {
        public override string UnityScene => "SampleScene8";
    }
    public class XGameSceneTest9 : XGameScene
    {
        public override string UnityScene => "SampleScene9";
    }
}
