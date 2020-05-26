Shader "Unlit/Overlay"
{
    Properties
    {
        _UnlitColor ("Color", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Overlay" }
        ZTest Always

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
            };

            float4 _UnlitColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _UnlitColor;
                return col;
            }
            ENDCG
        }
    }
}
