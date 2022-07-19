using System.Linq;
using LiveShapes.Source.Scripts.ShapesEditor.Src;
using LiveShapes.Source.Scripts.ShapesEditor.Src.CreateTools;
using LiveShapes.Source.Scripts.ShapesEditor.Src.CurveTools;
using LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MeshSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.UvSetup;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other.LiveShapesData;
using LiveShapes.Source.Scripts.ShapesEditor.Src.UndoSystem;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Windows;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Windows.Panels;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Editor
{
    public class EditorEntry : UnityEditor.Editor
    {
        private CurveObject curveObject;
        private CurveRenderer curveRenderer;
        private CurveEditor curveEditor;
        private UvEditor uvEditor;
        private ShapesCreator shapesCreator;
        private MeshUvKeeper meshUvKeeper;

        private LiveMeshBuilder liveMeshBuilder;
        private LiveUvBuilder liveUvBuilder;
        private LiveMaterialBuilder liveMaterialBuilder;
        private ExportToFileBuilder exportToFileBuilder;

        private FloatingPanels floatingPanels;

        private bool destroyPlanned;

        private LiveShapesUndo liveShapesUndo;
        private static LiveShapeData workedCopy;

        public void OnEnable()
        {
            curveObject = new CurveObject();
            curveRenderer = new CurveRenderer(curveObject);
            curveEditor = new CurveEditor(curveObject);
            shapesCreator = new ShapesCreator(curveObject);
            uvEditor = new UvEditor(curveObject.UvData);
            
            liveMeshBuilder = new LiveMeshBuilder(curveObject.MeshFilter);
            liveUvBuilder = new LiveUvBuilder(curveObject.MeshFilter);
            meshUvKeeper = new MeshUvKeeper(curveObject.MeshFilter);
            liveMaterialBuilder = new LiveMaterialBuilder(curveObject.MeshRenderer);
            exportToFileBuilder = new ExportToFileBuilder(curveObject, liveMeshBuilder, liveMaterialBuilder);
            liveShapesUndo = new LiveShapesUndo(curveObject, liveMeshBuilder, liveMaterialBuilder);
            
            floatingPanels = new FloatingPanels();
            floatingPanels.PanelMain.SetupClosePanelAction(() => destroyPlanned = true);
            floatingPanels.MeshTools.AddMeshBuilder(liveMeshBuilder);
            floatingPanels.MeshTools.AddMaterialBuilder(liveMaterialBuilder);
            floatingPanels.CreateTools.AddShapesCreator(shapesCreator);
            floatingPanels.CreateTools.AddUvEditor(uvEditor);
            floatingPanels.CreateTools.AddLiveMeshBuilder(liveMeshBuilder);
            floatingPanels.CreateTools.AddLiveMaterialBuilder(liveMaterialBuilder);
            floatingPanels.CreateTools.AddLiveFileBuilder(exportToFileBuilder);
            floatingPanels.CurveTools.AddUvKeeper(meshUvKeeper);
            floatingPanels.ExportTools.AddExportToFileBuilder(exportToFileBuilder);
            
            curveEditor.AddActionCheckAvailableMouse(floatingPanels.CheckAvailableMouse);
            shapesCreator.AddActionCheckAvailableMouse(floatingPanels.CheckAvailableMouse);
            shapesCreator.AddUvEditor(uvEditor);
            shapesCreator.AddLiveMeshBuilder(liveMeshBuilder);
            uvEditor.AddActionCheckAvailableMouse(floatingPanels.CheckAvailableMouse);
            liveMeshBuilder.AddMeshUpdatedAction(()=>liveUvBuilder.Update(curveObject.UvData, true));
            curveObject.Transform.AddCheckHandleMovableAction(floatingPanels.HasPressedCap);

            curveEditor.AddUndoSystem(liveShapesUndo);
            
            SceneView.duringSceneGui -= OnScene;
            SceneView.duringSceneGui += OnScene;

            AssemblyReloadEvents.beforeAssemblyReload -= Destroy;
            AssemblyReloadEvents.beforeAssemblyReload += Destroy;
            
            LsStatics.ResetPanelsPosition();
            LoadWorkedCopy();
        }

        private void OnScene(SceneView sceneView)
        {
            ScenePrepare.Update(sceneView);
            ScenePrepare.TryFocusOn(curveObject.Data.Positions);
            MouseInput.Update();
            
            liveShapesUndo.Update();
            liveShapesUndo.TryRestoreFromUndoObject();
            
            curveObject.Update();

            curveEditor.Update(sceneView);
            curveRenderer.Update(sceneView);
            shapesCreator.Update();
            uvEditor.Update(sceneView);
            
            liveMeshBuilder.Update(curveObject.Transform.GetLocalPositionsList(), !curveObject.MeshRenderer.enabled);
            liveUvBuilder.Update(curveObject.UvData);
            meshUvKeeper.Update();
            liveMaterialBuilder.Update();

            Handles.BeginGUI();
            floatingPanels.Update(curveObject, sceneView);
            Handles.EndGUI();
            
            TryDestroy(sceneView);
        }

        private void TryDestroy(SceneView sceneView)
        {
            if (destroyPlanned)
            {
                SaveWorkCopy();
                ScenePrepare.RestoreSceneState(sceneView);
                Destroy();
            }
        }

        private void Destroy() => DestroyImmediate(this);

        private void OnDestroy()
        {
            ScenePrepare.RestoreSavedSceneState();
            SceneView.duringSceneGui -= OnScene;
            AssemblyReloadEvents.beforeAssemblyReload -= Destroy;
  
            LsStatics.EditCornersMode = false;
            LsStatics.HandlesHide = false;

            curveObject.Transform.Destroy();
            UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
        }

        private void SaveWorkCopy()
        {
            workedCopy = LiveShapeSerialization.GetPreparedData(
                curveObject, 
                liveMeshBuilder, 
                liveMaterialBuilder, 
                exportToFileBuilder.FileName);
        }

        private void LoadWorkedCopy()
        {
            if (workedCopy == null)
                return;
            
            exportToFileBuilder.FileName = workedCopy.Name;
            curveObject.SetPositions(workedCopy.CurveLocalPositions.ToList());
            liveMeshBuilder.GenerateMeshSteps = workedCopy.GenerateMeshSteps;
            liveMeshBuilder.MeshOptimization = workedCopy.MeshOptimization;
            curveObject.UvData.LocalPosition = workedCopy.UvLocalPosition;
            curveObject.UvData.LocalAnchor = workedCopy.UvLocalAnchor;
            
            liveMaterialBuilder.SetupPaintingId = workedCopy.SetupPainting;
            liveMaterialBuilder.Color = workedCopy.PaintingColor;
            liveMaterialBuilder.GradientType = workedCopy.GradientType;
            liveMaterialBuilder.Gradient = workedCopy.Gradient;
            liveMaterialBuilder.GradientResolution = workedCopy.GradientResolution;
            var texture = EditorUtility.InstanceIDToObject(workedCopy.Texture2DId) as Texture2D;
            liveMaterialBuilder.Texture2D = texture != null ? texture : null;
            liveMaterialBuilder.Texture2DPosition = workedCopy.Texture2DPosition;
            var material = EditorUtility.InstanceIDToObject(workedCopy.MaterialId) as Material;
            liveMaterialBuilder.Material = material != null ? material : null;
        }
    }
}
