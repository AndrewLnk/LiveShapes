using System;
using LiveShapes.Source.Scripts.ShapesEditor.Images;
using LiveShapes.Source.Scripts.ShapesEditor.Images.Additional;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Windows.Panels;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Windows
{
    public static class LGUI
    {
        public static void PanelButton(Rect rect, Action action, bool selected)
        {
            var style = new GUIStyle
            {
                active = {background = Texture2D.whiteTexture}
            };
            
            var oldColor = GUI.color;
            var newColor = Customization.PressedButtonColor;
            GUI.color = newColor;
            
            if (GUI.Button(rect, GUIContent.none, style))
                action?.Invoke();
            
            GUI.color = oldColor;
        }
        
        public static void VisibleToggle(FloatingPanel panel, Vector2 position, ref bool active, Action action)
        {
            var rect = new Rect(panel.WindowPosition + position, new Vector2(14,14));
            
            var icon = active
                ? SkinAdditional.Instance.visibleOff
                : SkinAdditional.Instance.visibleOn;

            if (GUI.Button(rect, icon, GUIStyle.none))
            {
                active = !active;
                action?.Invoke();
            }
        }
        
        public static void ActiveToggle(FloatingPanel panel, Vector2 position, ref bool active, Action action)
        {
            var rect = new Rect(panel.WindowPosition + position, new Vector2(12,12));
            
            var icon = active
                ? SkinAdditional.Instance.toggleOn
                : SkinAdditional.Instance.toggleOff;

            if (GUI.Button(rect, icon, GUIStyle.none))
            {
                active = !active;
                action?.Invoke();
            }
        }
        
        public static void FloatInput(FloatingPanel panel, float positionY, string label, ref float value)
        {
            var position = panel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(panel.WindowSize.x - 14, 15f));
            
            GUI.Box(rect, label, Customization.TextLabelPanelStyle);
            rect.position += new Vector2(13, 1);
            rect.size += new Vector2(-13, 0);
            value = EditorGUI.FloatField(rect, value);
        }
        
        public static void Vector2Input(FloatingPanel panel, float positionY, string label, ref Vector2 value)
        {
            var position = panel.WindowPosition + new Vector2(7, positionY);
            var rect = new Rect(position, new Vector2(panel.WindowSize.x - 14, 15f));

            if (label.Length > 0)
            {
                GUI.Box(rect, label, Customization.TextPanelAdditionalStyle);
                rect.position += new Vector2(0, 18); 
            }
            
            value = EditorGUI.Vector2Field(rect, "", value);
        }
    }
}
