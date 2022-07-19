using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools.Algorithms.SvgExporter
{
    public class SvgData
    {
        public Vector2[] Points { get; private set; }
        public bool HasGradient { get; private set; }
        public Color FillColor { get; private set; }
        public Gradient FillGradient { get; private set; }
        public int FillGradientId { get; private set; }
        public Vector2 FillGradientPivot { get; private set; }
        public Vector2 FillGradientAnchor { get; private set; }
        public bool HasStroke { get; private set; }
        public float StrokeWidth { get; private set; }
        public Color StrokeColor { get; private set; }
        public Vector2 ShapeSize { private set; get; }

        public SvgData(Vector2[] points)
        {
            this.Points = points;
        }

        public void SetupColor(Color color)
        {
            FillColor = color;
        }

        public void SetupGradient(Gradient gradient, int fillId)
        {
            if (fillId > 1)
                return;
            
            HasGradient = true;
            FillGradientId = Mathf.Clamp(fillId, 0, 1);
            FillGradient = gradient;
        }
        
        public void SetupUvForGradient(Vector2 pivot, Vector2 anchor)
        {
            FillGradientPivot = pivot;
            FillGradientAnchor = anchor;
        }

        public void SetupStroke(float width, Color color)
        {
            HasStroke = true;
            StrokeWidth = width;
            StrokeColor = color;
        }

        public void Prepare()
        {
            PreparePositions();
            NormalizeStroke();
            FindShapeSize();
            NormalizeSize();
        }

        public string GenerateCode()
        {
            var svg = string.Empty;
            AlgorithmSvgStringDeclaring.AddDeclaring(this, ref svg);
            
            AlgorithmSvgStringDeclaring.AddDefsDeclaring(ref svg);
            AlgorithmSvgStringGradient.AddGradient(this, ref svg);
            AlgorithmSvgStringDeclaring.FinishDefsDeclaring(ref svg);
            
            AlgorithmSvgStringPath.AddPath(this, ref svg);

            AlgorithmSvgStringDeclaring.FinishDeclaring(ref svg);
            return svg;
        }

        private void PreparePositions()
        {
            var gridShift = new Vector2(float.PositiveInfinity, float.NegativeInfinity);
            for (var i = 0; i < Points.Length; i++)
            {
                if (Points[i].x < gridShift.x)
                    gridShift.x = Points[i].x;

                if (Points[i].y > gridShift.y)
                    gridShift.y = Points[i].y;
            }

            for (var i = 0; i < Points.Length; i++)
                Points[i] -= gridShift;

            for (var i = 0; i < Points.Length; i++)
                Points[i] = new Vector2(Points[i].x, -Points[i].y);

            FillGradientPivot -= gridShift;
            FillGradientAnchor -= gridShift;
            FillGradientPivot = new Vector2(FillGradientPivot.x, -FillGradientPivot.y);
            FillGradientAnchor = new Vector2(FillGradientAnchor.x, -FillGradientAnchor.y);
        }

        private void NormalizeStroke()
        {
            var bounds = LiveMath.GetBoundsOfPositions(Points);
            var size = bounds.max - bounds.min;
            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);
            var minSide = Mathf.Min(size.x, size.y);
            
            if (StrokeWidth > minSide)
                StrokeWidth = minSide;
        }
        
        private void FindShapeSize()
        {
            var size = Vector2.negativeInfinity;
            for (var i = 0; i < Points.Length; i++)
            {
                if (Points[i].x > size.x)
                    size.x = Points[i].x;

                if (Points[i].y > size.y)
                    size.y = Points[i].y;
            }

            if (size.x >= 0 || size.y >= 0)
                ShapeSize = size;
        }

        private void NormalizeSize()
        {
            var maxSize = Mathf.Max(ShapeSize.x, ShapeSize.y);
            
            if (maxSize <= 0)
                return;
            
            var deltaMultiplier = maxSize / 1000;
            
            ShapeSize /= deltaMultiplier;
            StrokeWidth /= deltaMultiplier;
            FillGradientPivot /= deltaMultiplier;
            FillGradientAnchor /= deltaMultiplier;

            for (var i = 0; i < Points.Length; i++)
                Points[i] /= deltaMultiplier;
        }
    }
}
