using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms
{
    public static class AlgorithmIsPointInQuad
    {
        public static bool Check( 
            Vector2 v0, 
            Vector2 v1, 
            Vector2 v2, 
            Vector2 v3,
            Vector2 point,
            float checkDistance)
        {
            var bounds = GetBoundsOfCurve(v0, v1, v2, v3);
            MovePointsIfOverlay(ref bounds.min, ref bounds.max, checkDistance);
            
            // Handles.color = Color.green;
            // Handles.DrawLine(bounds.min, new Vector3(bounds.min.x, bounds.max.y));
            // Handles.DrawLine(new Vector3(bounds.min.x, bounds.max.y), bounds.max);
            // Handles.DrawLine(bounds.max, new Vector3(bounds.max.x, bounds.min.y));
            // Handles.DrawLine(new Vector3(bounds.max.x, bounds.min.y), bounds.min);
            
            return CheckPoint(bounds.min, bounds.max, point);
        }

        private static void ReorderPoints(ref Vector2 v0, ref Vector2 v1, 
                                         ref Vector2 v2, ref Vector2 v3)
        {
            var array = new[] {v0, v1, v2, v3};
            Array.Sort(array, (vector2, vector3) => vector2.x < vector3.x ? -1 : 1);
            v0 = array[0].y < array[1].y ? array[0] : array[1];
            v1 = array[0].y < array[1].y ? array[1] : array[0];
            v2 = array[2].y > array[3].y ? array[2] : array[3];
            v3 = array[2].y > array[3].y ? array[3] : array[2];
        }

        private static (Vector2 min, Vector2 max) GetBoundsOfCurve(params Vector2[] v)
        {
            var min = Vector2.positiveInfinity;
            var max = Vector2.negativeInfinity;

            foreach (var vector2 in v)
            {
                if (vector2.x < min.x)
                    min.x = vector2.x;

                if (vector2.x > max.x)
                    max.x = vector2.x;

                if (vector2.y < min.y)
                    min.y = vector2.y;

                if (vector2.y > max.y)
                    max.y = vector2.y;
            }

            return (min, max);
        }

        private static void MovePointsIfOverlay(
            ref Vector2 v0, ref Vector2 v1, 
            ref Vector2 v2, ref Vector2 v3, float distance)
        {
            var m = v0;
            var m2 = v3;
            
            if (v1.PointOnLine(v0, v3, 0.01f))
            {
                v0 = FindPositionRightAngle(m2, m, distance);
                v1 = FindPositionRightAngle(m2, m, -distance);
            }

            if (v2.PointOnLine(v0, v3, 0.01f))
            {
                v2 = FindPositionRightAngle(m, m2, distance);
                v3 = FindPositionRightAngle(m, m2, -distance);
            }
        }

        private static Vector2 FindPositionRightAngle(Vector2 v0, Vector2 v1, float distance)
        {
            var t = v1 - v0;
            t.Normalize();
            var r = new Vector2(t.y, -t.x);
            r *= distance;
            return v1 + r;
        }
        
        private static bool CheckPoint(Vector2 v0, Vector2 v1, Vector2 point) => 
            point.x >= v0.x && point.x <= v1.x && point.y >= v0.y && point.y <= v1.y;

        private static void MovePointsIfOverlay(ref Vector2 v0, ref Vector2 v1, float distance)
        {
            if (Mathf.Abs(v1.x - v0.x) < distance)
            {
                v0.x -= distance / 2;
                v1.x += distance / 2;
            }

            if (Mathf.Abs(v1.y - v0.y) < distance)
            {
                v0.y -= distance / 2;
                v1.y += distance / 2;
            }
        }

        private static bool CheckPoint(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, Vector2 point)
        {
            return LiveMath.CheckPointInTriangle(point, v0, v1, v3) ||
                   LiveMath.CheckPointInTriangle(point, v3, v1, v2) || 
                   LiveMath.CheckPointInTriangle(point, v0, v1, v2) || 
                   LiveMath.CheckPointInTriangle(point, v3, v0, v2);
        }
    }
}
