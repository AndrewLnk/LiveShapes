using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Images.Additional
{
    public class SkinAdditional : ScriptableObject
    {
        private static SkinAdditional instance;

        public static SkinAdditional Instance
        {
            get
            {
                if (instance == null)
                    instance = CreateInstance<SkinAdditional>();
                
                return instance;
            }
        }
        
        public Texture2D close;

        public Texture2D visibleOn;
        public Texture2D visibleOff;
        
        public Texture2D toggleOn;
        public Texture2D toggleOff;
    }
}
