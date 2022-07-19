using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Other
{
    public static class LiveLog
    {
        public static bool HasWarning;
        
        public static void ResetWarnings()
        {
            HasWarning = false;
        }
        
        public static void LogWarning(string message)
        {
            if (!Application.isEditor || Application.isPlaying) 
                return;
            
            Debug.LogWarning("[Live Shapes] " + message);
            HasWarning = true;
        }
        
        public static void LogWriteFileWarning(string additionalInfo)
        {
            Debug.LogWarning("[Live Shapes] Failed to process file. Multiple writing is possible." +
                             $"\nMore info:" +
                             $"\n{additionalInfo}");
        }
        
        public static void LogDeserializationFileWarning(string additionalInfo)
        {
            Debug.LogError($"[Live Shapes] Failed to load .ls file." +
                           $"\nMore info:" +
                           $"\n{additionalInfo}");
        }
        
        public static void LogNoFileToLoadWarning()
        {
            Debug.LogWarning($"[Live Shapes] There is no file to load.");
        }
    }
}
