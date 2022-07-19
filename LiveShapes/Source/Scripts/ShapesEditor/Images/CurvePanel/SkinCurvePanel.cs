using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Images.CurvePanel
{
    public class SkinCurvePanel : ScriptableObject
    {
        private static SkinCurvePanel instance;

        public static SkinCurvePanel Instance
        {
            get
            {
                if (instance == null)
                    instance = CreateInstance<SkinCurvePanel>();
                
                return instance;
            }
        }
    
        public Texture2D icon;
        public Texture2D editCornersOn;
        public Texture2D editCornersOff;
        public Texture2D handlesHide;
        public Texture2D onlyCurve;
        public Texture2D focusOn;
    }
}
