
Shader "Vander/Numbers"
	{

	Properties{
	//Properties
		_numberCount("Counter", Float) = 1.0
		_xAnimate("xAnimate", Float) = 0.0
		_yAnimate("yAnimate", Float) = 0.0
	}

	SubShader
	{
	Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

	Pass
	{
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#include "UnityCG.cginc"

	struct VertexInput {
    fixed4 vertex : POSITION;
	fixed2 uv:TEXCOORD0;
    fixed4 tangent : TANGENT;
    fixed3 normal : NORMAL;
	UNITY_VERTEX_INPUT_INSTANCE_ID // Added for SPI
	//VertexInput
	};


	struct VertexOutput {
	fixed4 pos : SV_POSITION;
	fixed2 uv:TEXCOORD0;
	UNITY_VERTEX_OUTPUT_STEREO // Added for SPI
	//VertexOutput
	};

	//Variables
	float _numberCount;
	float _xAnimate;
	float _yAnimate;
	UNITY_INSTANCING_BUFFER_START(Props)
    // put more per-instance properties here
    UNITY_INSTANCING_BUFFER_END(Props)

	fixed ddig(fixed2 uv, int dig){
	
	bool u=true,
		 c=false,
		 l=false,
		 t=true;
	
	if(uv.y<min(0.,abs(uv.x)-0.5)) u=!u;
	uv.y=abs(uv.y)-0.5;
	if(abs(uv.x)<abs(uv.y)){ uv=uv.yx; c=!c; }
	uv.y=abs(uv.y)-0.4;
	if(uv.x<0.) l=!l;
	uv.x=abs(abs(uv.x)-0.5);
	
	dig-=(dig/10)*10;
	
	
	/* vvv   Don't even try   vvv */
	if((dig==0) && (c&&l)) return 1.;
	else if((dig==1) && (c||l)) return 1.;
	else if((dig==2) && ((u&&l||!(u||l))&&!c)) return 1.;
	else if((dig==3) && (l&&!c)) return 1.;
	else if((dig==4) && (c&&!l||l&&!u)) return 1.;
	else if((dig==5) && (!c &&(!l&&u || l&&!u))) return 1.;
	else if((dig==6) && (u&&!c&&!l)) return 1.;
	else if((dig==7) && (l||c&&!u)) return 1.;
	else if((dig==9) && (!u&&l)) return 1.;
	
	//if(!t)
	return uv.x+max(0.,uv.y);
	return 1.;
}

fixed3 shade(fixed2 uv){
	
	fixed v=1.;
	[unroll(100)]
	for(fixed i=0. ; i<7. ; i++){
		if(i>0.1){
			fixed d=fmod(_numberCount/pow(10.,i-0.),1.)*10.;
			v=min(v,ddig(uv*1.5+fixed2(i*1.3-1.95,0.),int(d)));
		}
	}
	
	v=smoothstep(0.07,0.05,v);
	return fixed3(v,v,v);
}


	VertexOutput vert (VertexInput v)
	{
		VertexOutput o;

		UNITY_SETUP_INSTANCE_ID(v); // Added for SPI
        UNITY_INITIALIZE_OUTPUT(VertexOutput, o); // Added for SPI
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); // Added for SPI

		o.pos = UnityObjectToClipPos (v.vertex);
		o.uv = v.uv;
		//VertexFactory
		return o;
	}


	fixed4 frag(VertexOutput i) : SV_Target
	{
	
		fixed2 uv = (2.*i.uv-1)/0.5;
		uv.x = uv.x-0.87;
		//for animation on x or y
		uv.x += _xAnimate;
		uv.y += _yAnimate;

		//uv = uv*fixed2(0.8,0.7)+fixed2(-0.1,0.1);
		return fixed4(shade(uv*2.),1.0);

	}
	ENDCG
	}
  }
}

