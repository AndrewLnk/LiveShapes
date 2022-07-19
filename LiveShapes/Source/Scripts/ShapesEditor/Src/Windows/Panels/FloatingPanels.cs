using System.Collections.Generic;
using System.Linq;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEditor;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Windows.Panels
{
    public class FloatingPanels
    {
        private readonly List<IPanel> panels = new List<IPanel>();
        
        public readonly PanelMain PanelMain;
        public readonly PanelCreateTools CreateTools;
        public readonly PanelCurveTools CurveTools;
        public readonly PanelMeshTools MeshTools;
        public readonly PanelExportTools ExportTools;

        private bool pressed;
        private bool isFullScreen;
        private EditorWindow editorWindow;
        private Vector2 editorSavedSize = Vector2.positiveInfinity;
        private Vector2 EditorSize => new Vector2(editorWindow.position.width, editorWindow.position.height);
        
        public FloatingPanels()
        {
            var tempWindow = new FloatingPanel(false);
            tempWindow.SetupCheckWindowActiveFunc(()=> true);
            PanelMain = new PanelMain(tempWindow, this);
            panels.Add(PanelMain);

            tempWindow = new FloatingPanel(true);
            tempWindow.SetupCheckWindowActiveFunc(()=> LsStatics.ShowPanelCreate);
            CreateTools = new PanelCreateTools(tempWindow);
            panels.Add(CreateTools);

            tempWindow = new FloatingPanel(true);
            tempWindow.SetupCheckWindowActiveFunc(()=> LsStatics.ShowPanelEdit);
            CurveTools = new PanelCurveTools(tempWindow);
            panels.Add(CurveTools);

            tempWindow = new FloatingPanel(true);
            tempWindow.SetupCheckWindowActiveFunc(()=> LsStatics.ShowPanelMesh);
            MeshTools = new PanelMeshTools(tempWindow);
            panels.Add(MeshTools);

            tempWindow = new FloatingPanel(true);
            tempWindow.SetupCheckWindowActiveFunc(()=> LsStatics.ShowPanelExport);
            ExportTools = new PanelExportTools(tempWindow);
            panels.Add(ExportTools);

            foreach (var panel in panels)
                panel.FloatingPanel.SetupCorrectDeltaFunc(CheckDeltaOfPanelMove);
        }

        public void Update(CurveObject curveObject, SceneView sceneView)
        {
            GUI.color = Color.white;
            editorWindow = sceneView;
            
            foreach (var panel in panels)
                panel.UpdatePanel();
            
            CheckMouseDown();
            CheckCapForOneSelection();
            CheckChangedFullScreen();
            CheckWindowResize();
            CheckMainPanelOverlay();
            
            foreach (var panel in panels)
                panel.Update(curveObject);
        }

        public bool CheckAvailableMouse() => panels.All(panel => !panel.FloatingPanel.CheckMouseInPanel());

        private void CheckMouseDown()
        {
            if (MouseInput.Touched && !pressed && Event.current.button < 1)
            {
                pressed = true;

                foreach (var panel in panels)
                    panel.FloatingPanel.CheckPressedCap();
            }

            if (!MouseInput.Touched && pressed)
            {
                foreach (var panel in panels)
                    panel.FloatingPanel.PressedCap = false;

                pressed = false;
            }
        }

        public bool HasPressedCap()
        {
            foreach (var panel in panels)
            {
                if (panel.FloatingPanel.PressedCap)
                    return true;
            }
            
            return false;
        }

        private void CheckCapForOneSelection()
        {
            var state = false;
            for (var i = panels.Count - 1; i >= 0; i--)
            {
                if (!state && panels[i].FloatingPanel.PressedCap)
                {
                    state = true;
                    continue;
                }

                if (state && panels[i].FloatingPanel.PressedCap)
                    panels[i].FloatingPanel.PressedCap = false;
            }
        }

        private void CheckChangedFullScreen()
        {
            if (isFullScreen && !editorWindow.maximized)
                LsStatics.ResetPanelsPosition();

            if (!isFullScreen && editorWindow.maximized)
                editorSavedSize = EditorSize;
            
            isFullScreen = editorWindow.maximized;
        }

        private Vector2 CheckDeltaOfPanelMove(Vector2 delta, Vector2 position, Vector2 size)
        {
            if (position.y + size.y - delta.y > editorWindow.position.height - 24)
                delta.y = position.y + size.y - (editorWindow.position.height - 24);
            
            if (position.y - delta.y < 5)
                delta.y = position.y - 5;

            if (position.x + size.x - delta.x > editorWindow.position.width - 5)
                delta.x = position.x + size.x - (editorWindow.position.width - 5);
            
            if (position.x - delta.x < 5)
                delta.x = position.x - 5;

            return delta;
        }

        private void CheckWindowResize()
        {
            if (editorSavedSize.Equals(Vector2.positiveInfinity))
                editorSavedSize = EditorSize;
            
            if (EditorSize.VectorsEquals(editorSavedSize))
                return;

            if (!LsStatics.ShowPanelCreate && !LsStatics.ShowPanelEdit && !LsStatics.ShowPanelMesh && !LsStatics.ShowPanelExport)
            {
                LsStatics.ResetPanelsPosition();
                editorSavedSize = EditorSize;
                return;
            }
            
            var deltaChanged = editorSavedSize - EditorSize;
            editorSavedSize = EditorSize;
            MoveCurrentPanel(deltaChanged);
        }

        private void CheckMainPanelOverlay()
        {
            var sizeSize = PanelMain.FloatingPanel.WindowPosition + PanelMain.FloatingPanel.WindowSize +
                           Vector2.one * 5;
            var delta = LsStatics.FloatPanelPosition - sizeSize;

            if (delta.x > 0 || delta.y > 0)
                return;
            
            if (delta.x > delta.y)
                LsStatics.FloatPanelPosition = new Vector2(sizeSize.x, LsStatics.FloatPanelPosition.y);
            else
                LsStatics.FloatPanelPosition = new Vector2(LsStatics.FloatPanelPosition.x, sizeSize.y);
        }

        private void MoveCurrentPanel(Vector2 deltaChanged)
        {
            if (LsStatics.ShowPanelCreate)
            {
                CreateTools.FloatingPanel.ReplaceWindowByDelta(deltaChanged);
            }
            
            if (LsStatics.ShowPanelEdit)
            {
                CurveTools.FloatingPanel.ReplaceWindowByDelta(deltaChanged);
            }
            
            if (LsStatics.ShowPanelMesh)
            {
                MeshTools.FloatingPanel.ReplaceWindowByDelta(deltaChanged);
            }
            
            if (LsStatics.ShowPanelExport)
            {
                ExportTools.FloatingPanel.ReplaceWindowByDelta(deltaChanged);
            }
        }
    }
}
