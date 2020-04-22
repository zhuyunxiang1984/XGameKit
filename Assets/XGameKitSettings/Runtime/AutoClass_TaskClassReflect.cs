using System;
using System.Collections.Generic;
using XGameKit.Core;

public static class AutoClass_TaskClassReflect
{
	public static Dictionary<string, Func<object>> datas = new Dictionary<string, Func<object>>()
	{
		{"XUIWindowTask_Cache", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_Cache>();}},
		{"XUIWindowTask_Hide", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_Hide>();}},
		{"XUIWindowTask_Idle", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_Idle>();}},
		{"XUIWindowTask_IsShow", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_IsShow>();}},
		{"XUIWindowTask_IsHide", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_IsHide>();}},
		{"XUIWindowTask_IsRemove", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_IsRemove>();}},
		{"XUIWindowTask_LoadAsset", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_LoadAsset>();}},
		{"XUIWindowTask_Remove", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_Remove>();}},
		{"XUIWindowTask_Show", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_Show>();}},
		{"XUIWindowTask_StateChange", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_StateChange>();}},
		{"XUIWindowTask_UnloadAsset", () => { return XObjectPool.Alloc<XGameKit.XUI.XUIWindowTask_UnloadAsset>();}},
		{"XBTComposite_Parallel_And", () => { return XObjectPool.Alloc<XGameKit.XBehaviorTree.XBTComposite_Parallel_And>();}},
		{"XBTComposite_Parallel_Or", () => { return XObjectPool.Alloc<XGameKit.XBehaviorTree.XBTComposite_Parallel_Or>();}},
		{"XBTComposite_Selector", () => { return XObjectPool.Alloc<XGameKit.XBehaviorTree.XBTComposite_Selector>();}},
		{"XBTComposite_Sequence", () => { return XObjectPool.Alloc<XGameKit.XBehaviorTree.XBTComposite_Sequence>();}},
		{"XBTDecorator_AlwayFailure", () => { return XObjectPool.Alloc<XGameKit.XBehaviorTree.XBTDecorator_AlwayFailure>();}},
		{"XBTDecorator_AlwaySuccess", () => { return XObjectPool.Alloc<XGameKit.XBehaviorTree.XBTDecorator_AlwaySuccess>();}},
		{"XBTDecorator_Not", () => { return XObjectPool.Alloc<XGameKit.XBehaviorTree.XBTDecorator_Not>();}},
		{"XBTDecorator_Repeat", () => { return XObjectPool.Alloc<XGameKit.XBehaviorTree.XBTDecorator_Repeat>();}},
		{"XBTTaskLog", () => { return XObjectPool.Alloc<XGameKit.XBehaviorTree.XBTTaskLog>();}},
		{"XBTTaskWait", () => { return XObjectPool.Alloc<XGameKit.XBehaviorTree.XBTTaskWait>();}},
	};
}
