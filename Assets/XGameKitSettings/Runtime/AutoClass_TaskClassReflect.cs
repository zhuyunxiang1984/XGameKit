using System;
using System.Collections.Generic;
using XGameKit.Core;
using XGameKit.XBehaviorTree;

public static class AutoClass_TaskClassReflect
{
	public static Dictionary<string, Func<object>> datas = new Dictionary<string, Func<object>>()
	{
		{"XBTComposite_Parallel_And", () => { return XObjectPool.Alloc<XBTComposite_Parallel_And>();}},
		{"XBTComposite_Parallel_Or", () => { return XObjectPool.Alloc<XBTComposite_Parallel_Or>();}},
		{"XBTComposite_Selector", () => { return XObjectPool.Alloc<XBTComposite_Selector>();}},
		{"XBTComposite_Sequence", () => { return XObjectPool.Alloc<XBTComposite_Sequence>();}},
		{"XBTDecorator_Not", () => { return XObjectPool.Alloc<XBTDecorator_Not>();}},
		{"XBTDecorator_Repeat", () => { return XObjectPool.Alloc<XBTDecorator_Repeat>();}},
		{"XBTTaskLog", () => { return XObjectPool.Alloc<XBTTaskLog>();}},
		{"XBTTaskWait", () => { return XObjectPool.Alloc<XBTTaskWait>();}},
	};
}
