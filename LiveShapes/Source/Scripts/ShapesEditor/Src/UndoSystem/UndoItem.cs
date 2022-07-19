using System;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.UndoSystem
{
    [Serializable]
    public struct UndoItem
    {
        public sbyte code;

        public Vector2[] info0;
        public Vector2 info1;
        public int info2;
        public Vector4 info3;
        public Gradient info4;
        public Color info5;
    }
}
