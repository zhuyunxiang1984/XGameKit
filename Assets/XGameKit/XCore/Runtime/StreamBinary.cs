using System;
using System.Net;
using System.Text;

namespace XGameKit.Core
{
    #region 二进制流的写
    public class XStreamBinaryWriter
    {
        private byte[] m_buffer;
        private int m_offset;

        public XStreamBinaryWriter()
        {
            m_buffer = new byte[1024];
            m_offset = 0;
        }
        public new string ToString()
        {
            return Convert.ToBase64String(m_buffer);
        }
        public byte[] GetData()
        {
            if (m_offset < 1)
                return null;
            var data = new byte[m_offset];
            Array.Copy(m_buffer, 0, data, 0, m_offset);
            return data;
        }
        public void Reset()
        {
            m_offset = 0;
        }
        public void WriteByte(byte value)
        {
            if (m_offset + 1 > m_buffer.Length)
            {
                _Expand(1);
            }
            m_buffer[m_offset++] = value;
        }
        public void WriteInt16(short value)
        {
            if (m_offset + 2 > m_buffer.Length)
            {
                _Expand(2);
            }
            short temp = IPAddress.HostToNetworkOrder(value);
            byte[] bytes = BitConverter.GetBytes((ushort)temp);

            m_buffer[m_offset++] = bytes[0];
            m_buffer[m_offset++] = bytes[1];
        }
        public void WriteInt32(int value)
        {
            if (m_offset + 4 > m_buffer.Length)
            {
                _Expand(4);
            }
            int temp = IPAddress.HostToNetworkOrder(value);
            byte[] bytes = BitConverter.GetBytes((uint)temp);

            m_buffer[m_offset++] = bytes[0];
            m_buffer[m_offset++] = bytes[1];
            m_buffer[m_offset++] = bytes[2];
            m_buffer[m_offset++] = bytes[3];
        }
        public void WriteFloat(float value)
        {
            if (m_offset + 4 > m_buffer.Length)
            {
                _Expand(4);
            }
            byte[] bytes = BitConverter.GetBytes(value);
            m_buffer[m_offset++] = bytes[0];
            m_buffer[m_offset++] = bytes[1];
            m_buffer[m_offset++] = bytes[2];
            m_buffer[m_offset++] = bytes[3];
        }
        public void WriteString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                WriteInt32(0);
                return;
            }
            var bytes = Encoding.UTF8.GetBytes(value);
            int len = bytes.Length;
            WriteInt32(len);
            if (m_offset + len > m_buffer.Length)
            {
                _Expand(len);
            }
            Array.Copy(bytes, 0, m_buffer, m_offset, len);
            m_offset += len;
        }

        private void _Expand(int size)
        {
            //
            int length = m_buffer.Length;
            while (m_offset + size > length)
            {
                length *= 2;
            }
            //
            byte[] buffer = new byte[length];
            Array.Copy(m_buffer, 0, buffer, 0, m_offset);
            m_buffer = buffer;
        }
    }
    #endregion

    #region 二进制流的读
    public class XStreamBinaryReader
    {
        private byte[] m_buffer;
        private int m_offset;

        public XStreamBinaryReader()
        {
            m_offset = 0;
        }
        public void SetBytes(byte[] data)
        {
            m_buffer = data;
        }
        public void Reset()
        {
            m_offset = 0;
        }

        public byte ReadByte()
        {
            if (m_offset + 1 > m_buffer.Length)
                return 0;
            return m_buffer[m_offset++];
        }
        public short ReadInt16()
        {
            if (m_offset + 2 > m_buffer.Length)
                return 0;
            short value = (short)BitConverter.ToUInt16(m_buffer, m_offset);
            m_offset += 2;
            return IPAddress.NetworkToHostOrder(value);
        }
        public int ReadInt32()
        {
            if (m_offset + 4 > m_buffer.Length)
                return 0;
            int value = (int)BitConverter.ToUInt32(m_buffer, m_offset);
            m_offset += 4;
            return IPAddress.NetworkToHostOrder(value);
        }
        public float ReadFloat()
        {
            if (m_offset + 4 > m_buffer.Length)
                return 0;
            float value = BitConverter.ToSingle(m_buffer, m_offset);
            m_offset += 4;
            return value;
        }
        public string ReadString()
        {
            int len = ReadInt32();
            if (len < 1)
                return string.Empty;
            var bytes = new byte[len];
            Array.Copy(m_buffer, m_offset, bytes, 0, len);
            m_offset += len;
            return Encoding.UTF8.GetString(bytes);
        }
    }
    #endregion
}
