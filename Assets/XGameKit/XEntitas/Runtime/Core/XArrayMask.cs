using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XEntitas
{
    /*
     * 
     * */
    public class XArrayMask
    {
        //数据长度
        private const int DATA_SIZE = 32;
        //数据数组
        private uint[] m_array;
        //数组长度
        private int m_arrayLength;
        //可描述字节数量
        private int m_count;

        public XArrayMask(int capacity = DATA_SIZE)
        {
            _Resize(capacity);
        }

        public void Reset()
        {
            for (int i = 0; i < m_arrayLength; ++i)
            {
                m_array[i] = 0;
            }
        }

        //检测bit  (1<<value)
        public bool Check(int value)
        {
            if (value >= m_count)
                return false;
            int index = value / DATA_SIZE;
            return (m_array[index] & (uint)(1 << (value % DATA_SIZE))) != 0;
        }
        //设置bit (1<<value)
        public void Add(int value)
        {
            if (value >= m_count)
                _Resize(_CalcArrayLen(value + 1) * DATA_SIZE);
            int index = value / DATA_SIZE;
            m_array[index] = m_array[index] | (uint)(1 << (value % DATA_SIZE));
        }

        //清除bit (1<<value)
        public void Remove(int value)
        {
            if (value >= m_count)
                return;
            int index = value / DATA_SIZE;
            var temp = (uint)(1 << (value % DATA_SIZE));
            m_array[index] = m_array[index] | temp;
            m_array[index] = m_array[index] ^ temp;
        }

        //debug
        public new string ToString()
        {
            string text = $"数组mask length={m_arrayLength} count={m_count} \n";
            int count = 0;
            for (int i = 0; i < m_arrayLength; ++i)
            {
                var data = m_array[i];
                for (int j = 0; j < DATA_SIZE; ++j)
                {
                    if ((data & (1 << j)) != 0)
                    {
                        text += "1";
                    }
                    else
                    {
                        text += "0";
                    }
                    ++count;
                    if (count % 8 == 0) //间隔8位加个空
                    {
                        text += " ";
                    }
                }
            }
            return text;
        }

        //重设大小
        private void _Resize(int count)
        {
            int newlen = _CalcArrayLen(count);
            if (m_arrayLength == newlen)
                return;
            var newdatas = new uint[newlen];
            if (m_arrayLength > 0)
            {
                for (int i = 0; i < newlen && i < m_arrayLength; ++i)
                {
                    newdatas[i] = m_array[i];
                }
            }
            m_array = newdatas;
            m_arrayLength = newlen;
            m_count = count;
        }

        //计算数组长度
        private int _CalcArrayLen(int count)
        {
            return Mathf.CeilToInt(count / (float)DATA_SIZE);
        }
    }
}