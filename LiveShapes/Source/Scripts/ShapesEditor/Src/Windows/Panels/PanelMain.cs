using System;
using LiveShapes.Source.Scripts.ShapesEditor.Images;
using LiveShapes.Source.Scripts.ShapesEditor.Images.CreatePanel;
using LiveShapes.Source.Scripts.ShapesEditor.Images.CurvePanel;
using LiveShapes.Source.Scripts.ShapesEditor.Images.ExportPanel;
using LiveShapes.Source.Scripts.ShapesEditor.Images.MainPanel;
using LiveShapes.Source.Scripts.ShapesEditor.Images.MeshPanel;
using LiveShapes.Source.Scripts.ShapesEditor.Src.MainComponents;
using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.Windows.Panels
{
    public class PanelMain : IPanel
    {
        public FloatingPanel FloatingPanel { get; }
        private Vector2 panelPosition = new Vector2(5,5);
        private const float buttonSize = 26f;
        private readonly FloatingPanels floatingPanels;

        public PanelMain(FloatingPanel floatingPanel, FloatingPanels floatingPanels)
        {
            this.floatingPanels = floatingPanels;
            FloatingPanel = floatingPanel;
            floatingPanel.SetupWindowSize(new Vector2(150, 57));
            floatingPanel.SetupTitle("Editor", SkinMainPanel.Instance.icon);
            floatingPanel.SetupPositionFunc((v) => panelPosition = v, () => panelPosition);
            floatingPanel.SetupCheckWindowActiveFunc(()=> true);
        }

        public void SetupClosePanelAction(Action action) => FloatingPanel.SetupClosePanelAction(action);

        public void UpdatePanel()
        {
            FloatingPanel.Update();
        }

        public void Update(CurveObject curveObject)
        {
            DrawHasWarning();
            
            var position = 25;
            for (var i = 0; i < 4; i++)
                DrawButton(i, position);
        }
        
        private void DrawButton(int id, float position)
        {
            var color = GetState(id) ? 
                Customization.PanelButtonSelectedColor : 
                Customization.PanelButtonDefaultColor;
            var outlineColor = Customization.PanelButtonOutlineColor;
            var rect = new Rect(FloatingPanel.WindowPosition, Vector2.one * buttonSize);
            rect.position += new Vector2(8f + rect.size.x * id + 9.5f * id, position);
            
            var buttonRect = new Rect(rect);
            buttonRect.size -= Vector2.one * 2.5f;
            buttonRect.position += Vector2.one * 1.5f;
            LGUI.PanelButton(buttonRect, GetAction(id), GetState(id));
            
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, color, 0f, 5f);
            GUI.DrawTexture(rect, Texture2D.whiteTexture, ScaleMode.ScaleAndCrop, false, 1, outlineColor, 1.5f, 5f);

            var iconRect = new Rect(rect);
            iconRect.size = Vector2.one * 12f;
            iconRect.position += (Vector2.one * buttonSize - iconRect.size) / 2;
            
            GUI.DrawTexture(iconRect, GetIcon(id));
        }
        
        private Texture2D GetIcon(int buttonId)
        {
            switch (buttonId)
            {
                case 0:
                    return SkinCreatePanel.Instance.icon;
                case 1:
                    return SkinCurvePanel.Instance.icon;
                case 2:
                    return SkinMeshPanel.Instance.icon;
                case 3:
                    return SkinExportPanel.Instance.icon;
                default:
                    return Texture2D.whiteTexture;
            }
        }

        private Action GetAction(int buttonId)
        {
            switch (buttonId)
            {
                case 0:
                    return () =>
                    {
                        LsStatics.ShowPanelCreate = !LsStatics.ShowPanelCreate;
                        LsStatics.ShowPanelEdit = false;
                        LsStatics.ShowPanelMesh = false;
                        LsStatics.ShowPanelExport = false;
                        floatingPanels.CreateTools.FloatingPanel.ReplaceWindowByDelta(Vector2.zero);
                    };
                case 1:
                    return () =>
                    {
                        LsStatics.ShowPanelEdit = !LsStatics.ShowPanelEdit;
                        LsStatics.ShowPanelCreate = false;
                        LsStatics.ShowPanelMesh = false;
                        LsStatics.ShowPanelExport = false;
                        floatingPanels.CurveTools.FloatingPanel.ReplaceWindowByDelta(Vector2.zero);
                    };
                case 2:
                    return () =>
                    {
                        LsStatics.ShowPanelMesh = !LsStatics.ShowPanelMesh;
                        LsStatics.ShowPanelCreate = false;
                        LsStatics.ShowPanelEdit = false;
                        LsStatics.ShowPanelExport = false;
                        floatingPanels.MeshTools.FloatingPanel.ReplaceWindowByDelta(Vector2.zero);
                    };
                case 3:
                    return () =>
                    {
                        LsStatics.ShowPanelExport = !LsStatics.ShowPanelExport;
                        LsStatics.ShowPanelCreate = false;
                        LsStatics.ShowPanelEdit = false;
                        LsStatics.ShowPanelMesh = false;
                        floatingPanels.ExportTools.FloatingPanel.ReplaceWindowByDelta(Vector2.zero);
                    };
                default:
                    return () => { };
            }
        }

        private bool GetState(int buttonId)
        {
            switch (buttonId)
            {
                case 0: return LsStatics.ShowPanelCreate;
                case 1: return LsStatics.ShowPanelEdit;
                case 2: return LsStatics.ShowPanelMesh;
                case 3: return LsStatics.ShowPanelExport;
                default:
                    return false; 
            }
        }

        private void DrawHasWarning()
        {
            if (!LiveLog.HasWarning)
                return;
            
            var rect = new Rect(FloatingPanel.WindowPosition + 
                                new Vector2(FloatingPanel.WindowSize.x - 35, 3.5f), new Vector2(12,12));
            GUI.Box(rect, SkinMainPanel.Instance.warning, GUIStyle.none);
        }
    }
}