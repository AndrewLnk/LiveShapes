using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData
{
    public class LiveShapeDataObject : ScriptableObject
    {
        [HideInInspector] [SerializeField] 
        public string AssetName;
        [HideInInspector] [SerializeField] 
        public Vector2[] CurveLocalPositions;
        [HideInInspector] [SerializeField] 
        public int GenerateMeshSteps;
        [HideInInspector] [SerializeField] 
        public int MeshOptimization;
        [HideInInspector] [SerializeField] 
        public Vector2 UvLocalPosition;
        [HideInInspector] [SerializeField]
        public Vector2 UvLocalAnchor;
        [HideInInspector] [SerializeField] 
        public int SetupPainting;
        [HideInInspector] [SerializeField] 
        public Color PaintingColor;
        [HideInInspector] [SerializeField] 
        public int GradientType;
        [HideInInspector] [SerializeField] 
        public Gradient Gradient;
        [HideInInspector] [SerializeField] 
        public int GradientResolution;
        [HideInInspector] [SerializeField] 
        public int Texture2DId;
        [HideInInspector] [SerializeField] 
        public Vector4 Texture2DPosition;
        [HideInInspector] [SerializeField] 
        public int MaterialId;

        public void Setup(LiveShapeData data)
        {
            AssetName = data.Name;
            CurveLocalPositions = data.CurveLocalPositions;
            GenerateMeshSteps = data.GenerateMeshSteps;
            MeshOptimization = data.MeshOptimization;
            UvLocalPosition = data.UvLocalPosition;
            UvLocalAnchor = data.UvLocalAnchor;
            SetupPainting = data.SetupPainting;
            PaintingColor = data.PaintingColor;
            GradientType = data.GradientType;
            Gradient = data.Gradient;
            GradientResolution = data.GradientResolution;
            Texture2DId = data.Texture2DId;
            Texture2DPosition = data.Texture2DPosition;
            MaterialId = data.MaterialId;
        }
    }
}
