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
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2

            #include "UnityCG.cginc"

            uniform float4x4 objectToWorld;

            // defines StructuredBuffer<VertexIndices> vertexIndicesBuffer;
            // defines StructuredBuffer<EdgeVertices> edgeVerticesBuffer;
            #include "lib/vertexLookup.cginc"

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD1;
            };

            struct GeometryOutput {
                float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD2;
            };

            VertexOutput vert (uint id : SV_VertexID) {
                VertexOutput o;
                float3 pos = vertexLookup(id);
                o.posWorld = mul(objectToWorld, float4(pos, 1.0));
                o.pos = mul(UNITY_MATRIX_MVP, o.posWorld);
                return o;
            }

            [maxvertexcount(3)]
            void geom(triangle VertexOutput input[3], inout TriangleStream<GeometryOutput> OutputStream) {
            float3 normal = normalize(cross(input[1].posWorld.xyz - input[0].posWorld.xyz, input[2].posWorld.xyz - input[0].posWorld.xyz));
              GeometryOutput o = (GeometryOutput)0;
              for(int i = 0; i < 3; i++)
              {
                  o.normalDir = normal;
                  o.pos = input[i].pos;
                  OutputStream.Append(o);
              }
            }

            float4 frag (GeometryOutput i) : COLOR {
                float3 polyNormalDir = i.normalDir;
                return float4(polyNormalDir,1.);
            }
            ENDCG
        }
    }
    Fallback Off
}
