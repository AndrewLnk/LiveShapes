using LiveShapes.Source.Scripts.ShapesEditor.Images;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools
{
    public class CurveRenderer
    {
        private const float circleRadius = 0.02f;
        private const float pivotRadius = 0.01f;
        private const float dotSize = 0.05f;
        private const float linesWidth = 2f;
        private const float curveWidth = 3f;
        
        private readonly CurveObject curveObject;
        private CurveData CurveData => curveObject.Data;
        
        private SceneView currentView;
        private Vector2 minViewPosition;
        private Vector2 maxViewPosition;
        
        public CurveRenderer(CurveObject curveObject) => this.curveObject = curveObject;

        public void Update(SceneView window)
        {
            currentView = window;

            PrepareToCheckVisible();
            DrawScenePivot();
            DrawLines();
            DrawCurves();
            DrawPositions();
            DrawAdditionalPointOnLine();
            DrawObjectPivot();

            currentView.Repaint();
        }

        private void DrawScenePivot()
        {
            Handles.color = Customization.SceneViewPivotY;
            Handles.DrawLine(new Vector2(0, -1000000), new Vector2(0, 1000000));
            Handles.color = Customization.SceneViewPivotX;
            Handles.DrawLine(new Vector2(-1000000, 0), new Vector2(1000000, 0));
        }

        private void DrawLines()
        {
            if (LsStatics.HandlesHide || LsStatics.AllHandlesHide || LsStatics.HideCurve || !LsStatics.HideUvEditor)
                return;

            var positions = CurveData.Positions;
            for (var i = 1; i < positions.Length; i++)
            {
                if ((i + 1) % 3 == 0)
                    continue;
                
                var from = positions[i];
                var to = positions[i - 1];
                
                if (LiveMath.VectorDistancePowTwo(from, to) < 0.002f)
                    continue;

                if (!IsPointVisible(from) && !IsPointVisible(to))
                    continue;

                Handles.DrawBezier(from, to, 
                                    from, to, 
                                    CurveCustomization.GetCurveLineColor(), 
                                    null, ConvertLinesLocalScaleToScene(linesWidth));
            }
        }

        private void DrawCurves()
        {
            if (LsStatics.HideCurve)
                return;
            
            for (var i = 0; i < CurveData.Positions.Length; i += 3)
            {
                if (i + 3 >= CurveData.Positions.Length)
                    break;

                var color = CurveCustomization.GetColorOfCurveByMode(CurveData, i);

                var v0 = CurveData.Positions[i];
                var v1 = CurveData.Positions[i + 1];
                var v2 = CurveData.Positions[i + 2];
                var v3 = CurveData.Positions[i + 3];
                
                if (!IsPointVisible(v0) && !IsPointVisible(v1) && !IsPointVisible(v2) && !IsPointVisible(v3))
                    continue;

                if (LiveMath.VectorDistancePowTwo(v0, v1) +
                    LiveMath.VectorDistancePowTwo(v1, v2) +
                    LiveMath.VectorDistancePowTwo(v2, v3) +
                    LiveMath.VectorDistancePowTwo(v3, v0) < 0.005f)
                    continue;
                
                Handles.DrawBezier(v0, v3,
                    v1, v2,
                    color, null, ConvertLinesLocalScaleToScene(curveWidth));
            }
        }

        private void DrawPositions()
        {
            if (LsStatics.AllHandlesHide || LsStatics.HideCurve || !LsStatics.HideUvEditor)
                return;
            
            for (var i = 0; i < CurveData.Positions.Length; i++)
            {
                if (LsStatics.HandlesHide)
                    break;
                
                if (i % 3 == 0) 
                    continue;
                
                if (!IsPointVisible(CurveData.Positions[i]))
                    continue;
                
                CurveCustomization.SetDefaultPivotsColor(CurveData, i);
                DrawCircle(CurveData.Positions[i]);
            }
            
            for (var i = 0; i < CurveData.Positions.Length; i++)
            {
                if (i % 3 != 0) 
                    continue;
                
                if (!IsPointVisible(CurveData.Positions[i]))
                    continue;
                
                CurveCustomization.SetDefaultPivotsColor(CurveData, i);
                DrawDot(CurveData.Positions[i]);
            }
        }

        private void DrawAdditionalPointOnLine()
        {
            if (LsStatics.HideCurve || !LsStatics.HideUvEditor)
                return;
            
            if (CurveData.Mode != Mode.Add)
                return;
            
            if (CurveData.HoverCurveId == -1)
                return;
            
            CurveCustomization.SetDefaultPivotsColor(CurveData, -1, true);
            DrawDot(CurveData.HoverPosition);
        }
        
        private void DrawDot(Vector2 screenPoint)
        {
            var localSize = Vector2.one * ConvertHandlesLocalScaleToScene(dotSize);
            var rect = new Rect(screenPoint - localSize / 2, localSize);
            Handles.DrawSolidRectangleWithOutline(rect, Color.white, Color.white * 0);
        }

        private void DrawCircle(Vector2 position)
        {
            var localRadius = ConvertHandlesLocalScaleToScene(circleRadius);
            Handles.DrawSolidDisc(position, new Vector3(0,0,1), localRadius);
        }

        private float ConvertHandlesLocalScaleToScene(float value)
        {
            var scalePercentage = Mathf.Clamp(currentView.cameraDistance, 0, 50) * 0.02f;
            value *= (0.5f + scalePercentage * 8);
            return value;
        }
        
        private float ConvertLinesLocalScaleToScene(float value)
        {
            var scalePercentage = Mathf.Clamp(currentView.cameraDistance, 0, 50) * 0.02f;
            value *= (1.5f - scalePercentage);
            return value;
        }

        private void DrawObjectPivot()
        {
            if (curveObject.Transform.HidePivot || !LsStatics.HideUvEditor)
                return;
            
            Handles.color = CurveCustomization.GetPivotColor();
            var radius = ConvertHandlesLocalScaleToScene(pivotRadius);
            Handles.DrawSolidDisc(curveObject.Transform.Position, new Vector3(0,0,1), radius);
        }

        private void PrepareToCheckVisible()
        {
            var camPos = currentView.camera.gameObject.transform.position;
            var camHeight = currentView.camera.orthographicSize;
            var camWidth = camHeight * currentView.camera.aspect;
            minViewPosition = new Vector2(camPos.x - camWidth, camPos.y - camHeight);
            maxViewPosition = new Vector2(camPos.x + camWidth, camPos.y + camHeight);
        }

        private bool IsPointVisible(Vector2 point)
        {
            if (point.x >= minViewPosition.x && 
                point.x <= maxViewPosition.x && 
                point.y >= minViewPosition.y && 
                point.y <= maxViewPosition.y)
                return true;

            return false;
        }
    }
}
