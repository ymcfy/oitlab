Shader "Custom/OITAccum" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//_Weight ("Weight Exponent", Range(-5.0, 0.0)) = 0.0
	}
	SubShader {
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "IgnoreProjector"="True" }
		LOD 200
		Cull Off Lighting Off ZWrite Off ZTest LEqual Fog { Mode Off }
		Blend One One

		Pass {
			CGPROGRAM
			#pragma multi_compile _WEIGHTED0 _WEIGHTED1 _WEIGHTED2 _WEIGHTED3
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile PREMULTIPLIED_ALPHA_ON PREMULTIPLIED_ALPHA_OFF
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float _Weight;

			struct appdata {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;
			};
			
			struct vs2ps {
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 uv : TEXCOORD0;			
				float z : TEXCOORD1;
			};
			
			vs2ps vert(appdata i) {
				float4 v = mul(UNITY_MATRIX_MV, i.vertex);
			
				vs2ps o;
				o.vertex = mul(UNITY_MATRIX_P, v);
				//o.color = float4(i.color.rgb * i.color.a, i.color.a);
				o.color = i.color * i.color.a;
				o.uv = i.uv;
				o.z = abs(v.z);
				return o;
			}
			
			//float w(float z) {
			//	return pow(z, _Weight);
			//}
			//float w(float z, float alpha) {
			//	if (alpha > 0.8)
			//		return pow(z, -2.5);
			//	else
			//		return 1.0;
			//}
			float w(float z, float alpha) {
				#ifdef _WEIGHTED0
					return pow(z, -2.5);
				#elif _WEIGHTED1
					return alpha * max(1e-2, min(3 * 1e3, 10.0/(1e-5 + pow(z/5, 2) + pow(z/200, 6))));
				#elif _WEIGHTED2
					return alpha * max(1e-2, min(3 * 1e3, 0.03/(1e-5 + pow(z/200, 4))));
				#elif _WEIGHTED3
				return 1.0;
				#endif
				return 1.0;
			}
			
			float4 frag(vs2ps i) : COLOR {
				 float4 c = tex2D(_MainTex, i.uv);
				 #ifdef PREMULTIPLIED_ALPHA_OFF
				 c.rgb *= c.a;
				 #endif
				 //return c  * w(i.z,c.a);
				 //return c * i.color * w(abs(i.z), c.a);
				// return c ;
				 return c  * w(abs(i.z), c.a);
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
