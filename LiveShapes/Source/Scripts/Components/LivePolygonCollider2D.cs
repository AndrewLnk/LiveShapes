using System.Collections.Generic;
using System.Linq;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData;
using UnityEngine;

namespace LiveShapes.Source.Scripts.Components
{
    public class LivePolygonCollider2D : MonoBehaviour
    {
        public LiveShapeDataObject data;
        
        [Header("Settings")]
        [Range(1,100)]
        public int meshGenerateSteps = 1;
        [Range(0,100)]
        public int meshOptimization;

        private PolygonCollider2D polygonCollider2D;
        private MeshFilter meshFilter;
        public MeshFilter MeshFilter => meshFilter;

        [ContextMenu("Recalculate")]
        public void Recalculate()
        {
            FindOrCreateComponents();

            var builder = new LiveMeshBuilder(null);
            builder.GenerateMeshSteps = meshGenerateSteps;
            builder.MeshOptimization = meshOptimization;
            var points = new List<Vector2>();
            builder.GeneratePoints(data.CurveLocalPositions.ToList(), points);
            polygonCollider2D.SetPath(0, points);
        }

        public void FindOrCreateComponents()
        {
            if (polygonCollider2D == null)
            {
                polygonCollider2D = gameObject.AddComponent<PolygonCollider2D>();
                UnityEditorInternal.ComponentUtility.MoveComponentUp(polygonCollider2D);
            }
            
            if (meshFilter == null)
                meshFilter = gameObject.GetComponent<MeshFilter>();
        }
    }
}
