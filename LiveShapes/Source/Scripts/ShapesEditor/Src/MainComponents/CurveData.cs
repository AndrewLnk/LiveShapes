using System.Collections.Generic;
using System.Linq;
using LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents
{
    public class CurveData
    {
        public Vector2[] Positions;

        public Mode Mode;
        public int[] ColoredPosIds = new int[2];
        public int[] ColoredCurveIds = new int[2];
        public Vector2 HoverPosition;
        public int HoverCurveId;

        public CurveData()
        {
            Positions = new Vector2[0];
        }

        public void ResetArray(int length)
        {
            if (length < 0)
                return;
            
            Positions = new Vector2[length];
        }
        
        public List<Vector2> ToList(Vector2 pivotPosition)
        {
            var list = Positions.Select(position => position + pivotPosition).ToList();
            return list;
        }
        
        public List<Vector2> ToListLocal(Vector2 pivotPosition)
        {
            var list = Positions.Select(position => position - pivotPosition).ToList();
            return list;
        }

        public void SetPositions(Vector2[] positionsIn, Vector2 pivotPosition)
        {
            for (var i = 0; i < positionsIn.Length; i++)
                positionsIn[i] -= pivotPosition;
            
            Positions = positionsIn;
        }
    }
}
