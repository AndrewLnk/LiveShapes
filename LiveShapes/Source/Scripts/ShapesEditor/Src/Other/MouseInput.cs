using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Other
{
    public static class MouseInput
    {
        public static Vector2 WorldPosition
        {
            get
            {
                var mousePosition = Event.current.mousePosition;
                var ray = HandleUtility.GUIPointToWorldRay(mousePosition);
                return ray.GetPoint(0);
            }
        }

        public static Vector2 ScreenPosition => Event.current.mousePosition;
        
        public static Vector2 Delta;
        public static int ButtonPressed;
        public static bool Touched;

        public static void Update()
        {
            var input = Event.current;
            ButtonPressed = input.button;
            Delta = input.delta;

            OnSceneGUI();
        }

        private static void OnSceneGUI()
        {
            var e = Event.current;
            

            if (e.type == EventType.Layout || e.type == EventType.Repaint)
                return;

            if (e.type == EventType.MouseDown && e.button < 1)
                Touched = true;

            if (e.type == EventType.MouseUp || 
                e.type == EventType.Ignore || 
                e.type == EventType.MouseLeaveWindow)
                Touched = false;

            if (e.type == EventType.MouseDown)
                GUIUtility.hotControl = 0;
        }
    }
}
