using System;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEditor;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.CreateTools
{
    public class ShapesCreator
    {
        private readonly CurveObject curveObject;
        private UvEditor uvEditor;
        private LiveMeshBuilder liveMeshBuilder;
        private Func<bool> mouseAvailable;
        private ICreateMode currentMode;
        private bool pressed;
        public int DrawCurveShapeId = -1;

        public ShapesCreator(CurveObject curveObject) => this.curveObject = curveObject;
        public void AddActionCheckAvailableMouse(Func<bool> func) => mouseAvailable = func;
        public void AddUvEditor(UvEditor editor) => uvEditor = editor;

        public void AddLiveMeshBuilder(LiveMeshBuilder builder) => liveMeshBuilder = builder;
        
        public void Update()
        {
            CheckMouseDown();
            currentMode?.Update();
        }
        
        private void CheckMouseDown()
        {
            if (MouseInput.Touched && !pressed && currentMode == null && 
                MouseInput.ButtonPressed < 1 && Tools.current != Tool.View)
            {
                if (!mouseAvailable.Invoke())
                    return;
                
                pressed = true;
                
                CreateMode();
                
                if (currentMode == null)
                    return;
                
                currentMode.SetupGetPositionFunc(() => MouseInput.WorldPosition);
                currentMode.SetupCurveData(curveObject);
                currentMode.Start();
                curveObject.ResetUv();
            }

            if (!MouseInput.Touched && pressed)
            {
                pressed = false;
                
                if (currentMode == null)
                    return;
                
                currentMode.Finish();
                liveMeshBuilder.Update(curveObject.Transform.GetLocalPositionsList());
                curveObject.ResetUv();
                LsStatics.HideUvEditor = true;
                curveObject.Transform.HidePivot = false;
                DrawCurveShapeId = -1;
                currentMode = null;
            }
        }
        
        private void CreateMode()
        {
            switch (DrawCurveShapeId)
            {
                case 0: currentMode = new RectangleMode(); break;
                case 1: currentMode = new CircleMode(); break;
                case 2: currentMode = new TriangleMode(); break;
            }
        }
    }
}
