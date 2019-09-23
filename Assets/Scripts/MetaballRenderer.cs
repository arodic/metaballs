using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MetaballRenderer : MonoBehaviour {
    [Range(1, 25)]
    public int resolution = 10;
    [Range(0.01f, 10.0f)]
    public float threshold = 1;
    public ComputeShader computeShader;
    public Material material;
    // public bool useGeometryShader = true;

    private MetaballGrid grid;

    public void Update() {
        grid.evaluateAll();
    }

    void Render(Camera c){
        material.SetPass(0);
        material.SetMatrix("objectToWorld", transform.localToWorldMatrix); // TODO: unity_ObjectToWorld not working
        material.SetBuffer("vertexIndicesBuffer", grid.vertexIndicesBuffer);
        material.SetBuffer("edgeVerticesBuffer", grid.edgeVerticesBuffer);
        Graphics.DrawProceduralNow(MeshTopology.Triangles,  grid.vertexCount);
    }

    void OnEnable () {
        grid = gameObject.AddComponent<MetaballGrid>();
        grid.Setup(this, computeShader);
        Camera.onPostRender += Render;
    }

    void OnDisable(){
        grid.Release();
        Camera.onPostRender -= Render;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
    }
}
