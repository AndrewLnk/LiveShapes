using System.Collections.Generic;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup
{
    public static class AlgorithmMeshRebuild
    {
        public static void RebuildMesh(MeshFilter meshFilter, List<Vector3> positions)
        {
            var mesh = meshFilter.sharedMesh;
            if (mesh == null)
                mesh = new Mesh();

            mesh.Clear();
            mesh.SetVertices(positions);
            mesh.triangles = AlgorithmTriangulation.TriangulateCycled(positions).ToArray(); // TODO 42KB, 9 MS
            mesh.RecalculateNormals();
            meshFilter.sharedMesh = mesh;
        }
        
        public static void RewriteListWithCurvePositions(
            Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, 
            List<Vector3> input, float tDelta)
        {
            var t = 0f;

            if (v1.PointOnLine(v0, v3) && v2.PointOnLine(v0, v3))
                tDelta = 1f;

            while (t <= 1f)
            {
                input.Add(LiveMath.GetBezierPosition(v0, v1, v2, v3, t));
                t += tDelta;
            }
        }
        
        public static void RewriteListWithCurvePositions(
            Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, 
            List<Vector2> input, float tDelta)
        {
            var t = 0f;

            if (v1.PointOnLine(v0, v3) && v2.PointOnLine(v0, v3))
                tDelta = 1f;

            while (t <= 1f)
            {
                input.Add(LiveMath.GetBezierPosition(v0, v1, v2, v3, t));
                t += tDelta;
            }
        }

        public static void CleanDoublePoints(List<Vector3> positions)
        {
            for (var i = 0; i < positions.Count - 1; i++)
            {
                if (!positions[i].VectorsEquals(positions[i + 1]))
                    continue;
                
                positions.RemoveAt(i);
                i--;
            }
            
            var maxPos = positions.Count - 1;
            if (maxPos <= 1)
                return;
            
            if (positions[0].VectorsEquals(positions[maxPos])) 
                positions.RemoveAt(maxPos);
        }
        
        public static void CleanDoublePoints(List<Vector2> positions)
        {
            for (var i = 0; i < positions.Count - 1; i++)
            {
                if (!positions[i].VectorsEquals(positions[i + 1]))
                    continue;
                
                positions.RemoveAt(i);
                i--;
            }
            
            var maxPos = positions.Count - 1;
            if (maxPos <= 1)
                return;
            
            if (positions[0].VectorsEquals(positions[maxPos])) 
                positions.RemoveAt(maxPos);
        }
        
        public static void CleanPointsOnLine(List<Vector3> positions, float filterCrossProduct) // 0.001
        {
            for (var i = 1; i < positions.Count - 1; i++)
            {
                if (!positions[i].PointOnLine(positions[i - 1], positions[i + 1], filterCrossProduct))
                    continue;
                
                positions.RemoveAt(i);
                i--;
            }

            var maxPos = positions.Count - 1;
            if (maxPos <= 1)
                return;
            
            if (Mathf.Abs(LiveMath.CrossProduct(positions[maxPos - 1] - positions[maxPos], positions[0] - positions[maxPos])) < filterCrossProduct) 
                positions.RemoveAt(maxPos);
        }
        
        public static void CleanPointsOnLine(List<Vector2> positions, float filterCrossProduct) // 0.001
        {
            for (var i = 1; i < positions.Count - 1; i++)
            {
                if (!positions[i].PointOnLine(positions[i - 1], positions[i + 1], filterCrossProduct))
                    continue;
                
                positions.RemoveAt(i);
                i--;
            }

            var maxPos = positions.Count - 1;
            if (maxPos <= 1)
                return;
            
            if (Mathf.Abs(LiveMath.CrossProduct(positions[maxPos - 1] - positions[maxPos], positions[0] - positions[maxPos])) < filterCrossProduct) 
                positions.RemoveAt(maxPos);
        }
    }
}
