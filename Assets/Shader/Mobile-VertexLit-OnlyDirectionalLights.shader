Shader "Glow 11/Unity/Mobile/VertexLit (Only Directional Lights)" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
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