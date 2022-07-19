using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Images.ExportPanel
{
    public class SkinExportPanel : ScriptableObject
    {
        private static SkinExportPanel instance;

        public static SkinExportPanel Instance
        {
            get
            {
                if (instance == null)
                    instance = CreateInstance<SkinExportPanel>();
                
                return instance;
            }
        }
        
        public Texture2D icon;
    }
}
