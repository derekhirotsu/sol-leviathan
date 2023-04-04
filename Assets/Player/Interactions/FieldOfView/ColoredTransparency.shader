Shader "Custom/ColoredTransparency"
{
    Properties
    {
        _MainColor ( "MainColor", Color ) = (1, 1, 1, 1)
        _Outline ("Outline", float) = 1
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            uniform sampler2D _CameraDepthTexture; //Depth Texture
            sampler2D _MainTex, _NoiseTex;//
            float4 _MainTex_ST, _MainColor;
            float _Outline;//

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
                // sample the texture, apply color.
                fixed4 col = tex2D(_MainTex, i.uv) * _MainColor;
                // apply fog
                // UNITY_APPLY_FOG(i.fogCoord, col);
                return col ;
            }
            ENDCG
        }
    }
}
