using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using XGameKit.XBehaviorTree;

public class EditorXBehaviorTreeWindow : EditorWindow
{
    private void OnEnable()
    {
        _Filter();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("新增子节点"))
        {
            _RunMethodForFirstOne((go) =>
            {
                var mono = new GameObject().AddComponent<XBTNodeMono>();
                mono.transform.SetParent(go.transform);
            });
        }
        if (GUILayout.Button("新增父节点"))
        {
            _RunMethodForFirstOne((go) =>
            {
                var mono = new GameObject().AddComponent<XBTNodeMono>();
                mono.transform.SetParent(go.transform.parent);
            });
        }
        if (GUILayout.Button("新增兄节点"))
        {
            _RunMethodForFirstOne((go) =>
            {
                var mono = new GameObject().AddComponent<XBTNodeMono>();
                mono.transform.SetParent(go.transform.parent);
                go.transform.SetParent(mono.transform);
            });
        }
        GUILayout.Space(10);
        //所有task实现类
        DrawTaskButtons();
    }

    static string _ClassNameFilter = String.Empty;
    static List<(string, string, Type)> _FilterList = new List<(string, string, Type)>();
    void DrawTaskButtons()
    {
        GUILayout.Label("节点列表");
        //过滤
        EditorGUI.BeginChangeCheck();
        _ClassNameFilter = GUILayout.TextField(_ClassNameFilter);
        if (EditorGUI.EndChangeCheck())
        {
            _Filter();
        }
        GUILayout.BeginHorizontal();
        int count = Mathf.CeilToInt(_FilterList.Count / 2f);
        for (int col = 0; col < 2; ++col)
        {
            GUILayout.BeginVertical();
            for (int row = 0; row < count; ++row)
            {
                int index = col + 2 * row;
                if (index >= _FilterList.Count)
                {
                    continue;
                }
                var data = _FilterList[index];
                string text = string.Empty;
                if (string.IsNullOrEmpty(data.Item2))
                {
                    text = data.Item1;
                }
                else
                {
                    text = data.Item2;
                }
                if (GUILayout.Button(text))
                {
                    _RunMethodForEveryOne((go) =>
                    {
                        var mono = go.GetComponent<XBTNodeMono>();
                        if (mono == null)
                            return;
                        Undo.RecordObject(mono, "change TaskClassName");
                        mono.className = data.Item1;
                        EditorUtility.SetDirty(mono);
                    });
                }
            }
            GUILayout.EndVertical();
            
        }
        GUILayout.EndHorizontal();
    }

    void _Filter()
    {
        var listAllTaskClass = XBTUtilities.GetAllTaskClassListEditor();
        _FilterList.Clear();
        if (string.IsNullOrEmpty(_ClassNameFilter))
        {
            _FilterList.AddRange(listAllTaskClass);
        }
        else
        {
            var temp = _ClassNameFilter.ToLower();
            foreach (var data in listAllTaskClass)
            {
                if (data.Item1.ToLower().IndexOf(temp) == -1 &&
                    data.Item2.ToLower().IndexOf(temp) == -1)
                    continue;
                _FilterList.Add(data);
            }
        }
    }

    void _RunMethodForFirstOne(Action<GameObject> method)
    {
        var go = Selection.activeGameObject;
        if (go == null)
            return;
        method?.Invoke(go);
    }
    void _RunMethodForEveryOne(Action<GameObject> method)
    {
        foreach (var gameObject in Selection.gameObjects)
        {
            if (gameObject == null)
                continue;
            method?.Invoke(gameObject);
        }
    }

    
}
