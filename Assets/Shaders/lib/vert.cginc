#include "vertexLookup.cginc"

VertexOutput vert (uint id : SV_VertexID) {
    VertexOutput o;
    VertexData v = vertexLookup(id);
    o.pos = mul(UNITY_MATRIX_MVP, mul(objectToWorld, float4(v.vertex, 1.0)));
    o.normal = UnityObjectToWorldNormal(v.normal);
    return o;
}
