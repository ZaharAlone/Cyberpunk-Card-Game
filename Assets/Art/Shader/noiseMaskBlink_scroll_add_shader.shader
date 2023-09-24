Shader "SHADERS/special/noiseMaskBlink_scroll_add" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _NoiseTex ("NoiseTex", 2D) = "white" {}
        _blinkTex ("blinkTex", 2D) = "white" {}
		_EmissivePower ("Emissive Power", Range(1, 10)) = 1
    }
    SubShader 
	{
        Tags 
		{
            "Queue"="Transparent+1"
            "PreviewType"="Plane"
        }
        Pass 
		{

            Blend One One
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            #pragma target 3.0
			
            uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
            uniform sampler2D _NoiseTex;
			uniform float4 _NoiseTex_ST;
            uniform sampler2D _blinkTex;
			uniform float4 _blinkTex_ST;
			uniform float _EmissivePower;
			
            struct appdata 
			{
                float4 vertex : POSITION;
                float4 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct v2f 
			{
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            v2f vert (appdata v) 
			{
                v2f o = (v2f)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(v2f i) : COLOR 
			{

                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                float2 UVRemaped1 = (i.uv0*1.0+-1.0).rg;
                float2 NoiseScroll = float2(UVRemaped1.r,(UVRemaped1.g+(_Time.g*i.uv0.a)));
                float4 _NoiseTex_var = tex2D(_NoiseTex,TRANSFORM_TEX(NoiseScroll, _NoiseTex));
                float UVRemaped2 = (i.uv0.b*2.0+-1.0);
                float2 BlinkScroll = float2(UVRemaped1.r,(UVRemaped1.g+UVRemaped2));
                float4 _blinkTex_var = tex2D(_blinkTex,TRANSFORM_TEX(BlinkScroll, _blinkTex));
				float mask = (1 - saturate((UVRemaped2 + i.uv0.g)* 3 - 2))*(1 - saturate(((i.uv0.g+UVRemaped2) * -3 + 1)));
                float grayscale = ((_MainTex_var.r * _NoiseTex_var.r)+(mask*(_MainTex_var.r * _blinkTex_var.r)));
                float3 emissive = i.vertexColor.a * _EmissivePower * ((i.vertexColor.rgb * grayscale)+saturate((grayscale * 4 - 2)));
                return fixed4(emissive,1);
            }
            ENDCG
        }
    }

}
