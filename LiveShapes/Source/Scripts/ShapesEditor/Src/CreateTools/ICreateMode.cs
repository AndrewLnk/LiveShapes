using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CreateTools
{
    public interface ICreateMode
    {
        void SetupGetPositionFunc(Func<Vector2> getPositionFunc);
        void SetupCurveData(CurveObject package);
        void Start();
        void Update();
        void Finish();
    }
}
