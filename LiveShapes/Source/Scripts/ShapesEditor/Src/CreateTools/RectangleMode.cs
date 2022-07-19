using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools.CurveAlgorithms;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CreateTools
{
    public class RectangleMode : ICreateMode
    {
        private Func<Vector2> getPosition;
        private CurveObject curveObject;
        
        private Vector2[] Positions => curveObject.Data.Positions;

        public void SetupGetPositionFunc(Func<Vector2> getPositionFunc) => getPosition = getPositionFunc;
        
        public void SetupCurveData(CurveObject package) => curveObject = package;

        public void Start()
        {
            LsStatics.HideCurve = false;
            curveObject.Transform.ResetPositions(13);
            var pos = getPosition.Invoke();
            Positions[0] = pos;
            Positions[1] = pos;
            Positions[11] = pos;
            Positions[12] = pos;
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
            curveObject.Transform.SetupPivot(Vector2.Lerp(Positions[0], Positions[6], 0.5f));
        }

        private void SetupPointsByMouse()
        {
            var pos = getPosition.Invoke();
            Positions[5] = pos;
            Positions[6] = pos;
            Positions[7] = pos;
        }

        private void CalculateOtherPoints()
        {
            var pos1 = new Vector2(getPosition.Invoke().x, Positions[0].y);
            Positions[2] = pos1;
            Positions[3] = pos1;
            Positions[4] = pos1;
            
            var pos2 = new Vector2(Positions[0].x, getPosition.Invoke().y);
            Positions[8] = pos2;
            Positions[9] = pos2;
            Positions[10] = pos2;
        }
    }
}
