Shader "Sprites/Bumped Diffuse"
{
    Properties
    {
        [PerRendererData] _MainTex ("Albedo", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1)
        _BumpMap ("Normal", 2D) = "bump" {}

        [MaterialToggle] PixelSnap ("Pixel Snap", Float) = 0
        _Cutoff ("Alpha Cutoff", Range (0,1)) = 0.5
    }
 
    SubShader
    {
        Tags
        {
            "Queue"="AlphaTest"
            "IgnoreProjector"="True"
            "RenderType"="TransparentCutOut"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
            LOD 300
 
 
        Cull Off
        Lighting On
        ZWrite On
        Fog { Mode Off }
     
 
        CGPROGRAM
        #pragma surface surf Lambert alpha vertex:vert  alphatest:_Cutoff fullforwardshadows
        #pragma multi_compile PIXELSNAP_ON
 
        sampler2D _MainTex;
        sampler2D _BumpMap;
        fixed4 _Color;
 
        struct Input
        {
            float2 uv_MainTex;
            fixed4 color;
        };
     
        void vert (inout appdata_full v, out Input o)
        {
            #if defined(PIXELSNAP_ON) && !defined(SHADER_API_FLASH)
            v.vertex = UnityPixelSnap (v.vertex);
            #endif
            v.normal = float3(0,0,-1);
            v.tangent =  float4(1, 0, 0, 1);
         
            UNITY_INITIALIZE_OUTPUT(Input, o);
            o.color = _Color;
        }
 
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * IN.color;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
        }
        ENDCG
    }
 
Fallback "Transparent/Cutout/Diffuse"
}