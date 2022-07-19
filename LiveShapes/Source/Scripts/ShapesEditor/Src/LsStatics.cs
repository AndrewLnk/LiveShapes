using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src
{
    public static class LsStatics
    {
        public static bool EditCornersMode;
        public static bool HandlesHide;
        public static bool AllHandlesHide;
        public static bool ShouldFocusOn;
        public static bool HideCurve;
        public static bool ResetChosenCurvePosition;
        public static bool HideMesh;
        public static bool HideUvEditor;

        public static bool ShowPanelCreate;
        public static bool ShowPanelEdit;
        public static bool ShowPanelMesh;
        public static bool ShowPanelExport;

        public static Vector2 FloatPanelPosition;

        public static Vector2 CurrentHandlePosition;

        public static void ResetPanelsPosition()
        {
            FloatPanelPosition = new Vector2(5, 65);
        }
    }
}
