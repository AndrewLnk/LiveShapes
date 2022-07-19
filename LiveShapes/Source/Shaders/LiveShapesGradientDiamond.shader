Shader "LiveShapes/Gradient Diamond"
{
    Properties
    {
        _MainTex ("MainTexture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
        
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };
        
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
        
            fixed4 frag (v2f i) : SV_Target
            {
                i.uv -= float2(0.5, 0.5);
	            float t = abs(i.uv.x) / 0.5 + abs(i.uv.y) / 0.5;
                
                fixed4 c = tex2D (_MainTex, t);
                UNITY_APPLY_FOG(i.fogCoord, c);
                return c;
            }

            ENDCG
        }
    }
}
