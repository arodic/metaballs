// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/Metaball" {
    Properties {

    }
    SubShader{
        Tags{
            "RenderType" = "Opaque"
        }
        Pass{
            Name "FORWARD"
            Tags {
                "LightMode" = "ForwardBase"
            }
            CGPROGRAM

            #pragma target 4.0
            //#pragma vertex vert
            #pragma vertex vert_geom
            #pragma geometry geom
            #pragma fragment frag
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2

            #include "UnityCG.cginc"

            uniform float4x4 objectToWorld;

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float3 normal : TEXCOORD1;
            };

            #include "lib/geom.cginc"
            //#include "lib/vert.cginc"

            float4 frag (VertexOutput i) : COLOR {
                float3 polyNormalDir = i.normal;
                return float4(polyNormalDir,1.);
            }
            ENDCG
        }
    }
    Fallback Off
}
