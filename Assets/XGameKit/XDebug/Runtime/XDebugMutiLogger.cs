

namespace XGameKit.Core
{
    public struct XDebugMutiLogger
    {
        private XDebugLogger _logger;
        private string _message;
        private bool _mute;

        public XDebugMutiLogger(XDebugLogger logger)
        {
            _logger = logger;
            _message = string.Empty;
            _mute = logger.mute;
        }

        public void Append(string message)
        {
            if (_mute) return;
            if (string.IsNullOrEmpty(_message))
                _message = message;
            else
                _message = _message + "\n" + message;
        }
        public void Log()
        {
            if (_mute) return;
            _logger.Log(_message);
        }
        public void LogWarning()
        {
            if (_mute) return;
            _logger.LogWarning(_message);
        }
        public void LogError()
        {
            if (_mute) return;
            _logger.LogError(_message);
        }
    }
}