using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Images.MainPanel
{
    public class SkinMainPanel : ScriptableObject
    {
        private static SkinMainPanel instance;

        public static SkinMainPanel Instance
        {
            get
            {
                if (instance == null)
                    instance = CreateInstance<SkinMainPanel>();
                
                return instance;
            }
        }
    
        public Texture2D icon;
        public Texture2D warning;
    }
}
