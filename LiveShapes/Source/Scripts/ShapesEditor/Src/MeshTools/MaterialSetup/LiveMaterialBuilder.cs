using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.MeshTools.MaterialSetup
{
    public class LiveMaterialBuilder
    {
        private readonly MeshRenderer meshRenderer;
        private readonly Material liveMaterial;

        public readonly string[] ModeNames = {
            "Color",
            "Texture",
            "Texture Color",
            "Gradient",
            "Custom"
        };
        
        public readonly string[] GradientTypeNames = {
            "Linear",
            "Radial",
            "Angle",
            "Diamond"
        };
        
        public readonly string[] GradResolutionNames = {
            "32",
            "64",
            "128",
            "256",
            "512",
            "1024"
        };
        
        public int SetupPaintingId;
        private int savedMaterialSetupId;
        
        public Color Color = Color.green;
        private Color savedColor;
        
        public int GradientType;
        private int savedGradientType;
        public Gradient Gradient = new Gradient();
        private Gradient savedGradient;
        public int GradientResolution;
        private int savedGradientResolution;

        public Texture2D Texture2D;
        public Vector4 Texture2DPosition;
        private Texture2D savedTexture2D;
        private Vector4 savedTexture2DPosition;
        
        public Material Material;
        private Material savedMaterial;
        
        public LiveMaterialBuilder(MeshRenderer meshRenderer)
        {
            this.meshRenderer = meshRenderer;
            liveMaterial = new Material(Shader.Find("LiveShapes/Color")) {color = Color};
            meshRenderer.material = liveMaterial;
            Texture2DPosition = new Vector4(1,1,0,0);
        }

        public void Update()
        {
            UpdateMaterialProcess();
            ColorProcess();
            TextureProcess();
            GradientProcess();
            MaterialProcess();
        }

        private void UpdateMaterialProcess()
        {
            if (savedMaterialSetupId == SetupPaintingId && savedGradientType == GradientType)
                return;

            switch (SetupPaintingId)
            {
                case 0:
                    liveMaterial.shader = Shader.Find("LiveShapes/Color");
                    break;
                case 1:
                    liveMaterial.shader = Shader.Find("LiveShapes/Texture");
                    break;
                case 2:
                    liveMaterial.shader = Shader.Find("LiveShapes/Color Texture");
                    break;
                case 3 when GradientType == 0:
                    liveMaterial.shader = Shader.Find("LiveShapes/Gradient Linear");
                    break;
                case 3 when GradientType == 1:
                    liveMaterial.shader = Shader.Find("LiveShapes/Gradient Radial");
                    break;
                case 3 when GradientType == 2:
                    liveMaterial.shader = Shader.Find("LiveShapes/Gradient Angle");
                    break;
                case 3 when GradientType == 3:
                    liveMaterial.shader = Shader.Find("LiveShapes/Gradient Diamond");
                    break;
            }

            meshRenderer.sharedMaterial = liveMaterial;
            savedMaterial = liveMaterial;
            
            liveMaterial.mainTexture = null;
            liveMaterial.mainTextureScale = Vector2.one;
            liveMaterial.mainTextureOffset = Vector2.zero;
            savedTexture2DPosition = Vector4.positiveInfinity;
            savedTexture2D = null;
            savedGradient = null;

            savedMaterialSetupId = SetupPaintingId;
            savedGradientType = GradientType;
        }

        private void ColorProcess()
        {
            if (savedMaterialSetupId != 0 && savedMaterialSetupId != 2)
                return;

            if (savedColor.Equals(Color))
                return;

            savedColor = Color;
            liveMaterial.color = savedColor;
        }
        
        private void TextureProcess()
        {
            if (savedMaterialSetupId != 1 && savedMaterialSetupId != 2)
                return;
            
            if (savedTexture2D == Texture2D && savedTexture2DPosition == Texture2DPosition)
                return;

            savedTexture2DPosition = Texture2DPosition;
            savedTexture2D = Texture2D;
            liveMaterial.mainTexture = savedTexture2D;
            liveMaterial.mainTextureScale = new Vector2(savedTexture2DPosition.x, savedTexture2DPosition.y);
            liveMaterial.mainTextureOffset = new Vector2(savedTexture2DPosition.z, savedTexture2DPosition.w);
        }

        private void GradientProcess()
        {
            if (savedMaterialSetupId != 3)
                return;
            
            if (Gradient.GradientEquals(savedGradient) && 
                savedGradientType == GradientType && 
                savedGradientResolution == GradientResolution)
                return;

            savedGradient = Gradient.Clone();
            savedGradientType = GradientType;
            savedGradientResolution = GradientResolution;
            liveMaterial.mainTexture = AlgorithmGradientCreator.CreatTexture2D(savedGradient, GradientResolution);
            liveMaterial.mainTextureScale = Vector2.one;
            liveMaterial.mainTextureOffset = Vector2.zero;
        }

        private void MaterialProcess()
        {
            if (savedMaterialSetupId != 4)
                return;
            
            if (savedMaterial == Material)
                return;

            savedMaterial = Material;
            meshRenderer.sharedMaterial = Material;
        }
    }
}
