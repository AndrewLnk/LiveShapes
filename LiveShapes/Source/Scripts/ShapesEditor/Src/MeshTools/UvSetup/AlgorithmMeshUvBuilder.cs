using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;
using UnityEngine.Profiling;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup
{
    public static class AlgorithmMeshUvBuilder
    {
        public static void BuildUvToMesh(MeshFilter meshFilter, Vector2 uvMin, Vector2 uvMax)
        {
            if (meshFilter == null)
                return;

            var mesh = meshFilter.sharedMesh;
            
            if (mesh == null)
                return;
            
            if (uvMin.VectorsEquals(uvMax))
            {
                mesh.uv = new Vector2[0];
                return;
            }
            
            var rotationPivots = Quaternion.Euler(0, 0, -meshFilter.transform.localEulerAngles.z);
            var mPivots = Matrix4x4.Rotate(rotationPivots);
            uvMax = mPivots.MultiplyPoint3x4(uvMax);
            uvMin = mPivots.MultiplyPoint3x4(uvMin);
            uvMax -= uvMin;
            
            Profiler.BeginSample("Test 1");
            var uvs = new Vector2[mesh.vertices.Length];
            for (var i = 0; i < mesh.vertexCount; i++)
                uvs[i] = new Vector2(mesh.vertices[i].x, mesh.vertices[i].y) - uvMin; // TODO Allocation
            Profiler.EndSample();
            var angle = LiveMath.AngleOfTriangle(Vector2.right, Vector2.zero, uvMax) - 45;
            var rotation = Quaternion.Euler(0, 0, -angle);
            var m = Matrix4x4.Rotate(rotation);

            uvMax = m.MultiplyPoint3x4(uvMax);
            for (var i = 0; i < uvs.Length; i++)
                uvs[i] = m.MultiplyPoint3x4(uvs[i]) / uvMax;
            
            mesh.uv = uvs;
        }
    }
}
