using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms
{
    public static class AlgorithmCurveInversionCheck
    {
        public static void FixInversion(Vector2[] positions)
        {
            for (var i = 0; i < positions.Length - 6; i+=6)
            {
                if (!LiveMath.IsTriangleOrientedClockwise(positions[i], positions[i + 3], positions[i + 6])) 
                    continue;
                
                return;
            }

            var maxId = positions.Length - 1;
            for (var i = 0; i < maxId / 2; i++)
            {
                var v = positions[i];
                positions[i] = positions[maxId - i];
                positions[maxId - i] = v;
            }
        }
    }
}
