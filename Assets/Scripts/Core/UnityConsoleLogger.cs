using UnityEngine;

namespace Core.Infrastructure.Logger
{
    public class UnityConsoleLogger : ILogger
    {
        public void Error(object mess, object[] args)
        {
#if UNITY_EDITOR || DEBUG
            Debug.LogErrorFormat(mess.ToString(), args);
#endif
        }

        public void Log(object mess, object[] args)
        {
#if UNITY_EDITOR || DEBUG
            Debug.LogFormat(mess.ToString(), args);
#endif
        }

        public void Warning(object mess, object[] args)
        {
#if UNITY_EDITOR || DEBUG
            Debug.LogWarningFormat(mess.ToString(), args);
#endif
        }
    }
}
