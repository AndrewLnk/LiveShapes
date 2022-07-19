using LiveShapes.Source.Scripts.ShapesEditor.Src.Other;
using UnityEngine;

namespace LiveShapes.Source.Scripts.ShapesEditor.Src.ExportTools.Algorithms
{
    public static class AlgorithmCreatePng
    {
        public static Texture2D CreateFromMesh(MeshFilter meshFilter, int textureSize, float outlineWidth, Color outlineColor)
        {
            if (meshFilter == null || meshFilter.sharedMesh == null || meshFilter.sharedMesh.vertices.Length < 3)
                return new Texture2D(1,1);

            var savedPosition = meshFilter.transform.position;
            var savedAngle = meshFilter.transform.eulerAngles;
            var savedLayer = meshFilter.gameObject.layer;
            var savedAntiAliasing = QualitySettings.antiAliasing;
            var savedFogActive = RenderSettings.fog;
            
            meshFilter.transform.position = new Vector3(10000,10000,10000);
            meshFilter.transform.eulerAngles = Vector3.zero;
            meshFilter.gameObject.layer = 31;
            QualitySettings.antiAliasing = 8;
            RenderSettings.fog = false;

            var outline = CreateMeshOutLine(meshFilter, outlineWidth, outlineColor);
            
            var renderTexture = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.ARGBFloat)
            { filterMode = FilterMode.Trilinear, antiAliasing = 4 };

            var vertices = GetRelevantVertices(meshFilter, outline);
            var camera = CreateRenderCameraAndPrepare(vertices);
            camera.targetTexture = renderTexture;
            camera.renderingPath = RenderingPath.VertexLit;
            camera.Render();

            var result = ConvertToTexture2D(renderTexture);
            camera.targetTexture = null;

            meshFilter.transform.position = savedPosition;
            meshFilter.transform.eulerAngles = savedAngle;
            meshFilter.gameObject.layer = savedLayer;
            QualitySettings.antiAliasing = savedAntiAliasing;
            RenderSettings.fog = savedFogActive;
            Object.DestroyImmediate(camera.gameObject);
            Object.DestroyImmediate(outline);
            
            return result;
        }

        private static Vector3[] GetRelevantVertices(MeshFilter meshFilter, LineRenderer outline)
        {
            if (outline == null)
                return meshFilter.sharedMesh.vertices;
            
            var camera = new GameObject().AddComponent<Camera>();
            var mesh = new Mesh();
            outline.BakeMesh(mesh, camera);
            Object.DestroyImmediate(camera.gameObject);
            
            var vertices = mesh.vertices.Length > 3 ? mesh.vertices : meshFilter.sharedMesh.vertices;
            return vertices;
        }

        private static Camera CreateRenderCameraAndPrepare(Vector3[] vertices)
        {
            var camera = new GameObject().AddComponent<Camera>();
            var camPosition = new Vector3(10000,10000,10000) - new Vector3(0,0, 10);
            camera.orthographic = true;
            camera.farClipPlane = 100;
            camera.nearClipPlane = 0f;
            camera.clearFlags = CameraClearFlags.Nothing;
            camera.cullingMask = (1 << 31);
            
            var (min, max) = LiveMath.GetBoundsOfPositions(vertices);
            var vector = max - min;
            vector = new Vector2(Mathf.Abs(vector.x), Mathf.Abs(vector.y));
            var s = Mathf.Max(vector.x, vector.y);
            camera.orthographicSize = s / 2;
            camPosition += (Vector3) (max - new Vector2(Mathf.Abs(min.x), Mathf.Abs(min.y))) / 2;

            camera.transform.position = camPosition;
            return camera;
        }
        
        private static Texture2D ConvertToTexture2D(RenderTexture renderTexture)
        {
            var tex = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false, false)
            {
                alphaIsTransparency = true, 
                filterMode = FilterMode.Trilinear
            };
            RenderTexture.active = renderTexture;
            tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            
            var pixels = tex.GetPixels();
            for (var i = 0; i < pixels.Length; i++)
            {
                if (pixels[i].a < 1f && pixels[i].a > 0f)
                {
                    pixels[i].r /= pixels[i].a;
                    pixels[i].g /= pixels[i].a;
                    pixels[i].b /= pixels[i].a;
                }
            }
            tex.SetPixels(pixels);
            
            tex.Apply();
            return tex;
        }

        private static LineRenderer CreateMeshOutLine(MeshFilter meshFilter, float width, Color color)
        {
            var line = meshFilter.gameObject.AddComponent<LineRenderer>();
            line.material = new Material(Shader.Find("LiveShapes/Color")) {color = color};
            line.useWorldSpace = false;
            
            if (meshFilter.sharedMesh == null || meshFilter.sharedMesh.vertices.Length < 3)
                return line;
                
            var positions = meshFilter.sharedMesh.vertices;
            for (var i = 0; i < positions.Length; i++)
                positions[i].z -= 0.1f;
            
            NormalizeStroke(positions, ref width);
            line.startWidth = width;
            line.endWidth = width;
            
            line.positionCount = positions.Length + 3;
            line.SetPositions(positions);
            line.SetPosition(positions.Length, positions[0]);
            line.SetPosition(positions.Length + 1, positions[1]);
            line.SetPosition(positions.Length + 2, positions[2]);

            return line;
        }
        
        private static void NormalizeStroke(Vector3[] vertices, ref float width)
        {
            var bounds = LiveMath.GetBoundsOfPositions(vertices);
            var size = bounds.max - bounds.min;
            size.x = Mathf.Abs(size.x);
            size.y = Mathf.Abs(size.y);
            var minSide = Mathf.Min(size.x, size.y);
            
            if (width > minSide)
                width = minSide;
        }
    }
}
