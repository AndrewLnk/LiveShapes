using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using LiveShapes.Source.Scripts.ShapesEditor.Src.UndoSystem;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools
{
    public class CurveEditor
    {
        private const float distanceCheckCurve = 0.05f;
        private readonly CurveObject curveObject;
        private readonly CurveData curveData;
        private SceneView currentView;

        private Vector2 savedPosition;
        private int removalPosId;
        private bool pressed;
        private bool checkMouseDown;
        private bool checkMouseArea;
        private int chosenPosition;
        private float dynamicDistanceCheckCurve;

        private Vector2 hoveredPosition;
        private int hoveredId;
        private float hoveredBezierPosition;
        private Func<bool> checkMouseAvailable;
        private LiveShapesUndo liveShapesUndo;

        public CurveEditor(CurveObject data)
        {
            curveData = data.Data;
            curveObject = data;
            chosenPosition = -1;
        }
        
        public void AddActionCheckAvailableMouse(Func<bool> action) => checkMouseAvailable = action;

        public void AddUndoSystem(LiveShapesUndo system) => liveShapesUndo = system;

        public void Update(SceneView view)
        {
            currentView = view;

            UpdateDynamicCheckPositionDistance();
            AutoSetupCurrentMode();
            CheckMouseDown();
            
            SyncPanelPositionWithChosenPosition();
            MoveModeProcess();
            CheckPointOnCurveProcess();
            UpdateData();
            EditCornersProcess();

            MouseDownActions();
        }

        private void AutoSetupCurrentMode()
        {
            var input = Event.current;

            if (input.control && !input.shift)
                curveData.Mode = Mode.Add;
            
            if (!input.control && input.shift)
                curveData.Mode = Mode.Remove;
            
            if (!input.control && !input.shift)
                curveData.Mode = Mode.Move;

            if (LsStatics.EditCornersMode)
                curveData.Mode = Mode.Corners;
        }

        private Mode GetModeByKeys()
        {
            var input = Event.current;
            
            if (input.control && !input.shift)
                return Mode.Add;
            
            if (!input.control && input.shift)
                return Mode.Remove;
            
            if (!input.control && !input.shift)
                return Mode.Move;

            return Mode.Empty;
        }
        
        private int CheckSelectedPosition()
        {
            var minDistance = float.PositiveInfinity;
            var result = -1;
            for (var i = 0; i < curveData.Positions.Length; i++)
            {
                var currentDistance = Vector2.Distance(MouseInput.WorldPosition, curveData.Positions[i]);
                
                if (currentDistance > minDistance) 
                    continue;
                
                if (result != -1 && i % 3 != 0)
                    continue;
                
                minDistance = currentDistance;
                if (minDistance < dynamicDistanceCheckCurve) result = i;
            }

            if (!AlgorithmsOther.SelectFilter(curveData.Mode, result))
                result = -1;

            return result;
        }

        private void CheckMouseDown()
        {
            if (MouseInput.Touched && !pressed && 
                MouseInput.ButtonPressed < 1 && !checkMouseArea &&
                Tools.current != Tool.View)
            {
                checkMouseArea = true;
                
                if (!checkMouseAvailable.Invoke())
                    return;
                
                pressed = true;
                checkMouseDown = true;
                chosenPosition = CheckSelectedPosition();
                savedPosition = MouseInput.WorldPosition;
                LsStatics.ResetChosenCurvePosition = false;
                
                if (curveData.Mode != Mode.Corners)
                    AlgorithmMovePoints.PrepareToMove(curveData.Positions, chosenPosition);

                if (curveData.Mode == Mode.Corners)
                    AlgorithmMoveCorners.PrepareToMove(curveData.Positions, chosenPosition);
            }

            if (!MouseInput.Touched && pressed)
                pressed = false;

            if (!MouseInput.Touched && checkMouseArea)
                checkMouseArea = false;
        }

        private void MoveModeProcess()
        {
            if (curveData.Mode != Mode.Move || !pressed)
                return;
            
            var delta = savedPosition - MouseInput.WorldPosition;
            savedPosition = MouseInput.WorldPosition;
            
            AlgorithmMovePoints.Move(curveData.Positions, chosenPosition, delta);
        }
        
        private void CheckPointOnCurveProcess()
        {
            if ((curveData.Mode != Mode.Add && curveData.Mode != Mode.Remove && curveData.Mode != Mode.Corners) ||
                !checkMouseAvailable.Invoke() || !LsStatics.HideUvEditor)
            {
                hoveredPosition = Vector2.positiveInfinity;
                hoveredId = -1;
                removalPosId = -1;
                
                return;
            }

            var (pos, curveId, t) = AlgorithmFindPointNearCurve.Check(curveData.Positions,
                MouseInput.WorldPosition, dynamicDistanceCheckCurve);
            
            hoveredPosition = pos;
            hoveredId = curveId;
            hoveredBezierPosition = t;
            
            removalPosId = AlgorithmsOther.GetRemovalPointId(hoveredId,
                hoveredBezierPosition, curveData.Positions.Length);
        }

        private void UpdateData()
        {
            curveData.HoverPosition = hoveredPosition;
            curveData.HoverCurveId = hoveredId;
            
            AlgorithmsOther.UpdateColoredPoint(out curveData.ColoredPosIds, 
                curveData.Mode, chosenPosition, removalPosId, curveData.Positions.Length);
            
            AlgorithmsOther.UpdateColoredCurves(out curveData.ColoredCurveIds, 
                curveData.Mode, chosenPosition, hoveredId, removalPosId, curveData.Positions);
        }
        
        private void MouseDownActions()
        {
            if (!checkMouseDown)
                return;

            checkMouseDown = false;

            if (GetModeByKeys() == Mode.Remove && removalPosId != -1)
                AlgorithmRemoveMainPoint.Remove(curveObject, removalPosId);

            if (GetModeByKeys() == Mode.Add && removalPosId != -1)
                AlgorithmSplitCurve.Split(curveObject, hoveredId, hoveredBezierPosition);
        }

        private void EditCornersProcess()
        {
            if (curveData.Mode != Mode.Corners || !pressed)
                return;
            
            var delta = savedPosition - MouseInput.WorldPosition;
            savedPosition = MouseInput.WorldPosition;

            AlgorithmMoveCorners.Move(curveData.Positions, chosenPosition, delta);
        }

        private void SyncPanelPositionWithChosenPosition()
        {
            if ((curveData.Mode == Mode.Add || curveData.Mode == Mode.Remove) && checkMouseAvailable.Invoke())
                chosenPosition = -1;

            if (curveData.Mode == Mode.Corners && !pressed)
                chosenPosition = -1;

            if (LsStatics.AllHandlesHide)
                chosenPosition = -1;
            
            if (LsStatics.HideCurve)
                chosenPosition = -1;
            
            if (LsStatics.ResetChosenCurvePosition)
                chosenPosition = -1;
            
            if (!LsStatics.HideUvEditor)
                chosenPosition = -1;
            
            if (LsStatics.HandlesHide && chosenPosition % 3 != 0)
                chosenPosition = -1;
            
            if (curveData.Positions.Length == 0 || chosenPosition >= curveData.Positions.Length)
                chosenPosition = -1;
            
            if (chosenPosition == -1)
            {
                LsStatics.CurrentHandlePosition = Vector2.positiveInfinity;
                return;
            }

            if (LsStatics.CurrentHandlePosition.Equals(Vector2.positiveInfinity))
                LsStatics.CurrentHandlePosition = curveData.Positions[chosenPosition];
            
            if (pressed)
                LsStatics.CurrentHandlePosition = curveData.Positions[chosenPosition];
            else
            {
                var oldPosition = curveData.Positions[chosenPosition];
                var delta = oldPosition - LsStatics.CurrentHandlePosition;
                AlgorithmMovePoints.Move(curveData.Positions, chosenPosition, delta);
            }
        }

        private void UpdateDynamicCheckPositionDistance()
        {
            var scalePercentage = Mathf.Clamp(currentView.cameraDistance, 0, 50) * 0.02f;
            dynamicDistanceCheckCurve = distanceCheckCurve * (0.5f + scalePercentage * 8);
        }
    }
    
    public enum Mode
    {
        Move = 0,
        Add = 1,
        Remove = 2,
        Corners = 3,
        Empty = 4
    }
}
