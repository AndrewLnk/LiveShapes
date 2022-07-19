using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CreateTools
{
    public class TriangleMode : ICreateMode
    {
        private Func<Vector2> getPosition;
        private Vector2[] Positions => curveObject.Data.Positions;
        private CurveObject curveObject;

        private Vector2 basePoint;

        public void SetupGetPositionFunc(Func<Vector2> getPositionFunc) => getPosition = getPositionFunc;
        public void SetupCurveData(CurveObject package) => curveObject = package;

        public void Start()
        {
            LsStatics.HideCurve = false;
            curveObject.Transform.ResetPositions(10);
            basePoint = getPosition.Invoke();
        }

        public void Update()
        {
            SetupPointsByMouse();
            CalculateOtherPoints();
            AlgorithmCurveInversionCheck.FixInversion(curveObject.Data.Positions);
        }

        public void Finish()
        {
            curveObject.EditPivot = false;
            curveObject.Transform.SetupPivot((Positions[0] + Positions[3] + Positions[6]) / 3);
        }

        private void SetupPointsByMouse()
        {
            var pos = getPosition.Invoke();
            Positions[0] = new Vector2(basePoint.x + (pos.x - basePoint.x) / 2, pos.y);
            Positions[3] = new Vector2(basePoint.x + (pos.x - basePoint.x), basePoint.y);
            Positions[6] = basePoint;
            Positions[9] = Positions[0];
        }

        private void CalculateOtherPoints()
        {
            Positions[1] = Positions[0];
            Positions[2] = Positions[3];
            Positions[4] = Positions[3];
            Positions[5] = Positions[6];
            Positions[7] = Positions[6];
            Positions[8] = Positions[9];
        }
    }
}
