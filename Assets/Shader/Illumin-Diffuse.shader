Shader "Glow 11/Unity/Self-Illumin/Diffuse" {
Properties {
 _Color ("Main Color", Color) = (1,1,1,1)
 _MainTex ("Base (RGB) Gloss (A)", 2D) = "white" { }
 _Illum ("Illumin (A)", 2D) = "white" { }
 _EmissionLM ("Emission (Lightmapper)", Float) = 0
 _GlowTex ("Glow", 2D) = "" { }
 _GlowColor ("Glow Color", Color) = (1,1,1,1)
 _GlowStrength ("Glow Strength", Float) = 1
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