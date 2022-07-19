using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup
{
    public class MeshUvKeeper
    {
        private readonly MeshFilter meshFilter;
        private Vector2[] savedUvs;
        private bool active;
        
        public MeshUvKeeper(MeshFilter meshFilter) => this.meshFilter = meshFilter;

        public void SetActive(bool status)
        {
            active = status;
            
            if (!active)
                return;
            
            if (meshFilter == null)
                return;
            
            if (meshFilter.sharedMesh == null)
                return;

            savedUvs = (Vector2[])meshFilter.sharedMesh.uv.Clone();
        }

        public void Update()
        {
            if (!active)
                return;
            
            if (meshFilter == null)
                return;
            
            if (meshFilter.sharedMesh == null)
                return;
            
            if (meshFilter.sharedMesh.uv.Length != savedUvs.Length)
                return;
            
            meshFilter.sharedMesh.uv = savedUvs;
        }
    }
}
