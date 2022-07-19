using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup
{
    public class UvEditor
    {
        private const float capRadius = 0.02f;
        private const float distanceCheckCurve = 0.05f;
        private readonly UvData uvData;
        private SceneView currentView;
        private Func<bool> checkMouseAvailable;
        private float dynamicDistanceCheckCurve;

        private Vector2 v0;
        private Vector2 v1;
        private Vector2 v2;
        private Vector2 v3;

        private bool pressed;
        private Vector2 savedPosition;
        private EditSession editSession;

        public UvEditor(UvData data)
        {
            uvData = data;
        }
        
        public void AddActionCheckAvailableMouse(Func<bool> action) => checkMouseAvailable = action;
        
        public void Update(SceneView sceneView)
        {
            if (LsStatics.HideUvEditor)
                return;
            
            currentView = sceneView;

            UpdateUvInfo();
            DrawUvFigure();
            UpdateDynamicCheckPositionDistance();
            CheckOnMouseDown();
            MoveProcess();
        }

        private void UpdateUvInfo()
        {
            v0 = uvData.Position;
            v2 = uvData.Anchor;
            var middle = (v2 - v0) / 2;
            v1 = v0 + middle + new Vector2(-middle.y, middle.x);
            v3 = v0 + middle + new Vector2(middle.y, -middle.x);
        }

        private void DrawUvFigure()
        {
            if (uvData.LocalPosition.VectorsEquals(uvData.LocalAnchor))
                return;
            
            Handles.color = CurveCustomization.GetColorByMode(Mode.Empty);
            Handles.DrawLine(v0, v1);
            Handles.DrawLine(v1, v2);
            Handles.DrawLine(v2, v3);
            Handles.DrawLine(v3, v0);
            
            Handles.color = CurveCustomization.GetUvRectColor();
            Handles.DrawAAConvexPolygon(new Vector3[]{v0, v1, v2, v3});

            Handles.color = CurveCustomization.GetColorByMode(Mode.Empty);
            var radius = CapRadius();
            Handles.DrawSolidDisc(v1, new Vector3(0,0,1), radius);
            Handles.DrawSolidDisc(v3, new Vector3(0,0,1), radius);

            Handles.color = CurveCustomization.GetColorByMode(Mode.Move);
            Handles.DrawSolidDisc(v0, new Vector3(0,0,1), radius);

            Handles.color = CurveCustomization.GetUvPivotColor();
            Handles.DrawSolidDisc(v2, new Vector3(0,0,1), radius);
        }
        
        private float CapRadius()
        {
            var scalePercentage = Mathf.Clamp(currentView.cameraDistance, 0, 50) * 0.02f;
            var value = capRadius * (0.5f + scalePercentage * 8);
            return value;
        }

        private void UpdateDynamicCheckPositionDistance()
        {
            var scalePercentage = Mathf.Clamp(currentView.cameraDistance, 0, 50) * 0.02f;
            dynamicDistanceCheckCurve = distanceCheckCurve * (0.5f + scalePercentage * 8);
        }
        
        private void CheckOnMouseDown()
        {
            if (MouseInput.Touched && !pressed && 
                MouseInput.ButtonPressed < 1 &&
                Tools.current != Tool.View && Event.current.type == EventType.MouseDown)
            {
                if (!checkMouseAvailable.Invoke())
                    return;

                pressed = true;
                
                savedPosition = MouseInput.WorldPosition;
                var contactType = GetContact();
                if (contactType == EditSession.ContactType.None)
                    return;
                
                editSession = new EditSession(uvData, contactType);
            }

            if (!MouseInput.Touched && pressed)
            {
                pressed = false;
                editSession = null;
            }
        }
        
        private void MoveProcess()
        {
            if (editSession == null || !pressed)
                return;
            
            var delta = savedPosition - MouseInput.WorldPosition;
            savedPosition = MouseInput.WorldPosition;
            
            editSession.Update(delta);
        }
        
        private EditSession.ContactType GetContact()
        {
            var checkDistance = dynamicDistanceCheckCurve * dynamicDistanceCheckCurve;

            if (LiveMath.VectorDistancePowTwo(savedPosition, v0) < checkDistance)
                return EditSession.ContactType.Position;
            
            if (LiveMath.VectorDistancePowTwo(savedPosition, v2) < checkDistance)
                return EditSession.ContactType.Anchor;
            
            if (LiveMath.VectorDistancePowTwo(savedPosition, v1) < checkDistance)
                return EditSession.ContactType.Left;
            
            if (LiveMath.VectorDistancePowTwo(savedPosition, v3) < checkDistance)
                return EditSession.ContactType.Right;
            
            if (LiveMath.CheckPointInTriangle(savedPosition, v0, v1, v2) || 
                LiveMath.CheckPointInTriangle(savedPosition, v0, v2, v3))
                return EditSession.ContactType.Position;

            return EditSession.ContactType.None;
        }
    }
}
