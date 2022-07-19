using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData
{
    public class LiveShapeData
    {
        public string Name;
        public Vector2[] CurveLocalPositions;
        public Vector2 ShapePosition;
        public float ShapeRotation;
        public int GenerateMeshSteps;
        public int MeshOptimization;
        public Vector2 UvLocalPosition;
        public Vector2 UvLocalAnchor;

        public int SetupPainting;
        public Color PaintingColor;
        public int GradientType;
        public Gradient Gradient;
        public int GradientResolution;
        public int Texture2DId;
        public Vector4 Texture2DPosition;
        public int MaterialId;
    }
}
