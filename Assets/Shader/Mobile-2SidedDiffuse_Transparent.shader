Shader "Mobile/2-Sided Diffuse Transparent"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 200
        Cull Off // Makes it double-sided
        ZWrite Off // Avoids depth issues with transparent objects
        Blend SrcAlpha OneMinusSrcAlpha // Standard alpha blending

        CGPROGRAM
        #pragma surface surf Lambert alpha // Enable transparency

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a; // Use texture's alpha
        }
        ENDCG
    }
}
