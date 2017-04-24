Shader "UI/FuelBar"
 {
     Properties
     {
         [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
         _Color ("Tint", Color) = (1,1,1,1)
         
         _StencilComp ("Stencil Comparison", Float) = 8
         _Stencil ("Stencil ID", Float) = 0
         _StencilOp ("Stencil Operation", Float) = 0
         _StencilWriteMask ("Stencil Write Mask", Float) = 255
         _StencilReadMask ("Stencil Read Mask", Float) = 255
 
         _ColorMask ("Color Mask", Float) = 15
     }
 
     SubShader
     {
         Tags
         { 
             "Queue"="Overlay" 
             "IgnoreProjector"="True" 
             "RenderType"="Transparent" 
             "PreviewType"="Plane"
             "CanUseSpriteAtlas"="True"
         }
         
         Stencil
         {
             Ref [_Stencil]
             Comp [_StencilComp]
             Pass [_StencilOp] 
             ReadMask [_StencilReadMask]
             WriteMask [_StencilWriteMask]
         }
 
         Cull Off
         Lighting Off
         ZWrite Off
         ZTest Off
         Blend SrcAlpha OneMinusSrcAlpha
         ColorMask [_ColorMask]
 
         Pass
         {
         CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #include "UnityCG.cginc"
             
             struct appdata_t
             {
                 float4 vertex   : POSITION;
                 float4 color    : COLOR;
                 float2 texcoord : TEXCOORD0;
             };
 
             struct v2f
             {
                 float4 vertex   : SV_POSITION;
                 fixed4 color    : COLOR;
                 half2 texcoord  : TEXCOORD0;
             };
             
             fixed4 _Color;
 
             v2f vert(appdata_t IN)
             {
                 v2f OUT;
                 OUT.vertex = UnityObjectToClipPos(IN.vertex);
                 OUT.texcoord = IN.texcoord;
 #ifdef UNITY_HALF_TEXEL_OFFSET
                 OUT.vertex.xy += (_ScreenParams.zw-1.0)*float2(-1,1);
 #endif
				 float brightness = (sin(4.0 * _Time.a) + 1.0) / 2.0;
                 OUT.color = IN.color * _Color;

                 return OUT;
             }
 
             sampler2D _MainTex;
 
             fixed4 frag(v2f IN) : SV_Target
             {
                 half4 color = IN.color;

                 float pi = 3.1415;
                 float offset = (sin(1 * pi * IN.texcoord.y + _Time.r) + 
                 				 sin(3 * pi * IN.texcoord.y - 3.0 *_Time.a) + 
                 				 sin(5 * pi * IN.texcoord.y + 1.0 *_Time.a)) / 3.0;

                 if (IN.texcoord.x > (0.99 + .01 * offset)) {
                 	color.a = 0.0;
                 }

                 clip (color.a - 0.01);
                 return color;
             }
         ENDCG
         }
     }
 }