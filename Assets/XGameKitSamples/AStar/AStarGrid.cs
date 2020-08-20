using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AStarGrid : MonoBehaviour
{
    public GameObject prefab;
    public float size = 1f;

    protected int m_col;
    protected int m_row;
    protected List<GameObject> m_list = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        prefab.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clear()
    {
        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
        m_list.Clear();
    }

    public void Init(int col, int row)
    {
        m_col = col;
        m_row = row;

        var w = col * size;
        var h = row * size;

        for (int y = 0; y < row; ++y)
        {
            for (int x = 0; x < col; ++x)
            {
                var go = Instantiate(prefab, transform);
                var trans = go.GetComponent<RectTransform>();
                trans.anchoredPosition = new Vector2(x * size - w * 0.5f, h * 0.5f - y * size);
                trans.sizeDelta = new Vector2(size, size);
                go.SetActive(true);
                m_list.Add(go);
            }
        }
    }
    public void SetColor(int x, int y, Color color)
    {
        int index = y * m_col + x;
        m_list[index].GetComponent<Image>().color = color;
    }
}
