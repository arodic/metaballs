// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#include "vertexLookup.cginc"

struct GeometryInput {
    int cubeIndex : TEXCOORD1;
};

GeometryInput vert_geom (uint id : SV_VertexID) {
    GeometryInput o;
    o.cubeIndex = getCubeIndex(id);
    return o;
}


[maxvertexcount(45)]
void geom(triangle GeometryInput input[3], inout TriangleStream<VertexOutput> OutputStream)
{
     for (int i = 0; i < 3; i ++) {
         for (int k = 0; k < 15; k += 3) {
            float3 v0 = getVertex(input[i].cubeIndex, k + 2);
            if (v0.x == 0.0f) break; // early out when end of triTable array is reached.
            float3 v1 = getVertex(input[i].cubeIndex, k + 1);
            float3 v2 = getVertex(input[i].cubeIndex, k + 0);
            float3 normal = normalize(cross(normalize(v1 - v0), normalize(v2 - v0)));
            VertexOutput g0 = (VertexOutput)0;
            VertexOutput g1 = (VertexOutput)0;
            VertexOutput g2 = (VertexOutput)0;
            g0.normal = normal;
            g1.normal = normal;
            g2.normal = normal;
            g0.pos = UnityObjectToClipPos(mul(objectToWorld, float4(v0, 1.0)));
            g1.pos = UnityObjectToClipPos(mul(objectToWorld, float4(v1, 1.0)));
            g2.pos = UnityObjectToClipPos(mul(objectToWorld, float4(v2, 1.0)));
            OutputStream.Append(g0);
            OutputStream.Append(g1);
            OutputStream.Append(g2);
            OutputStream.RestartStrip();
         }
     }
}
