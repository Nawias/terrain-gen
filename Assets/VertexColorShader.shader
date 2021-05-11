Shader "Custom/VertexColorShader"
{
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert
		#pragma vertex vert
		#pragma target 3.0
		//Struktura przechowujaca kolor wierzchołka
		struct Input {
			float4 vertColor;
		};
		//Vertex Shader przekazujący kolor wierzchołka do Surface Shadera
		void vert(inout appdata_full vertex, out Input surfInput) {
			UNITY_INITIALIZE_OUTPUT(Input, surfInput);
			surfInput.vertColor = vertex.color;
		}
		//Surface Shader kolorujący powierzchnię na podstawie koloru wierzchołka
		void surf(Input IN, inout SurfaceOutput output) {
			output.Albedo = IN.vertColor.rgb;
		}
		ENDCG
	}
    FallBack "Diffuse"
}
