using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup
{
    public class UvData
    {
        private Vector2 pivot;
        public Vector2 LocalAnchor;
        public Vector2 LocalPosition;

        public float LocalAngle
        {
            get
            {
                var angle = LiveMath.AngleOfTriangle(LocalPosition + Vector2.right, LocalPosition, LocalAnchor) - 45;
                if (angle < 0)
                    angle = 360 + angle;
                return angle;
            }
            set
            {
                var distance = Vector2.Distance(LocalAnchor, LocalPosition);
                var rotationPivots = Quaternion.Euler(0, 0, value + 45);
                var mPivots = Matrix4x4.Rotate(rotationPivots);
                var v = mPivots.MultiplyPoint3x4(new Vector2(distance, 0));
                LocalAnchor = LocalPosition + (Vector2)v;
            }
        }

        public Vector2 Position
        {
            get => LocalPosition + pivot;
            set => LocalPosition = value - pivot;
        }
        
        public Vector2 Anchor
        {
            get => LocalAnchor + pivot;
            set => LocalAnchor = value - pivot;
        }

        public float UVShapeScale
        {
            get
            {
                var middle = (Anchor - Position) / 2;
                var pos = Position + middle + new Vector2(-middle.y, middle.x);
                return Vector2.Distance(Position, pos);
            }
            set => Anchor = Vector2.LerpUnclamped(Position, Anchor, value / UVShapeScale);
        }

        public void UpdatePivotPosition(Vector2 pivotPosition) => pivot = pivotPosition;
    }
}
