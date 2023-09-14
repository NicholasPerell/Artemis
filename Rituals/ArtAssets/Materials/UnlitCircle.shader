Shader "Custom/UnlitCircle" {
	Properties
    {
        _Color   ("Color",  Color) = (1, 1, 1, 1)
        _Cutoff  ("Cutoff", Float) = 0.5
    }

	Category {
	Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	Lighting Off

	SubShader {
		Pass {
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			float4 _Color;
    		float _Cutoff;

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = v.texcoord;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				clip(1-distance(i.texcoord, 0.5) - _Cutoff);
				float Transparency = i.color.a;
				float4 color = i.color;
				return i.color * _Color;
			}
			ENDCG 
		}
	}	
}
}
