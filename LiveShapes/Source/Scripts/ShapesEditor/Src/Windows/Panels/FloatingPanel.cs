using System;
using LiveShapes.Source.Scripts.ShapesEditor.Images;
using LiveShapes.Source.Scripts.ShapesEditor.Images.Additional;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Windows.Panels
{
    public class FloatingPanel
    {
        private const float capHeight = 20;
        private string windowTitle;
        private Texture2D windowIcon;
        private readonly GUIStyle textStyle;
        
        private Action<Vector2> setPos;
        private Func<Vector2> getPos;
        private Action closePanelAction;

        private Func<bool> IsWindowActive { get; set; }

        public Vector2 WindowSize { get; private set; }
        public Vector2 WindowPosition
        {
            get
            {
                if (getPos == null)
                    return Vector2.zero;
                
                return getPos.Invoke();
            }
            private set => setPos?.Invoke(value);
        }

        public bool PressedCap { set; get; }
        private Vector2 savedPosition;
        private readonly bool isMovable;
        private Func<Vector2, Vector2, Vector2, Vector2> correctDelta;

        public FloatingPanel(bool isMovable)
        {
            this.isMovable = isMovable;
            textStyle = Customization.TextLabelTitleStyle;
        }

        public void SetupTitle(string text, Texture2D icon)
        {
            windowIcon = icon;
            windowTitle = text;
        }

        public void SetupWindowSize(Vector2 size)
        {
            WindowSize = size;
        }

        public void SetupPositionFunc(Action<Vector2> setPos, Func<Vector2> getPos)
        {
            this.setPos = setPos;
            this.getPos = getPos;
        }

        public void SetupCheckWindowActiveFunc(Func<bool> action) => IsWindowActive = action;
        
        public void SetupClosePanelAction(Action action) => closePanelAction = action;
        
        public void SetupCorrectDeltaFunc(Func<Vector2, Vector2, Vector2, Vector2> func) => correctDelta = func;

        public bool CheckMouseInPanel()
        {
            return MouseInput.ScreenPosition.x > WindowPosition.x &&
                   MouseInput.ScreenPosition.x < WindowPosition.x + WindowSize.x &&
                   MouseInput.ScreenPosition.y > WindowPosition.y &&
                   MouseInput.ScreenPosition.y < WindowPosition.y + WindowSize.y && 
                   IsWindowActive.Invoke();
        }

        public void CheckPressedCap()
        {
            if (!isMovable)
                return;
            
            var pos = MouseInput.ScreenPosition;
            if (pos.x > WindowPosition.x && pos.x < WindowPosition.x + WindowSize.x &&
                pos.y > WindowPosition.y && pos.y < WindowPosition.y + capHeight && 
                IsWindowActive.Invoke() && Tools.current != Tool.View)
                PressedCap = true;
            
            savedPosition = MouseInput.ScreenPosition;
        }
        
        public void ReplaceWindowByDelta(Vector2 delta)
        {
            delta = correctDelta.Invoke(delta, WindowPosition, WindowSize);
            WindowPosition -= delta;
        }

        public void Update()
        {
            DrawPlane();
            DrawImage(WindowPosition + new Vector2(2, 0), windowIcon);
            DrawTitle();
            DrawButtonClosePanel();
            ReplaceWindow();
        }

        private void DrawPlane()
        {
            var color = Customization.PanelMainColor;
            var rect = new Rect(WindowPosition, WindowSize);
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, 
                Customization.PanelBordersColor, 0, 3f);
            
            var correctedRect = new Rect(rect);
            correctedRect.position += new Vector2(0,capHeight);
            correctedRect.size -= new Vector2(0, capHeight);
            GUI.DrawTexture(correctedRect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, color, new Vector4(5f, 5f, 5f, 5f), new Vector4(0f, 0f, 5f, 5f));
            GUI.DrawTexture(correctedRect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, color, 0f, 5f);

            rect.size += new Vector2(2, 2);
            rect.position -= new Vector2(1, 1);
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, 
                Customization.PanelBordersColor, 1.5f, 5f);
        }
        
        private void DrawImage(Vector2 position, Texture2D texture2D)
        {
            var size = Vector2.one * capHeight * 0.6f;
            position += Vector2.up * (capHeight / 2f - size.y / 2f);
            position += Vector2.right * (capHeight / 2f - size.x / 2f);
            var rect = new Rect(position, size);
            
            GUI.DrawTexture(rect, texture2D);
        }
        
        private void DrawTitle()
        {
            var rectPosition = new Vector2(capHeight + 3, -0.5f) + WindowPosition;
            var rect = new Rect(rectPosition, new Vector2(WindowSize.x, capHeight));
            GUI.Box(rect, windowTitle, textStyle);
        }

        private void DrawButtonClosePanel()
        {
            var position = WindowPosition + new Vector2(WindowSize.x - capHeight, 0);
            var size = Vector2.one * capHeight * 0.6f;
            position += Vector2.up * (capHeight / 2f - size.y / 2f);
            position += Vector2.right * (capHeight / 2f - size.x / 2f);
            
            var rect = new Rect(position, size);
            if (GUI.Button(rect, SkinAdditional.Instance.close, GUIStyle.none))
                closePanelAction.Invoke();
        }

        private void ReplaceWindow()
        {
            if (!PressedCap)
                return;
            
            var delta = savedPosition - MouseInput.ScreenPosition;
            savedPosition = MouseInput.ScreenPosition;
            ReplaceWindowByDelta(delta);
        }
    }
}
