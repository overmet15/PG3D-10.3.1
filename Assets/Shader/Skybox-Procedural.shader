Shader "Skybox/Procedural" {
Properties {
 _SunTint ("Sun Tint", Color) = (1,1,1,1)
 _SunStrength ("Sun Strength", Float) = 1
 _Color ("Atmosphere Tint", Color) = (0.5,0.5,0.5,1)
 _GroundColor ("Ground", Color) = (0.369,0.349,0.341,1)
 _HdrExposure ("HDR Exposure", Float) = 1.3
}
	//DummyShaderTextExporter
	
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard fullforwardshadows
#pragma target 3.0
		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
		}
		ENDCG
	}
}