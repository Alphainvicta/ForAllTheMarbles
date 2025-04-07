Shader "Custom/DistanceFade"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _FadeStart ("Fade Start Distance", Float) = 50
        _FadeEnd ("Fade End Distance", Float) = 10
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

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
                float distance : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _FadeStart;
            float _FadeEnd;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                // Calcular distancia entre el objeto y la cámara
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.distance = distance(worldPos, _WorldSpaceCameraPos);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Muestrear la textura
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                
                // Calcular transparencia basada en la distancia
                float alpha = 1 - smoothstep(_FadeEnd, _FadeStart, i.distance);
                col.a *= alpha;
                
                return col;
            }
            ENDCG
        }
    }
}