// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

#include "vertexLookup.cginc"

VertexOutput vert (uint id : SV_VertexID) {
    VertexOutput o;
    VertexData v = vertexLookup(id);
    o.pos = UnityObjectToClipPos(mul(objectToWorld, float4(v.vertex, 1.0)));
    o.normal = UnityObjectToWorldNormal(v.normal);
    return o;
}
