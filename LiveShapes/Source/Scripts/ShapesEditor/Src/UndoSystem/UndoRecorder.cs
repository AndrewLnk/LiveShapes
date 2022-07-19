using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.UndoSystem
{
    public class LiveShapesUndo
    {
        private const string undoStateName = "Live Shape State";
        
        private readonly CurveObject curveObject;
        private readonly LiveMeshBuilder liveMeshBuilder;
        private readonly LiveMaterialBuilder liveMaterialBuilder;
        private readonly LiveShapeData data;
        private readonly UndoData undoData;
        
        private bool pressed;
        
        public LiveShapesUndo(CurveObject curveObject, LiveMeshBuilder liveMeshBuilder, LiveMaterialBuilder liveMaterialBuilder)
        {
            this.curveObject = curveObject;
            this.liveMeshBuilder = liveMeshBuilder;
            this.liveMaterialBuilder = liveMaterialBuilder;
            data = new LiveShapeData();
            undoData = ScriptableObject.CreateInstance<UndoData>();
        }

        public void Update()
        {
            if (MouseInput.Touched && !pressed && 
                MouseInput.ButtonPressed < 1 &&
                Tools.current != Tool.View)
            {
                pressed = true;
                AlgorithmRecordState.Record(data, curveObject, liveMeshBuilder, liveMaterialBuilder);
            }

            if (!MouseInput.Touched && pressed)
            {
                pressed = false;
                var changes = AlgorithmRecordState.GetChangeFromState(data, curveObject, liveMeshBuilder, liveMaterialBuilder);
                if (changes.code == -1) return;
                undoData.item = changes;
                RecordUndoData();
            }
        }

        private void RecordUndoData()
        {
            undoData.hasInfo = true;
            Undo.RegisterCompleteObjectUndo(undoData, undoStateName);
            undoData.hasInfo = false;
        }

        public void TryRestoreFromUndoObject()
        {
            if (!undoData.hasInfo)
                return;
            
            undoData.hasInfo = false;
            if (undoData.item.code < 1) return;
            AlgorithmRecordState.ApplyState(undoData.item, curveObject, liveMeshBuilder, liveMaterialBuilder);
        }
    }
}
