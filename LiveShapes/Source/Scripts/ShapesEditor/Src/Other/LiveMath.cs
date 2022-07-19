using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Other
{
    public static class LiveMath
    {
        public static bool PointOnLine(this Vector2 p, Vector2 a, Vector2 b, float t = 1E-07f)
        {
            var zero = (b.x - a.x) * (p.y - a.y) - (p.x - a.x) * (b.y - a.y);
            if (zero > t || zero < -t) return false;

            return true;
        }

        public static bool PointOnLine(this Vector3 p, Vector3 a, Vector3 b, float t = 1E-07f)
        {
            var zero = (p.y - a.y) * (b.x - a.x) - (p.x - a.x) * (b.y - a.y);
            if (zero > t || zero < -t) return false;

            return true;
        }
        
        public static float CrossProduct(Vector2 v0, Vector2 v1) => v0.x * v1.y - v1.x * v0.y;
        
        public static float CrossProduct(Vector3 v0, Vector3 v1) => v0.x * v1.y - v1.x * v0.y;

        public static bool CheckPointInTriangle(Vector2 point, Vector2 v0, Vector2 v1, Vector2 v2)
        {
            var res0 = IsTriangleOrientedClockwise(v0, v1, point);
            var res1 = IsTriangleOrientedClockwise(v1, v2, point);
            var res2 = IsTriangleOrientedClockwise(v2, v0, point);

            return (res0 && res1 && res2) || 
                   (!res0 && !res1 && !res2);
        }

        public static bool CheckPointInTriangleClockWise(Vector3 point, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            if (!IsTriangleOrientedClockwise(v2, v0, point))
                return false;
            
            if (!IsTriangleOrientedClockwise(v0, v1, point))
                return false;
            
            if (!IsTriangleOrientedClockwise(v1, v2, point))
                return false;

            return true;
        }
        
        public static float VectorDistancePowTwo(Vector2 v0, Vector2 v1)
        {
            var xD = v1.x - v0.x;
            var yD = v1.y - v0.y;
            
            return xD * xD + yD * yD;
        }

        public static bool VectorsEquals(this Vector2 v0, Vector2 v1)
        {
            return (v0.x - v1.x < 0.0005f && v0.x - v1.x > -0.0005f) && 
                   (v0.y - v1.y < 0.0005f && v0.y - v1.y > -0.0005f);
        }
        
        public static bool VectorsEquals(this Vector3 v0, Vector3 v1)
        {
            return (v0.x - v1.x < 0.0005f && v0.x - v1.x > -0.0005f) && 
                   (v0.y - v1.y < 0.0005f && v0.y - v1.y > -0.0005f);
        }
        
        public static bool VectorsEquals(this Vector3 v0, Vector2 v1)
        {
            return (v0.x - v1.x < 0.0005f && v0.x - v1.x > -0.0005f) && 
                   (v0.y - v1.y < 0.0005f && v0.y - v1.y > -0.0005f);
        }
        
        public static bool VectorsEquals(this Vector2 v0, Vector3 v1)
        {
            return (v0.x - v1.x < 0.0005f && v0.x - v1.x > -0.0005f) && 
                   (v0.y - v1.y < 0.0005f && v0.y - v1.y > -0.0005f);
        }
        
        public static bool IsTriangleOrientedClockwise(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            var determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;
            return determinant < 0f;
        }
        
        public static bool IsTriangleOrientedClockwise(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            var determinant = p1.x * p2.y + p3.x * p1.y + p2.x * p3.y - p1.x * p3.y - p3.x * p2.y - p2.x * p1.y;
            return determinant < 0f;
        }
        
        public static Vector2 GetBezierPosition (
            Vector2 v0, 
            Vector2 v1, 
            Vector2 v2, 
            Vector2 v3, 
            float t) {
            var dec = 1f - t;
            return dec * dec * dec * v0 + 3f * dec * dec * t * v1 + 3f * dec * t * t * v2 + t * t * t * v3;
        }

        public static (Vector2 min, Vector2 max) GetBoundsOfPositions(IEnumerable<Vector3> positions)
        {
            var min = Vector2.one * float.PositiveInfinity;
            var max = Vector2.one * float.NegativeInfinity;
            foreach (var v in positions)
            {
                if (v.x < min.x) min.x = v.x;
                if (v.y < min.y) min.y = v.y;
                if (v.x > max.x) max.x = v.x;
                if (v.y > max.y) max.y = v.y;
            }

            return (min, max);
        }
        
        public static (Vector2 min, Vector2 max) GetBoundsOfPositions(IEnumerable<Vector2> positions)
        {
            var min = Vector2.one * float.PositiveInfinity;
            var max = Vector2.one * float.NegativeInfinity;
            foreach (var v in positions)
            {
                if (v.x < min.x) min.x = v.x;
                if (v.y < min.y) min.y = v.y;
                if (v.x > max.x) max.x = v.x;
                if (v.y > max.y) max.y = v.y;
            }

            return (min, max);
        }
        
        public static float AngleOfTriangle(Vector2 v0, Vector2 v1, Vector2 v2)
        {
            var d1 = v0 - v1;
            var d2 = v2 - v1;
            d2.Normalize();
            d1.Normalize();
            var angle = Mathf.Acos(d1.x * d2.x + d1.y * d2.y);
            angle *= Mathf.Rad2Deg;

            if (float.IsNaN(angle))
                return 180f;

            if (Math.Abs(angle) < 0.0001f)
                return angle;

            if (!IsTriangleOrientedClockwise(v0, v1, v2))
                return 360f - angle;

            return angle;
        }

        public static bool GradientEquals(this Gradient gradient, Gradient target)
        {
            if (target == null)
                return false;
            
            if (gradient.alphaKeys.Length != target.alphaKeys.Length)
                return false;

            if (gradient.colorKeys.Length != target.colorKeys.Length)
                return false;
            
            for (var i = 0; i < gradient.alphaKeys.Length; i++)
                if (!gradient.alphaKeys[i].Equals(target.alphaKeys[i]))
                    return false;
            
            for (var i = 0; i < gradient.colorKeys.Length; i++)
                if (!gradient.colorKeys[i].Equals(target.colorKeys[i]))
                    return false;

            return true;
        }

        public static Gradient Clone(this Gradient gradient)
        {
            if (gradient == null)
                return null;
            
            return new Gradient
            {
                mode = gradient.mode,
                alphaKeys = (GradientAlphaKey[]) gradient.alphaKeys.Clone(),
                colorKeys = (GradientColorKey[]) gradient.colorKeys.Clone()
            };
        }
        
        public static bool ArraysEquals(Vector2[] array1, Vector2[] array2)
        {
            if (array1 == null && array2 != null)
                return false;
            
            if (array1 != null && array2 == null)
                return false;

            if (array1 == null)
                return true;

            if (array1.Length != array2.Length)
                return false;

            for (var i = 0; i < array1.Length; i++)
            {
                if (!array1[i].VectorsEquals(array2[i]))
                    return false;
            }

            return true;
        }
    }
}