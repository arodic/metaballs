#include "triTable.cginc"

struct VertexIndices {
    int cubeIndex;
    int lookupIndex;
};

struct EdgeVertices {
    int index;
    float3 edge0, edge1, edge2, edge3, edge4, edge5, edge6, edge7, edge8, edge9, edge10, edge11;
};

StructuredBuffer<VertexIndices> vertexIndicesBuffer;
StructuredBuffer<EdgeVertices> edgeVerticesBuffer;

float3 getVertexEdge(EdgeVertices vert, int i) {
    if(i == -1) return float3(0,0,0);
    if(i == 0) return vert.edge0;
    if(i == 1) return vert.edge1;
    if(i == 2) return vert.edge2;
    if(i == 3) return vert.edge3;
    if(i == 4) return vert.edge4;
    if(i == 5) return vert.edge5;
    if(i == 6) return vert.edge6;
    if(i == 7) return vert.edge7;
    if(i == 8) return vert.edge8;
    if(i == 9) return vert.edge9;
    if(i == 10) return vert.edge10;
    return vert.edge11;
};

float3 vertexLookup(int id) {
    VertexIndices c = vertexIndicesBuffer[id];
    EdgeVertices e = edgeVerticesBuffer[c.cubeIndex];
    return getVertexEdge(e, triTable[e.index][c.lookupIndex]);
};
