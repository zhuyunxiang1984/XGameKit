using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using XGameKit.Core;

namespace XGameKit.XBehaviorTree
{
    public static class EditorXBehaviorTree
    {
        private const string XBTMenu = "XGameKit/行为树";
        /* 创建模板
            public static class AutoClass_TaskClassReflect
            {
                public static Dictionary<string, Func<object>> datas = new Dictionary<string, Func<object>>()
                {
                    {"aa", () => { return XObjectPool.Alloc<XBTComposite_Sequence>();}},
                    {"aa", () => { return XObjectPool.Alloc<XBTComposite_Sequence>();}},
                };
            }
         */
        const string TaskClassPath = "Assets/XGameKitSettings/Runtime/AutoClass_TaskClassReflect.cs";
        [MenuItem(XBTMenu + "/生成task类数据集")]
        static void GenerateTaskClassData()
        {
            XUtilities.MakePathExist(TaskClassPath);
            var contents = string.Empty;
            var result = XBTUtilities.GetAllTaskClass();
            //contents += "\n";
            contents += "using System;\n";
            contents += "using System.Collections.Generic;\n";
            contents += "using XGameKit.Core;\n";
            contents += "\n";
            contents += "public static class AutoClass_TaskClassReflect\n";
            contents += "{\n";
            contents += "\tpublic static Dictionary<string, Func<object>> datas = new Dictionary<string, Func<object>>()\n";
            contents += "\t{\n";
            foreach (var data in result)
            {
                contents += $"\t\t{{\"{data.Item1}\", () => {{ return XObjectPool.Alloc<{data.Item3.FullName}>();}}}},\n";
            }
            contents += "\t};\n";
            contents += "}\n";
            File.WriteAllText(TaskClassPath, contents);
            AssetDatabase.Refresh();
        }

        [MenuItem(XBTMenu + "/清空task类数据集")]
        static void ClearTaskClassData()
        {
            AssetDatabase.DeleteAsset(TaskClassPath);
        }

        [MenuItem(XBTMenu + "/task节点编辑")]
        static void EditTreeNode()
        {
            EditorWindow window = EditorWindow.GetWindow<EditorXBehaviorTreeWindow>();
            window.Show();
        }
    }
}
