using UnityEngine;
using System.Collections;
using System.Collections.Generic;

struct GPUEdgeValues {
    public float edge0Val, edge1Val, edge2Val, edge3Val, edge4Val, edge5Val, edge6Val, edge7Val;
}

struct GPUPositions {
    public Vector3 centerPos;
    public Vector3 edge0Pos, edge1Pos, edge2Pos, edge3Pos, edge4Pos, edge5Pos, edge6Pos, edge7Pos;
}

struct GPUBall {
    public float factor;
    public Vector3 position;
}

struct GPUEdgeVertices {
    public int index;
    public Vector3 edge0, edge1, edge2, edge3, edge4, edge5, edge6, edge7, edge8, edge9, edge10, edge11;
};


struct GPUVertexIndices {
    public int cubeIndex;
    // public int lookupIndex0;
    // public int lookupIndex1;
    // public int lookupIndex2;
};


public class MetaballGrid : MonoBehaviour {
    public int width, height, depth;
    public int vertexCount;

    private MetaballRenderer metaballRenderer;

    private ComputeShader shader;
    private int shaderKernel;

    public ComputeBuffer edgeVerticesBuffer;
    public ComputeBuffer vertexIndicesBuffer;

    private ComputeBuffer positionsBuffer;
    private ComputeBuffer metaballsBuffer;
    private ComputeBuffer edgeMapBuffer;

    private GPUPositions[] precomputedPositions;
    private GPUVertexIndices[] vertexIndices;

    private bool initialized;

    private int vertIndex;
    private int cubeIndex;

    private GPUBall[] gpuBalls;
    [HideInInspector]
    public Metaball[] metaballs;

    private Vector3 centerPoint;
    private Vector3[,,] positionMap;
    private Vector3[] edgeMap;

