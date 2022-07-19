using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.UndoSystem
{
    public class UndoData : ScriptableObject
    {
        public bool hasInfo;
        public UndoItem item;
    }
}
