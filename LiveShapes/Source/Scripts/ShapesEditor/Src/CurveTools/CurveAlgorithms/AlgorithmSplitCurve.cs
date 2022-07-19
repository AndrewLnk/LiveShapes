using System.Collections.Generic;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms
{
    public static class AlgorithmSplitCurve
    {
        public static void Split(CurveObject curveObject, int insertId, float tPosition)
        {
            var list = curveObject.Transform.GetPositionsList();
            
            var insertPositions = GetSplitCurvePoints(
                list[insertId], list[insertId + 1], list[insertId + 2], list[insertId + 3],
                tPosition);
            
            InsertNewPosition(
                curveObject, 
                insertPositions, 
                insertId,
                tPosition);
        }

        private static IEnumerable<Vector2> GetSplitCurvePoints(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, float t)
        {
            var result = new Vector2[3];
            
            var v11 = (v1 - v0) * t + v0;
            var v21 = (v2 - v1) * t + v1;
            var v31 = (v3 - v2) * t + v2;

            result[0] = (v21 - v11) * t + v11;
            result[2] = (v31 - v21) * t + v21;
            result[1] = (result[2] - result[0]) * t + result[0];
            
            return result;
        }

        private static void InsertNewPosition(CurveObject curveObject, 
            IEnumerable<Vector2> insertPart, int insertCurveId, float tPosition)
        {
            var list = curveObject.Transform.GetPositionsList();
            
            list[insertCurveId + 1] = Vector2.Lerp(list[insertCurveId],
                list[insertCurveId + 1], tPosition);
            list[insertCurveId + 2] = Vector2.Lerp(list[insertCurveId + 3],
                list[insertCurveId + 2], 1 - tPosition);
            list.InsertRange(insertCurveId + 2, insertPart);
            
            curveObject.SetPositions(list);
        }
    }
}
