using UnityEngine;

namespace XGameKit.Core
{
    public class XDebugLogger
    {
        public bool mute;//是否禁止
        public string prefix;//前缀
        public string color; //颜色 rrggbbaa

        public void Log(string message)
        {
            Debug.Log(_AssembleMessage(message));
        }
        public void LogWarning(string message)
        {
            Debug.LogWarning(_AssembleMessage(message));
        }
        public void LogError(string message)
        {
            Debug.LogError(_AssembleMessage(message));
        }

        //创建一个支持多行的log
        public XDebugMutiLogger CreateMutiLogger()
        {
            return new XDebugMutiLogger(this);
        }

        #region 拼接Log文本

        string _AssembleMessage(string message)
        {
            if (!string.IsNullOrEmpty(prefix))
            {
                message = string.Format("[{0}] {1}", prefix, message);
            }
            if (!string.IsNullOrEmpty(color))
            {
                //换行符会打乱染色，所以需要替换
                var lines = message.Split('\n');
                for (int i = 0; i < lines.Length; ++i)
                {
                    lines[i] = string.Format("<color=#{0}>{1}</color>", color, lines[i]);
                }
                message = string.Empty;
                for (int i = 0; i < lines.Length; ++i)
                {
                    message += lines[i];
                    if (i != lines.Length - 1)
                    {
                        message += "\n";
                    }
                }
            }
            return message;
        }

        #endregion
    }
}