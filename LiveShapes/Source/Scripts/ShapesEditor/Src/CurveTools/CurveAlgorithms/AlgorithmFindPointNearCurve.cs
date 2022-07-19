using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms
{
    public static class AlgorithmFindPointNearCurve
    {
        public static (Vector2 pos, int curveId, float t) Check(Vector2[] positions, Vector2 point, float checkDistance)
        {
            var minDistance = float.PositiveInfinity;
            var selectedCurve = 0;
            var posResult = Vector2.positiveInfinity;
            var tResult = -1f;
            var activeCurves = CurvesOnPoint(positions, point, checkDistance);

            for (var i = 0; i < activeCurves.Length; i++)
            {
                var nearestPoint = FindNearestPositionOnCurve(
                    positions[activeCurves[i]],
                    positions[activeCurves[i] + 1],
                    positions[activeCurves[i] + 2],
                    positions[activeCurves[i] + 3],
                        point);

                var distance = Vector2.Distance(nearestPoint.posResult, point);
                if (distance > minDistance) 
                    continue;
                
                minDistance = distance;
                posResult = nearestPoint.posResult;
                tResult = nearestPoint.tResult;
                selectedCurve = i;
            }
            
            if (activeCurves.Length == 0)
                return (posResult, -1, -1f);

            if (minDistance > checkDistance)
                return (Vector2.positiveInfinity, -1, -1f);
            
            return (posResult, activeCurves[selectedCurve], tResult);
        }

        private static int[] CurvesOnPoint(Vector2[] positions, Vector2 point, float checkDistance)
        {
            var result = new int[0];
            for (var i = 0; i < positions.Length; i += 3)
            {
                if (i + 3 >= positions.Length)
                    break;

                if (!AlgorithmIsPointInQuad.Check(
                    positions[i],
                    positions[i + 1],
                    positions[i + 2],
                    positions[i + 3],
                    point, checkDistance)) 
                    continue;

                var currentId = result.Length;
                Array.Resize(ref result, currentId + 1);
                result[currentId] = i;
            }

            return result;
        }
        
        private static (Vector2 posResult, float tResult) FindNearestPositionOnCurve(
            Vector2 v0, 
            Vector2 v1, 
            Vector2 v2, 
            Vector2 v3,
            Vector2 point)
        {
            var tMin = 0f;
            var tMax = 1f;
            var tResult = 0f;
            var dynamicCheckDistance = Vector2.Distance(v0, LiveMath.GetBezierPosition(v0, v1, v2, v3, 0.05f));

            tResult = FindNearestPositionOnCurveIteration(v0, v1, v2, v3, point, 150, ref tMin, ref tMax);

            var resultPosition = LiveMath.GetBezierPosition(v0, v1, v2, v3, tResult);
            if (Vector2.Distance(resultPosition, point) > dynamicCheckDistance)
                return (resultPosition, tResult);
            
            for (var i = 0; i < 15; i++)
                tResult = FindNearestPositionOnCurveIteration(v0, v1, v2, v3, point, 5, ref tMin, ref tMax);
            
            return (LiveMath.GetBezierPosition(v0, v1, v2, v3, tResult), tResult);
        }

        private static float FindNearestPositionOnCurveIteration(
            Vector2 v0, 
            Vector2 v1, 
            Vector2 v2, 
            Vector2 v3,
            Vector2 point,
            int accuracy,
            ref float tMin,
            ref float tMax)
        {
            var tDelta = (tMax - tMin) / accuracy;
            var minDistance = float.PositiveInfinity;
            var tNearest = 0f;

            for (var i = 0; i <= accuracy; i++)
            {
                var t = tMin + i * tDelta;
                var target = LiveMath.GetBezierPosition(v0, v1, v2, v3, t);
                var dist = LiveMath.VectorDistancePowTwo(point, target);
                
                if (dist > minDistance) 
                    continue;
                
                tNearest = t;    
                minDistance = dist;
            }

            tMin = Mathf.Clamp(tNearest - tDelta, 0, 1);
            tMax = Mathf.Clamp(tNearest + tDelta, 0, 1);
            
            return tNearest;
        }
    }
}
