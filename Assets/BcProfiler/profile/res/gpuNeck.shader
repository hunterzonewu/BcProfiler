// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "DGM/gpuNeck" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}  

SubShader {
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	LOD 100
	Cull Off
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	
	Pass {  
			CGPROGRAM
			#pragma multi_compile_fwdbase
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			//#include "shaderCommon.cginc"
			struct v2f {
				float4 vertex : SV_POSITION;
				float4 vertexColor: COLOR0;
				half2 texcoord : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4    _MainTex_ST;
			
			v2f vert (appdata_full v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.vertexColor=v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.texcoord)*i.vertexColor;
				col.a = 0.02;
				return col;
			}
		ENDCG
	}
}

}
