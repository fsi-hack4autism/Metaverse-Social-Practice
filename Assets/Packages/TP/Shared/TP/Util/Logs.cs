using UnityEngine;

namespace TP.Util
{
    public static class Logs
    {
        public static bool disabled = false; 
        
        public static void Log(string message, bool force = false)
        {
            if (CanLog || force) Debug.Log(message);
        }
    
        public static void LogError(string message, bool force = false)
        {
            if (CanLog || force) Debug.LogError(message);
        }
    
        public static void LogWarning(string message, bool force = false)
        {
            if (CanLog || force) Debug.LogWarning(message);
        }
    
        public static void LogWarning(string message, Object target, bool force = false)
        {
            if (CanLog || force) Debug.LogWarning(message, target);
        }
    
        public delegate bool CheckCanLogDelegate();

        public static CheckCanLogDelegate CheckCanLog = () =>
        {
            if (disabled)
            {
                return false;
            }
            
            #if LOGGING || UNITY_EDITOR
                return true;
            #else
                return false;
            #endif
        };

        public static bool CanLog
        {
            get { return CheckCanLog(); }
        }
    }
}
