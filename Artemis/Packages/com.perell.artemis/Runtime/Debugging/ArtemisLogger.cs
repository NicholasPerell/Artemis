using UnityEngine;

namespace Perell.Artemis.Debugging
{
    public enum LogLevel
    {
        Error,
        Warning,
        Info
    }

    [System.Serializable]
    public struct Logger
    {
        [SerializeField]
        private string _prefix;

        [SerializeField]
        public LogLevel Level;

        public Logger(string prefix, Color color)
        {
            _prefix = $"<color={ColorUtility.ToHtmlStringRGBA(color)}>[{prefix}]</color>";
            Level = LogLevel.Error;
        }

        public Logger(string prefix, string color)
        {
            _prefix = $"<color={color}>[{prefix}]</color>";
            Level = LogLevel.Error;
        }

        public void Info(string message)
        {
            if (Level == LogLevel.Info)
            {
                Debug.Log($"{_prefix} {message}");
            }
        }
    }
}