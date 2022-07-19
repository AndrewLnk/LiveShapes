using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Images.MeshPanel
{
    public class SkinMeshPanel : ScriptableObject
    {
        private static SkinMeshPanel instance;

        public static SkinMeshPanel Instance
        {
            get
            {
                if (instance == null)
                    instance = CreateInstance<SkinMeshPanel>();
                
                return instance;
            }
        }
        
        public Texture2D icon;
        public Texture2D reset;
        public Texture2D resetActive;
    }
}
