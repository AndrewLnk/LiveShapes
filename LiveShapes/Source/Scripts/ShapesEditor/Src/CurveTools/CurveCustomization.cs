using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools
{
    public static class CurveCustomization
    {
        public static Color GetCurveLineColor()
        {
            var color = Color.white;
            color.a = 0.5f;
            return color;
        }

        public static void SetDefaultPivotsColor(CurveData curveData, int iterationId, bool coloredWithoutIteration = false)
        {
            var checkByIteration = iterationId != -1 && 
                Array.Exists(curveData.ColoredPosIds, e => e.Equals(iterationId));
            var isColoredIteration = checkByIteration || coloredWithoutIteration;
            
            Handles.color = isColoredIteration ? GetColorByMode(curveData.Mode) : Color.white;
        }

        public static Color GetColorOfCurveByMode(CurveData curveData, int iterationId)
        {
            var isColoredIteration = iterationId != -1 && 
                Array.Exists(curveData.ColoredCurveIds, e => e.Equals(iterationId));

            return isColoredIteration ? GetColorByMode(curveData.Mode) : Color.white;
        }

        public static Color GetPivotColor()
        {
            var color = new Color32(101, 140, 255, 255);
            return color;
        }
        
        public static Color GetUvPivotColor()
        {
            var color = new Color32(171, 188, 255, 255);
            return color;
        }
        
        public static Color GetUvRectColor()
        {
            var color = new Color32(255, 255, 255, 15);
            return color;
        }


        public static Color GetColorByMode(Mode mode)
        {
            switch (mode)
            {
                case Mode.Move:
                    return new Color32(101, 140, 255, 255);
                case Mode.Add:
                    return new Color32(153, 255, 1, 255);
                case Mode.Remove:
                    return new Color32(250, 10, 51, 255);
                case Mode.Corners:
                    return new Color32(240, 188, 70, 255);
                default:
                    return Color.white;
            }
        }
    }
}
