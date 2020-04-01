using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XGOListManager : MonoBehaviour
{
    //预设
    public GameObject prefab;

    //实例根节点
    protected Transform _PrefabRoot;

    private Stack<GameObject> _Pool = new Stack<GameObject>();
    private List<GameObject> _ActiveInstList = new List<GameObject>();

    public List<GameObject> list
    {
        get { return _ActiveInstList; }
    }
    
    void Awake()
    {
        _PrefabRoot = prefab.transform.parent;
        prefab.SetActive(false);

        foreach (Transform child in _PrefabRoot)
        {
            if (child.gameObject == prefab)
                continue;
            child.gameObject.name = prefab.name + _Pool.Count;
            child.gameObject.SetActive(false);
            _Pool.Push(child.gameObject);
        }
    }

    public T AppendInst<T>(int index = -1) where T : MonoBehaviour
    {
        var go = AppendInst(index);
        return go?.GetComponent<T>();
    }

    public GameObject AppendInst(int index = -1)
    {
        GameObject inst = null;
        if (_Pool.Count > 0)
        {
            inst = _Pool.Pop();
        }
        else
        {
            inst = Instantiate(prefab, _PrefabRoot);
        }
        if (inst == null)
        {
            Debug.LogError("生成clone 失败！");
            return null;
        }
        inst.SetActive(true);

        if (index < 0 || index >= _ActiveInstList.Count)
        {
            inst.transform.SetSiblingIndex(_ActiveInstList.Count);
            _ActiveInstList.Add(inst);
        }
        else
        {
            inst.transform.SetSiblingIndex(index);
            _ActiveInstList.Insert(index, inst);
        }
#if UNITY_EDITOR
        _UpdateInstName();
#endif
        return inst;
    }

    public void RemoveInst(GameObject go)
    {
        var inst = go;
        inst.gameObject.SetActive(false);
        inst.transform.SetAsLastSibling();
        _ActiveInstList.Remove(inst);
        _Pool.Push(inst);
#if UNITY_EDITOR
        _UpdateInstName();
#endif
    }
    void _UpdateInstName()
    {
        int index = 0;
        foreach (var go in _ActiveInstList)
        {
            go.name = $"{prefab.name}_{index++}";
        }

        index = 0;
        foreach (var go in _Pool)
        {
            go.name = $"{prefab.name}_pool{index++}";
        }
    }
}