    public void Setup(MetaballRenderer metaballRenderer, ComputeShader shader) {
        this.metaballRenderer = metaballRenderer;
        this.shader = shader;

        width = metaballRenderer.resolution * 8;
        height = metaballRenderer.resolution * 8;
        depth = metaballRenderer.resolution * 8;

        // prepare data for the marching cubes algorithm on GPU
        vertexCount = width * height * depth;

        // if (!metaballRenderer.useGeometryShader)
        //     vertexCount *= 15;

        vertexIndices = new GPUVertexIndices[vertexCount];
        vertIndex = 0;
        cubeIndex = 0;
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < depth; z++) {
                    cubeIndex = x + width * (y + height * z);
                    // if (metaballRenderer.useGeometryShader) {
                        vertexIndices[vertIndex].cubeIndex = cubeIndex;
                        vertIndex ++;
                    // } else {
                    //     for (int k = 0; k < 15; k += 3) {
                    //         vertexIndices[vertIndex].cubeIndex = cubeIndex;
                    //         vertexIndices[vertIndex].lookupIndex0 = k + 0;
                    //         vertexIndices[vertIndex].lookupIndex1 = k + 2;
                    //         vertexIndices[vertIndex].lookupIndex2 = k + 1;
                    //         vertIndex ++;
                    //         vertexIndices[vertIndex].cubeIndex = cubeIndex;
                    //         vertexIndices[vertIndex].lookupIndex0 = k + 2;
                    //         vertexIndices[vertIndex].lookupIndex1 = k + 1;
                    //         vertexIndices[vertIndex].lookupIndex2 = k + 0;
                    //         vertIndex ++;
                    //         vertexIndices[vertIndex].cubeIndex = cubeIndex;
                    //         vertexIndices[vertIndex].lookupIndex0 = k + 1;
                    //         vertexIndices[vertIndex].lookupIndex1 = k + 0;
                    //         vertexIndices[vertIndex].lookupIndex2 = k + 2;
                    //         vertIndex ++;
                    //     }
                    // }
                }
            }
        }
        // TODO: remove lookup indices when using geometry shaders
        vertexIndicesBuffer = new ComputeBuffer(vertexIndices.Length, 4);
        vertexIndicesBuffer.SetData(vertexIndices);
        initialized = false;
    }

    void OnEnable() {
        metaballs = FindObjectsOfType<Metaball>();
    }

    public void evaluateAll() {
        if (metaballs == null) {
            return;
        }

        if(!initialized) {
            init();
        }

        // write info about metaballs in format readable by compute shaders
        gpuBalls = new GPUBall[metaballs.Length];
        for (int i = 0; i < metaballs.Length; i++) {
            Metaball metaball = metaballs[i];
            gpuBalls[i].position = metaballRenderer.transform.localToWorldMatrix.inverse.MultiplyPoint3x4(metaball.transform.position);
            gpuBalls[i].factor = metaball.factor / Mathf.Pow(((metaballRenderer.transform.lossyScale.x + metaballRenderer.transform.lossyScale.y + metaballRenderer.transform.lossyScale.z) / 3.0f), 2.0f);
        }

        runComputeShader(gpuBalls);
    }

    public void Release() {
        positionsBuffer.Release();
        metaballsBuffer.Release();
        edgeMapBuffer.Release();
        edgeVerticesBuffer.Release();
        vertexIndicesBuffer.Release();
    }

    private void init() {
        instantiateEdgeMap();
        instantiatePositionMap();
        instantiateGPUPositions();
        instantiateComputeShader();
        initialized = true;
    }

    private void instantiateEdgeMap() {
        edgeMap = new Vector3[] {
            new Vector3(-1, -1, -1),
            new Vector3(1, -1, -1),
            new Vector3(1, 1, -1),
            new Vector3(-1, 1, -1),
            new Vector3(-1, -1, 1),
            new Vector3(1, -1, 1),
            new Vector3(1, 1, 1),
            new Vector3(-1, 1, 1)
        };

        // scale edge map
        for (int i = 0; i < 8; i++) {
            edgeMap[i] /= 2;
            edgeMap[i] = new Vector3(edgeMap[i].x / ((float) width),
                edgeMap[i].y / ((float) height),
                edgeMap[i].z / ((float) depth));
        }
    }

    private void instantiatePositionMap() {
        positionMap = new Vector3[width, height, depth];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < depth; z++) {
                    float xCoord = (((float) x) / ((float) width)) - 0.5f;
                    float yCoord = (((float) y) / ((float) height)) - 0.5f;
                    float zCoord = (((float) z) / ((float) depth)) - 0.5f;
                    positionMap[x, y, z] = new Vector3(xCoord, yCoord, zCoord);
                }
            }
        }
    }

    private void instantiateGPUPositions() {
        precomputedPositions = new GPUPositions[width*height*depth];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < depth; z++) {
                    centerPoint = positionMap[x, y, z];
                    precomputedPositions[x + width * (y + height * z)].centerPos = centerPoint;
                    precomputedPositions[x + width * (y + height * z)].edge0Pos = centerPoint + edgeMap[0];
                    precomputedPositions[x + width * (y + height * z)].edge1Pos = centerPoint + edgeMap[1];
                    precomputedPositions[x + width * (y + height * z)].edge2Pos = centerPoint + edgeMap[2];
                    precomputedPositions[x + width * (y + height * z)].edge3Pos = centerPoint + edgeMap[3];
                    precomputedPositions[x + width * (y + height * z)].edge4Pos = centerPoint + edgeMap[4];
                    precomputedPositions[x + width * (y + height * z)].edge5Pos = centerPoint + edgeMap[5];
                    precomputedPositions[x + width * (y + height * z)].edge6Pos = centerPoint + edgeMap[6];
                    precomputedPositions[x + width * (y + height * z)].edge7Pos = centerPoint + edgeMap[7];
                }
            }
        }
    }

    private void instantiateComputeShader() {
        positionsBuffer = new ComputeBuffer(precomputedPositions.Length, 108);
        positionsBuffer.SetData(precomputedPositions);
        edgeMapBuffer = new ComputeBuffer(8, 12);
        edgeMapBuffer.SetData(edgeMap);
        edgeVerticesBuffer = new ComputeBuffer(precomputedPositions.Length, 148);
        metaballsBuffer = new ComputeBuffer(precomputedPositions.Length, 16);
    }

    // GPU metaball falloff function summator & part of marching cubes algorithm
    private void runComputeShader(GPUBall[] gpuBalls) {
        shaderKernel = shader.FindKernel("Calculate");

        metaballsBuffer.SetData(gpuBalls);

        shader.SetBuffer(shaderKernel, "positions", positionsBuffer);
        shader.SetBuffer(shaderKernel, "metaballs", metaballsBuffer);
        shader.SetBuffer(shaderKernel, "edgeMap", edgeMapBuffer);
        shader.SetBuffer(shaderKernel, "edgeVertices", edgeVerticesBuffer);

        shader.SetInt("numMetaballs", gpuBalls.Length);
        shader.SetInt("width", width);
        shader.SetInt("height", height);
        shader.SetFloat("threshold", metaballRenderer.threshold);
        // Run compute shader
        shader.Dispatch(shaderKernel, width / 8, height / 8, depth / 8);
    }
}
