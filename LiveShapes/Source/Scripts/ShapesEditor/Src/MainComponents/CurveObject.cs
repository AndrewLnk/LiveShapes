using System.Collections.Generic;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents
{
    public class CurveObject
    {
        public readonly CurveData Data = new CurveData();
        public readonly UvData UvData = new UvData();
        public readonly MeshFilter MeshFilter;
        public readonly MeshRenderer MeshRenderer;
        public readonly TransformHandle Transform;

        private Vector2 savedTransformPosition;
        public bool EditPivot
        {
            get => Transform.EditPivotProcess;
            set
            {
                Transform.EditPivotProcess = value;
                UpdateSyncUvAndTransform(value);
            }
        }

        public readonly GameObject GameObject;

        public CurveObject()
        {
            GameObject = new GameObject
            {
                name = "Live Shape",
                hideFlags = HideFlags.HideAndDontSave
            };
            MeshFilter = GameObject.AddComponent<MeshFilter>();
            MeshRenderer =  GameObject.AddComponent<MeshRenderer>();
            Transform = new TransformHandle(Data, GameObject.transform);
            Transform.AddRotationAction(RotateUvWithTransform);
        }

        public void Update()
        {
            MeshRenderer.enabled = !LsStatics.HideMesh;
            Transform.Update();

            if (!EditPivot && !(UvData.LocalPosition.VectorsEquals(UvData.LocalAnchor)))
                UvData.UpdatePivotPosition(Transform.SavedPosition);
        }

        public void SetPositions(List<Vector2> positions) => Data.SetPositions(positions.ToArray(), Transform.Position);

        public void ClearUv()
        {
            UvData.LocalPosition = Vector2.zero;
            UvData.LocalAnchor = Vector2.zero;
        }
        
        public void ResetUv()
        {
            if (MeshFilter.sharedMesh == null)
                return;

            UvData.UpdatePivotPosition(Transform.SavedPosition);
            
            var (min, max) = LiveMath.GetBoundsOfPositions(MeshFilter.sharedMesh.vertices);
            var dist = max - min;
            max = min + Vector2.one * Mathf.Max(dist.x, dist.y);

            var savedAngle = Transform.Angle;
            var rotation = Quaternion.Euler(0, 0, savedAngle);
            var m = Matrix4x4.Rotate(rotation);
            min = m.MultiplyPoint3x4(min);
            max = m.MultiplyPoint3x4(max);

            UvData.LocalPosition = min;
            UvData.LocalAnchor = max;
        }

        private void UpdateSyncUvAndTransform(bool stopSync)
        {
            if (stopSync)
            {
                UvData.UpdatePivotPosition(Transform.SavedPosition);
                savedTransformPosition = Transform.SavedPosition;
            }
            else
            {
                UvData.LocalPosition += savedTransformPosition - Transform.Position;
                UvData.LocalAnchor += savedTransformPosition - Transform.Position;
                UvData.UpdatePivotPosition(Transform.Position);
            }
        }

        private void RotateUvWithTransform(float deltaAngle)
        {
            var rotation = Quaternion.Euler(0, 0, deltaAngle);
            var m = Matrix4x4.Rotate(rotation);
            
            UvData.LocalPosition = m.MultiplyPoint3x4(UvData.LocalPosition);
            UvData.LocalAnchor = m.MultiplyPoint3x4(UvData.LocalAnchor);
        }
    }
}
