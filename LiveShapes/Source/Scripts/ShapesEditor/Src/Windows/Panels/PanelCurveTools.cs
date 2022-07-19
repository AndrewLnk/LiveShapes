using System;
using LiveShapes.Source.Scripts.ShapesEditor.Images;
using LiveShapes.Source.Scripts.ShapesEditor.Images.CurvePanel;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Windows.Panels
{
    public class PanelCurveTools : IPanel
    {
        private const float buttonSize = 26f;
        public FloatingPanel FloatingPanel { get; }
        private readonly GUIStyle textStyle;
        private readonly GUIStyle textStyleLabel;
        private CurveObject currentObject;
        private MeshUvKeeper meshUvKeeper;

        public PanelCurveTools(FloatingPanel floatingPanel)
        {
            FloatingPanel = floatingPanel;
            floatingPanel.SetupWindowSize(new Vector2(150, 198));
            floatingPanel.SetupTitle("Curve", SkinCurvePanel.Instance.icon);
            floatingPanel.SetupPositionFunc(
                pos => LsStatics.FloatPanelPosition = pos,
                () => LsStatics.FloatPanelPosition);
            floatingPanel.SetupClosePanelAction(() =>LsStatics.ShowPanelEdit = false);
            textStyle = Customization.TextStyle;
            textStyleLabel = Customization.TextLabelPanelStyle;
        }

        public void AddUvKeeper(MeshUvKeeper uvKeeper) => meshUvKeeper = uvKeeper;

        public void UpdatePanel()
        {
            if (!LsStatics.ShowPanelEdit)
                return;
            
            FloatingPanel.Update();
        }

        public void Update(CurveObject curveObject)
        {
            if (!LsStatics.ShowPanelEdit)
                return;
            
            currentObject = curveObject;
            DrawVisibleCurveToggle();
            
            var position = 24f;
            for (var i = 0; i < 4; i++)
                DrawButton(i, position, i);

            position += buttonSize + 5f;
            DrawHorizontalLine(position);
            position += 7;
            DrawTitle("Handle Position", 7, position);
            position += 18;
            DrawPositionInput(position);
            position += 25;
            DrawHorizontalLine(position);
            position += 7;
            
            
            DrawTitle("Transform", 7, position);
            DrawTransformVisibleToggle(position);
            position += 17;
            var transformPosition = curveObject.Transform.Position;
            var rot = 360f - curveObject.Transform.Angle;
            if (rot == 360f) rot = 0f;
            DrawTransformInput(position, "", ref transformPosition);
            position += 20;
            DrawTransformInput(position, "R", ref rot);
            curveObject.Transform.Position = new Vector3(transformPosition.x, transformPosition.y);
            curveObject.Transform.Angle = 360 - rot;

            position += 25;
            DrawHorizontalLine(position);
            position += 7;
            DrawTitle("Edit Pivot", 7, position);
            DrawEditPivotToggle(position);
        }
        
        private void DrawVisibleCurveToggle()
        {
            var rectPosition = new Vector2(FloatingPanel.WindowSize.x - 36, 3.5f);
            LGUI.VisibleToggle(FloatingPanel, rectPosition, ref LsStatics.HideCurve, () =>
            {
                if (LsStatics.HideUvEditor) 
                    return;
                
                LsStatics.HideUvEditor = true;
            });
        }

        private void DrawButton(int order, float position, int buttonId)
        {
            var color = GetState(buttonId) ? 
                Customization.PanelButtonSelectedColor : 
                Customization.PanelButtonDefaultColor;
            var outlineColor = Customization.PanelButtonOutlineColor;
            var rect = new Rect(FloatingPanel.WindowPosition, Vector2.one * buttonSize);
            rect.position += new Vector2(8f + rect.size.x * order + 9.5f * order, position);
            
            var buttonRect = new Rect(rect);
            buttonRect.size -= Vector2.one * 2.5f;
            buttonRect.position += Vector2.one * 1.5f;
            LGUI.PanelButton(buttonRect, GetAction(buttonId), GetState(buttonId));
            
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, color, 0f, 5f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, outlineColor, 1.5f, 5f);

            rect.size = Vector2.one * 14f;
            rect.position += (Vector2.one * buttonSize - rect.size) / 2;
            
            GUI.DrawTexture(rect, GetIcon(buttonId));
        }

        private Texture2D GetIcon(int buttonId)
        {
            switch (buttonId)
            {
                case 0:
                    return LsStatics.EditCornersMode
                        ? SkinCurvePanel.Instance.editCornersOn
                        : SkinCurvePanel.Instance.editCornersOff; 
                case 1:
                    return SkinCurvePanel.Instance.handlesHide;
                case 2:
                    return SkinCurvePanel.Instance.onlyCurve;
                case 3:
                    return SkinCurvePanel.Instance.focusOn;
                default:
                    return Texture2D.whiteTexture;
            }
        }

        private Action GetAction(int buttonId)
        {
            switch (buttonId)
            {
                case 0:
                    return () =>
                    {
                        if (!LsStatics.HideUvEditor)
                        {
                            LsStatics.HideUvEditor = true;
                            return;
                        }
                        
                        LsStatics.EditCornersMode = !LsStatics.EditCornersMode;
                        if (LsStatics.EditCornersMode)
                        {
                            LsStatics.HandlesHide = false;
                            LsStatics.AllHandlesHide = false;
                        }
                    };
                case 1:
                    return () =>
                    {
                        if (!LsStatics.HideUvEditor)
                        {
                            LsStatics.HideUvEditor = true;
                            return;
                        }
                        
                        LsStatics.HandlesHide = !LsStatics.HandlesHide;
                        if (LsStatics.HandlesHide)
                        {
                            LsStatics.EditCornersMode = false;
                            LsStatics.AllHandlesHide = false;
                        }
                    };
                case 2:
                    return () =>
                    {
                        if (!LsStatics.HideUvEditor)
                        {
                            LsStatics.HideUvEditor = true;
                            return;
                        }
                        
                        LsStatics.AllHandlesHide = !LsStatics.AllHandlesHide;
                        if (LsStatics.AllHandlesHide)
                        {
                            LsStatics.EditCornersMode = false;
                            LsStatics.HandlesHide = false;
                        }
                    };
                case 3:
                    return () => LsStatics.ShouldFocusOn = true;
                default:
                    return null;
            }
        }

        private bool GetState(int buttonId)
        {
            switch (buttonId)
            {
                case 0:
                    return LsStatics.EditCornersMode; 
                case 1:
                    return LsStatics.HandlesHide; 
                case 2:
                    return LsStatics.AllHandlesHide;
                default:
                    return false;
            }
        }

        private void DrawHorizontalLine(float positionY)
        {
            var rect = new Rect(FloatingPanel.WindowPosition + 
                                new Vector2(0, positionY), 
                new Vector2(FloatingPanel.WindowSize.x, 0.5f));
            
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, 
                Customization.PanelBordersColor, 0f, 0f);
        }

        private void DrawTitle(string text, float positionX, float positionY)
        {
            var position = FloatingPanel.WindowPosition +
                           new Vector2(positionX, positionY);

            var rect = new Rect(position, new Vector2(50, 8));
            GUI.Box(rect, text, textStyleLabel);
        }

        private void DrawPositionInput(float positionY)
        {
            var position = FloatingPanel.WindowPosition +
                           new Vector2(7, positionY);
            
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 14, 15f));

            var input = LsStatics.CurrentHandlePosition;
            
            input = EditorGUI.Vector2Field(rect, "", input);
            LsStatics.CurrentHandlePosition = input;
            
            if (input.Equals(Vector2.positiveInfinity))
            {
                rect.size += new Vector2(0, 4);
                var color = Customization.PanelNoAvailableLayer;
                GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, color, 0f, 0f);
            }
        }

        private void DrawTransformVisibleToggle(float position)
        {
            var rectPosition = new Vector2(70, position - 1.55f);

            if (currentObject.Data.Positions.Length == 0)
                return;
            
            LGUI.VisibleToggle(FloatingPanel, rectPosition, ref currentObject.Transform.HidePivot, () =>
            {
                if (!LsStatics.HideUvEditor)
                {
                    currentObject.Transform.HidePivot = !currentObject.Transform.HidePivot;
                    LsStatics.HideUvEditor = true;
                    return;
                }

                if (currentObject.EditPivot)
                    currentObject.EditPivot = false;

                if (currentObject.Transform.HidePivot)
                    currentObject.Transform.RecalculatePivot();
                else
                    currentObject.Transform.SelectObject();
            });
        }

        private void DrawTransformInput(float positionY, string label, ref Vector2 value)
        {
            if (currentObject.Data.Positions.Length == 0)
                value = Vector2.zero;
            
            LGUI.Vector2Input(FloatingPanel, positionY, label, ref value);
            
            if (currentObject.Data.Positions.Length > 0)
                return;

            var position = FloatingPanel.WindowPosition + new Vector2(2, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 4, 18));
            var color = Customization.PanelNoAvailableLayer;
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, color, 0f, 0f);
        }

        private void DrawTransformInput(float positionY, string label, ref float value)
        {
            if (currentObject.Data.Positions.Length == 0)
                value = 0;
            
            LGUI.FloatInput(FloatingPanel, positionY, label, ref value);
            
            if (currentObject.Data.Positions.Length > 0)
                return;

            var position = FloatingPanel.WindowPosition + new Vector2(2, positionY);
            var rect = new Rect(position, new Vector2(FloatingPanel.WindowSize.x - 4, 18));
            var color = Customization.PanelNoAvailableLayer;
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, color, 0f, 0f);
        }

        private void DrawEditPivotToggle(float position)
        {
            var rectPosition = new Vector2(67, position - 0.7f);
            var active = currentObject.EditPivot;
            LGUI.ActiveToggle(FloatingPanel, rectPosition, ref active, () =>
            {
                if (!LsStatics.HideUvEditor)
                {
                    LsStatics.HideUvEditor = true;
                    return;
                }

                currentObject.EditPivot = !currentObject.EditPivot;
                meshUvKeeper?.SetActive(currentObject.EditPivot);
                
                if (currentObject.Data.Positions.Length == 0)
                    return;
                
                if (currentObject.EditPivot)
                    currentObject.Transform.HidePivot = false;
                
                if (!currentObject.EditPivot)
                    currentObject.Transform.RecalculatePivot();
            });
        }
    }
}
