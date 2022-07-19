using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Other
{
    public static class SavedParams
    {
        public static string ExportPath
        {
            get => PlayerPrefs.GetString("ExportPath_LS_Asset", string.Empty);
            set => PlayerPrefs.SetString("ExportPath_LS_Asset", value);
        }
    }
}
