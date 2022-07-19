using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents
{
    public class TransformHandle
    {
        private readonly CurveData curveData;
        private readonly Transform transform;

        private Action<float> rotationAction;
        private Func<bool> handleMovable;
        private float savedAngles;
        public bool EditPivotProcess { set; get; }
        public bool HidePivot = true;

        public Vector2 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector2 SavedPosition { get; private set; }

        public float Angle
        {
            get => transform.localEulerAngles.z;
            set
            {
                var v = transform.localEulerAngles;
                v.z = value;
                transform.localEulerAngles = v;
            }
        }

        public TransformHandle(CurveData curveData, Transform transform)
        {
            this.curveData = curveData;
            this.transform = transform;
            
            SavedPosition = transform.position;
            savedAngles = 0f;
        }

        public void RecalculatePivot()
        {
            SavedPosition = transform.position;
            savedAngles = transform.localEulerAngles.z;
        }
        
        public List<Vector2> GetPositionsList() => curveData.ToList(SavedPosition);
        
        public void SetupPivot(Vector2 position)
        {
            var curvePositions = curveData.ToList(position);
            curveData.SetPositions(curvePositions.ToArray(), position);
            ResetPivot(position);
            
            LsStatics.ResetChosenCurvePosition = true;
        }
        
        public void ResetPositions(int length)
        {
            curveData.ResetArray(length);
            ResetPivot(Vector2.zero);
            
            LsStatics.ResetChosenCurvePosition = true;
        }

        public List<Vector2> GetLocalPositionsList()
        {
            var m = Matrix4x4.Rotate(Quaternion.Euler(0, 0, -savedAngles));
            var result = curveData.ToListLocal(SavedPosition);
            for (var i = 0; i < curveData.Positions.Length; i++)
                result[i] = m.MultiplyPoint3x4(result[i]);

            return result;
        }

        public void AddRotationAction(Action<float> action) => rotationAction = action;
        
        public void AddCheckHandleMovableAction(Func<bool> action) => handleMovable = action;
        
        public void Destroy()
        {
            if (transform == null)
                return;
            
            Object.DestroyImmediate(transform.gameObject);
        }
        
        public void SelectObject()
        {
            Selection.activeTransform = null;
            HidePivot = false;
        }

        public void Update()
        {
            EditPivotModeProcess();
            SyncPositionProcess();
            SyncAnglesProcess();
            DrawTransformHandle();
            KeepRotationOnlyZAxis();
        }
        
        private void SyncPositionProcess()
        {
            if (EditPivotProcess)
                return;

            if (SavedPosition == (Vector2)transform.position)
                return;

            var delta = SavedPosition - (Vector2) transform.position;
            SavedPosition = transform.position;
            
            for (var i = 0; i < curveData.Positions.Length; i++)
                curveData.Positions[i] -= delta;
            
            LsStatics.ResetChosenCurvePosition = true;
        }
        
        private void SyncAnglesProcess()
        {
            if (EditPivotProcess)
                return;
            
            if (Mathf.Abs(savedAngles - transform.eulerAngles.z) < 0.01)
                return;
            
            var delta = transform.eulerAngles.z - savedAngles;
            savedAngles = transform.eulerAngles.z;
            
            var rotation = Quaternion.Euler(0, 0, delta);
            var m = Matrix4x4.Rotate(rotation);

            for (var i = 0; i < curveData.Positions.Length; i++)
            {
                curveData.Positions[i] -= SavedPosition;
                curveData.Positions[i] = m.MultiplyPoint3x4(curveData.Positions[i]);
                curveData.Positions[i] += SavedPosition;
            }
            rotationAction?.Invoke(delta);
            
            LsStatics.ResetChosenCurvePosition = true;
        }
        
        private void DrawTransformHandle()
        {
            if (HidePivot || !LsStatics.HideUvEditor)
                return;

            var savedPosition = transform.position;
            var savedRotation = transform.rotation;

            if (Tools.current == Tool.Move)
                transform.position = Handles.PositionHandle(transform.position, transform.rotation);
            else if (Tools.current == Tool.Rotate)
                transform.rotation = Handles.RotationHandle(transform.rotation, transform.position);

            if (savedPosition != transform.position ||
                savedRotation != transform.rotation)
                UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

            if (handleMovable.Invoke())
            {
                transform.position = savedPosition;
                transform.rotation = savedRotation;
            }
        }
        
        private void EditPivotModeProcess()
        {
            if (!EditPivotProcess)
                return;

            SavedPosition = transform.position;
            savedAngles = transform.localEulerAngles.z;
        }
        
        private void ResetPivot(Vector2 position)
        {
            transform.position = position;
            transform.localScale = Vector3.one;
            transform.eulerAngles = Vector3.zero;
            
            savedAngles = 0f;
            SavedPosition = transform.position;
        }

        private void KeepRotationOnlyZAxis()
        {
            transform.eulerAngles = new Vector3(0,0, transform.eulerAngles.z);
        }
    }
}
