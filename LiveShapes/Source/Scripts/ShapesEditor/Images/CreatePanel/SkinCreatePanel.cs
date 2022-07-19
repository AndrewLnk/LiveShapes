using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Images.CreatePanel
{
    public class SkinCreatePanel : ScriptableObject
    {
        private static SkinCreatePanel instance;

        public static SkinCreatePanel Instance
        {
            get
            {
                if (instance == null)
                    instance = CreateInstance<SkinCreatePanel>();
                
                return instance;
            }
        }
        
        public Texture2D icon;
        
        public Texture2D createRect;
        public Texture2D createCircle;
        public Texture2D createTriangle;
        public Texture2D removeShape;
    }
}
