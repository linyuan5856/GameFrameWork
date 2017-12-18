Shader "TestToonRender/ThreeLightShader"
{
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}

		_FirstLight ("First Light Dir", Vector) = (1, 1, 1, 0)
		_FirstLightColour ("First Light  Color", Color) = (0.561, 0.341, 0.016, 1)
		[Toggle]_CanSpecular1("First Light Specular On?",Float)=1
		 _Specular1Colour ("First Light Specular", Color) = (1, 1, 1, 1)
         _Gloss1 ("First Light Gloss", Range(8.0, 256)) = 20

		_SecLight ("Second Light Dir", Vector) = (1, 1, 1, 0)
		_SecLightColour ("Second Light  Color", Color) = (0.561, 0.341, 0.016, 1)
		 [Toggle]_CanSpecular2("Second Light Specular On?",Float)=1
		 _Specular2Colour ("Second Light Specular", Color) = (1, 1, 1, 1)
         _Gloss2 ("Second Light Gloss", Range(8.0, 256)) = 20
		
		_ThirdLight ("Third Light Dir", Vector) = (1, 1, 1, 0)
		_ThirdLightColour ("Third Light  Color", Color) = (0.561, 0.341, 0.016, 1)
	     [Toggle]_CanSpecular3("Third Light Specular On?",Float)=1
		 _Specular3Colour ("Third Light Specular", Color) = (1, 1, 1, 1)
         _Gloss3 ("Third Light Gloss", Range(8.0, 256)) = 20		
		
		[Toggle]_ClipAlpha("Clip Texture Alpha",Float)=0
	}
	
    Category
    {
	Cull Off
	Lighting Off

	SubShader {

		Pass {
			Cull Off
			LOD 200
			Tags { "RenderType"="Opaque" }

			CGPROGRAM

			#pragma vertex vert 
	        #pragma fragment frag
			#pragma shader_feature _CLIPALPHA_ON
			#pragma shader_feature _CANSPECULAR1_ON
	        #include "UnityCG.cginc"
		
	        
	        uniform fixed4 _Color;       
			uniform fixed4 _FirstLight;
			uniform fixed4 _FirstLightColour;

			uniform fixed4 _SecLight;
			uniform fixed4 _SecLightColour;

			uniform fixed4 _ThirdLight;
			uniform fixed4 _ThirdLightColour;
			
			uniform fixed4 _Specular1Colour;
			uniform fixed _Gloss1;

			uniform fixed4 _Specular2Colour;
			uniform fixed _Gloss2;

			uniform fixed4 _Specular3Colour;
			uniform fixed _Gloss3;
		
	        struct v2f {
	        	float4 pos : SV_POSITION;
	        	fixed2 uv : TEXCOORD0;
				float3 worldNormal:TEXCOORD1;
				float3 worldView:TEXCOORD2;		
	        };
	        
	        uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
	        
	        v2f vert(appdata_base v) 
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.vertex,_MainTex);
			o.worldNormal=normalize(UnityObjectToWorldNormal(v.normal));
			o.worldView=normalize(WorldSpaceViewDir(v.vertex));;
			return o;
		}
			
		fixed4 frag(v2f i) : Color {											
			fixed4 albedo = tex2D(_MainTex, i.uv);
			#if _CLIPALPHA_ON			
			clip(albedo.a - 0.5);
			#endif
			
             fixed4 colour1 = saturate(dot(normalize(_FirstLight) ,i.worldNormal))*_FirstLightColour;
			 fixed4 colour2 = saturate(dot(normalize(_SecLight) ,i.worldNormal))*_SecLightColour;
			 fixed4 colour3 = saturate(dot(normalize(_ThirdLight) ,i.worldNormal))*_ThirdLightColour;

			 fixed3 halfDir=normalize(i.worldNormal+i.worldView);
			 fixed4 finalC=albedo*(colour1+colour2+colour3)*_Color;

			 #if _CANSPECULAR1_ON			 
			 fixed4 specular1=_FirstLightColour*_Specular1Colour* pow(saturate(dot(halfDir,i.worldNormal)) ,_Gloss1);
    	     finalC+=specular1;
		     #endif
			 #if _CANSPECULAR2_ON			 
			 fixed4 specular2=_SecLightColour*_Specular2Colour* pow(saturate(dot(halfDir,i.worldNormal)) ,_Gloss2);
    	     finalC+=specular2;
		     #endif
			 #if _CANSPECULAR3_ON			 
			 fixed4 specular3=_ThirdLightColour*_Specular3Colour* pow(saturate(dot(halfDir,i.worldNormal)) ,_Gloss3);
    	     finalC+=specular3;
		     #endif
			 return finalC;					
		}
			
	        ENDCG
		}

	}
    }
	FallBack "Diffuse"
}
