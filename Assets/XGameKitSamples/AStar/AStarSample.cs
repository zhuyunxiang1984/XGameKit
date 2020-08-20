using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class AStarSample : SerializedMonoBehaviour
{
    public AStarGrid m_grid;
    [HorizontalGroup("1"), LabelWidth(20)]
    public int w = 10, h = 10;
    [HorizontalGroup("2"), LabelWidth(20)]
    public int x1, y1;
    [HorizontalGroup("2"), LabelWidth(20)]
    public int x2, y2;

    [HorizontalGroup("3"), Button("重置")]
    void Button1()
    {
        map = new bool[h, w];
        Reset();
    }
    [HorizontalGroup("3"), Button("寻路")]
    void Button2()
    {
        AStar();
    }

    [TableMatrix(DrawElementMethod = "DrawColoredEnumElement", Transpose = true)]
    public bool[,] map = new bool[10, 10];

    private static bool DrawColoredEnumElement(Rect rect, bool value)
    {
        if (rect.Contains(Event.current.mousePosition) && (Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag))
        {
            if (Event.current.type == EventType.MouseDown)
            {
                value = !value;
            }
            if (Event.current.type == EventType.MouseDrag)
            {
                value = true;
            }
            GUI.changed = true;
            Event.current.Use();
        }

        UnityEditor.EditorGUI.DrawRect(rect, value ? new Color(0.1f, 0.8f, 0.2f) : new Color(0, 0, 0, 0.5f));

        return value;
    }

    protected AStar m_astar = new AStar();
    void Start()
    {
        Reset();
    }

    private void Reset()
    {
        m_astar.Init(map);

        int row = map.GetLength(0);
        int col = map.GetLength(1);
        //Debug.Log($"row:{row} col:{col}");
        m_grid.Clear();
        m_grid.Init(col, row);
        for (int y = 0; y < row; ++y)
        {
            for (int x = 0; x < col; ++x)
            {
                m_grid.SetColor(x, y, map[y, x] ? Color.black : Color.white);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void AStar()
    {
        Reset();
        var path = m_astar.FindPath(x1, y1, x2, y2);
        if (path != null)
        {
            for (int i = 0; i < path.Count; ++i)
            {
                //Debug.Log($"x:{node.x} y:{node.y}");
                var node = path[i];
                if (i == 0)
                {
                    m_grid.SetColor(node.x, node.y, Color.blue);
                }
                else if (i == path.Count - 1)
                {
                    m_grid.SetColor(node.x, node.y, Color.red);
                }
                else
                {
                    m_grid.SetColor(node.x, node.y, Color.green);
                }

            }
        }
    }
}

#region A*算法实现

public class AStar
{
    public const int BLOCK = 1;
    public const int COST = 1;

    public class Node
    {
        public Node parent;
        public int x, y;
        public int f, g, h;
    }
    private bool[,] m_map = null;
    private int m_row, m_col;

    private Dictionary<int, Node> m_opens = new Dictionary<int, Node>();
    private byte[] m_states = null;
    private Node m_node;
    private Node m_next;

    public void Init(bool[,] map)
    {
        m_map = map;
        m_row = map.GetLength(0);
        m_col = map.GetLength(1);
        m_states = new byte[m_row * m_col];
    }
    public List<Node> FindPath(int x1, int y1, int x2, int y2)
    {
        //清空搜索状态
        for (int i = 0; i < m_states.Length; ++i)
        {
            m_states[i] = 0;
        }
        m_opens.Clear();
        m_states[_XYToIndex(x1, y1)] = 1;
        m_node = new Node() { x = x1, y = y1,};
        m_next = null;

        List<Node> path = null;

        int count = 0;
        while (count < 99999)
        {
            ++count;
            //四方向查找
            for (int i = 0; i < 4; ++i)
            {
                if (m_next == null)
                    m_next = new Node();
                m_next.x = m_node.x;
                m_next.y = m_node.y;
                m_next.parent = m_node;
                switch (i)
                {
                    case 0://上
                        m_next.y = m_node.y - 1;
                        m_next.g = m_node.g + COST;
                        break;
                    case 1://下
                        m_next.y = m_node.y + 1;
                        m_next.g = m_node.g + COST;
                        break;
                    case 2://左
                        m_next.x = m_node.x - 1;
                        m_next.g = m_node.g + COST;
                        break;
                    case 3://右
                        m_next.x = m_node.x + 1;
                        m_next.g = m_node.g + COST;
                        break;
                }
                var index = _XYToIndex(m_next.x, m_next.y);
                if (!_CanWalkable(m_next))
                    continue;
                m_states[index] = 1;
                m_next.f = m_next.g + _ManhattanValue(m_next.x, m_next.y, x2, y2);
                //是否已经存在于开放节点
                if (m_opens.ContainsKey(index))
                {
                    var temp = m_opens[index];
                    if (temp.f < m_next.f)
                        continue;
                    temp.f = m_next.f;
                }
                else
                {
                    m_opens.Add(index, m_next);
                    m_next = null;
                }
            }
            
            //查找开放节点中代价最小的点为下一个探索父节点
            var mincostNode = _FindMinCostNode();
            if (mincostNode == null)
                break;

            //检测是否找到了终点
            if (mincostNode.x == x2 && mincostNode.y == y2)
            {
                path = new List<Node>();
                
                while (mincostNode != null)
                {
                    path.Insert(0, mincostNode);
                    mincostNode = mincostNode.parent;
                }
                break;
            }
            m_node = mincostNode;
        }
        return path;
    }

    //查找最小代价的节点
    private Node _FindMinCostNode()
    {
        if (m_opens.Count < 1)
            return null;
        Node result = null;
        int mincost = 0;
        foreach (var node in m_opens.Values)
        {
            if (result == null || node.f < mincost)
            {
                result = node;
                mincost = node.f;
            }
        }
        if (result == null)
            return null;
        m_opens.Remove(_XYToIndex(result.x, result.y));
        return result;
    }

    //检测是否可以通行
    private bool _CanWalkable(Node node)
    {
        //边界检测
        if (node.x < 0 || node.x >= m_col || node.y < 0 || node.y >= m_row)
            return false;
        //障碍检测
        if (m_map[node.y, node.x])
            return false;
        //状态检测
        if (m_states[_XYToIndex(node.x, node.y)] == 1)
            return false;
        return true;
    }

    private int _XYToIndex(int x, int y)
    {
        return y * m_col + x;
    }

    //曼哈顿估算
    private int _ManhattanValue(int x1, int y1, int x2, int y2)
    {
        return Mathf.Abs(x2 - x1) + Mathf.Abs(y2 - y1);
    }
}

#endregion