Shader "Custom/DistanceFadeOverlay"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _FadeStart ("Fade Start (Invisible)", Float) = 50
        _FadeEnd ("Fade End (Visible)", Float) = 10
    }
    SubShader
    {
        Tags { 
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
            "IgnoreProjector"="True" 
        }
        LOD 100

        // Configuración para transparencia correcta
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float cameraDist : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _FadeStart;
            float _FadeEnd;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Calcular distancia exacta a la cámara
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.cameraDist = distance(worldPos, _WorldSpaceCameraPos);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Muestra la textura original
                fixed4 col = tex2D(_MainTex, i.uv);

                // Controla el alpha basado en la distancia
                float alpha = 1 - smoothstep(_FadeEnd, _FadeStart, i.cameraDist);
                col.a *= alpha; // Solo modifica el alpha, no el color RGB
                
                return col;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/Diffuse"
}