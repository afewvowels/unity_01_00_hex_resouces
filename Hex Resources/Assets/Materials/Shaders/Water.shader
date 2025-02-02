﻿Shader "Custom/Water"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Specular ("Specular", Color) = (0.2, 0.2, 0.2)
        _BackgroundColor ("Background Color", Color) = (0, 0, 0)
        [NoScaleOffset] _HeightMap ("Heights", 2D) = "grey" {}
        //_BackgroundTex ("Emission Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf StandardSpecular alpha vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.5

        #pragma multi_compile _ HEX_MAP_EDIT_MODE

        #include "Water.cginc"
        #include "HexCellData.cginc"

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float2 visibility;
            float2 uv_HeightMap;
            //float4 screenPos;
        };
        sampler2D _HeightMap;
        float4 _HeightMap_TexelSize;

        half _Glossiness;
        fixed3 _Specular;
        fixed4 _Color;
        half3 _BackgroundColor;
        //sampler2D _BackgroundTex;
        //float4 _BackgroundTex_ST;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input data)
        {
            UNITY_INITIALIZE_OUTPUT(Input, data);

            float4 cell0 = GetCellData(v, 0);
            float4 cell1 = GetCellData(v, 1);
            float4 cell2 = GetCellData(v, 2);

            data.visibility.x = cell0.x * v.color.x + cell1.x * v.color.y + cell2.x * v.color.z;
            data.visibility.x = lerp(0.25, 1.0, data.visibility.x);
            data.visibility.y = cell0.y * v.color.x + cell1.y * v.color.y + cell2.y * v.color.z;
        }

        void surf (Input IN, inout SurfaceOutputStandardSpecular o)
        {
            float waves = Waves(IN.worldPos.xz, _MainTex);
            //float bgWaves = Waves(IN.worldPos.xz, _BackgroundTex);
            
            float2 heightUV = IN.worldPos.xz;
            heightUV.x *= 1 / (4 * 8.66025404);
            heightUV.y *= 1 / (2 * 15.0);
            heightUV *= 0.45;

            fixed4 c = saturate(_Color + waves);
            float explored = IN.visibility.y;

            //float2 bgTexCoordinates = IN.screenPos.xy / IN.screenPos.w;
            //float aspect = _ScreenParams.x / _ScreenParams.y;
            //bgTexCoordinates.x *= aspect;
            //bgTexCoordinates = TRANSFORM_TEX(bgTexCoordinates, _BackgroundTex);
            //fixed4 bgCol = saturate(_Color + bgWaves);
            //fixed4 bgCol = tex2D(_BackgroundTex, bgTexCoordinates);
            //o.Emission = bgCol * (c.a - explored);

            o.Albedo = c.rgb * (IN.visibility.x * explored);
            
            o.Specular = _Specular * explored;
            o.Smoothness = _Glossiness;
            o.Occlusion = explored;
            o.Normal = UnpackNormal (tex2D(_HeightMap, heightUV));
            o.Alpha = c.a * (IN.visibility.x * explored);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
