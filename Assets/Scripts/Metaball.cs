using UnityEngine;
using System.Collections;

public class Metaball : MonoBehaviour {

    public bool negativeBall;

    [HideInInspector]
    public float radius;
    [HideInInspector]
    public float factor;

    [HideInInspector]
    public Metaball[] metaballs;
    [HideInInspector]
    public MetaballGrid[] metaballGrids;

    public void Update() {
        radius = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 3.0f;
        factor = (negativeBall ? -1 : 1) * radius * radius;
    }

    void OnEnable () {
        UpdateMetaballGrids();
    }

    void OnDisable() {
        UpdateMetaballGrids();
    }

    void UpdateMetaballGrids() {
        metaballGrids = FindObjectsOfType<MetaballGrid>();
        metaballs = FindObjectsOfType<Metaball>();
        for (int i = 0; i < metaballGrids.Length; i++) {
            metaballGrids[i].metaballs = metaballs;
        }
    }

    void OnDrawGizmos() {
        radius = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 6.0f;
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    void OnDrawGizmosSelected() {
        radius = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 6.0f;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
