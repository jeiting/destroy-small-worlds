Shader "Custom/PlanetShader" {
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,1)
        [NoScaleOffset] _DamageMap ("Damage Map", 3D) = "white" {}
    }
    SubShader
    {

        Pass
        {
        	Tags {"LightMode"="ForwardBase"}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc" // for UnityObjectToWorldNormal
            #include "UnityLightingCommon.cginc" // for _LightColor0

            struct v2f
            {
                float2 uv : TEXCOORD0;
                fixed4 diff : COLOR0; // diffuse lighting color
                fixed4 damage : COLOR1; // diffuse lighting color
                float4 vertex : SV_POSITION;

            };


            sampler3D _DamageMap;

            v2f vert (appdata_base v)
            {
                v2f o;

                float4 damageUV = float4((v.vertex.zyx + 1.0) / 2.0, 1.0);

                o.damage = tex3Dlod(_DamageMap, damageUV);
                o.damage.a = o.damage.a * 2.0;

                o.vertex = UnityObjectToClipPos(v.vertex * (o.damage.a * 0.75 + 0.25));

                o.uv = v.texcoord;
                // get vertex normal in world space
                half3 worldNormal = UnityObjectToWorldNormal(v.normal);
                // dot product between normal and light direction for
                // standard diffuse (Lambert) lighting
                half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
                // factor in the light color
                o.diff = nl * _LightColor0;

                // Ambient lighting
                o.diff.rgb += ShadeSH9(half4(worldNormal,1)) * (o.damage.a * 0.75 + 0.25);

                return o;
            }

            // color from the material
            fixed4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                // sample texture
                fixed4 col = lerp(_Color, float4(i.damage.rgb, 1.0), (1.0 - i.damage.a));
                // multiply by lighting
                col *= i.diff;

                return col;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}
