using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup
{
    public class LiveUvBuilder
    {
        private readonly MeshFilter meshFilter;
        private Vector2 savedPosition;
        private Vector2 savedAnchor;

        public LiveUvBuilder(MeshFilter meshFilter) => this.meshFilter = meshFilter;

        public void Update(UvData uvData, bool forceUpdate = false)
        {
            if (!ShouldRebuildUv(uvData, forceUpdate))
                return;
            
            AlgorithmMeshUvBuilder.BuildUvToMesh(meshFilter, savedPosition, savedAnchor);
        }

        private bool ShouldRebuildUv(UvData uvData, bool forceUpdate)
        {
            if (uvData.LocalPosition.VectorsEquals(savedPosition) &&
                uvData.LocalAnchor.VectorsEquals(savedAnchor) &&
                !forceUpdate)
                return false;

            savedPosition = uvData.LocalPosition;
            savedAnchor = uvData.LocalAnchor;
            return true;
        }
    }
}
