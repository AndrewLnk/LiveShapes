using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Images
{
    public static class Customization
    {
        public static readonly Color32 SceneViewPivotX = new Color32(157, 27, 27, 255);
        public static readonly Color32 SceneViewPivotY = new Color32(27, 157, 27, 255);
        public static Color32 PressedButtonColor => EditorGUIUtility.isProSkin ?
            new Color32(75, 75, 75, 255) : new Color32(125, 125, 125, 115);
        
        public static Color32 PanelBordersColor => EditorGUIUtility.isProSkin ?
            new Color32(43, 43, 43, 255) : new Color32(132, 132, 132, 255);

        public static Color32 PanelMainColor => EditorGUIUtility.isProSkin ?
            new Color32(63, 63, 63, 255) : new Color32(194, 194, 194, 255);

        public static Color32 PanelNoAvailableLayer => EditorGUIUtility.isProSkin ?
            new Color32(63, 63, 63, 177) : new Color32(194, 194, 194, 177);
        
        public static Color32 PanelButtonSelectedColor => EditorGUIUtility.isProSkin ?
            new Color32(77, 77, 77, 255) : new Color32(105, 105, 105, 255);
        
        public static Color32 PanelButtonOutlineColor => EditorGUIUtility.isProSkin ?
            new Color32(87, 87, 87, 255) : new Color32(105, 105, 105, 255);
        public static Color32 PanelButtonDefaultColor => EditorGUIUtility.isProSkin ?
            new Color32(15, 15, 15, 100) : new Color32(77, 77, 77, 110);
        
        public static Color32 PanelTextAdditionalColor => EditorGUIUtility.isProSkin ?
            new Color32(255, 255, 255, 98) : new Color32(80, 80, 80, 255);
        public static Color32 PanelWhiteBorderColor => EditorGUIUtility.isProSkin ?
            new Color32(71, 71, 71, 255) : new Color32(147, 147, 147, 255);
        public static Color32 PanelTextInputFieldColor => EditorGUIUtility.isProSkin ?
            new Color32(255, 255, 255, 128) : new Color32(90, 90, 90, 255);

        public static GUIStyle TextStyle => new GUIStyle
        {
            fontSize = 10,
            normal = {textColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.7f) : new Color(0, 0, 0, 0.7f) },
            alignment = TextAnchor.MiddleLeft
        };
        
        public static GUIStyle TextLabelTitleStyle => new GUIStyle
        {
            fontSize = 11,
            normal = {textColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.6f) : new Color(0, 0, 0, 0.85f) },
            alignment = TextAnchor.MiddleLeft,
        };
        
        public static GUIStyle TextPanelAdditionalStyle => new GUIStyle
        {
            fontSize = 11,
            normal = {textColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.4f) : new Color(0, 0, 0, 0.6f)},
            alignment = TextAnchor.MiddleLeft,
        };
        
        public static GUIStyle TextLabelPanelStyle => new GUIStyle
        {
            fontSize = 11,
            normal = {textColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.75f) : new Color(0, 0, 0, 0.75f)},
            alignment = TextAnchor.MiddleLeft,
        };
        
        public static GUIStyle ButtonStyle => new GUIStyle(GUI.skin.button)
        {
            fontSize = 10
        };
    }
}
