using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectionManager : MonoBehaviour
{
    
    public static ProjectionManager instance = null;

    [SerializeField]
    private GameObject projectionTile;
    [SerializeField]
    private Material projectionMaterial;
    public Color matchColor;
    public Color nonMatchColor;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
            return;
        }
    }

    //Back right left forward
    public void ProjectAt(Vector3 position, Tile tile, bool match)
    {
        projectionTile.transform.localEulerAngles = new Vector3(projectionTile.transform.localEulerAngles.x, tile.logicalYRotation, projectionTile.transform.localEulerAngles.z);
        projectionTile.transform.position = position;
        Edge[] projectionEdges = projectionTile.GetComponent<Tile>().edges;
        for(int i = 0; i < projectionEdges.Length; i++)
        {
            projectionEdges[i].SetEdge(tile.edges[i].edgeState);
        }
        projectionMaterial.color = match ? matchColor : nonMatchColor;
        projectionTile.SetActive(true);
    }

    public void HideProjectionTile()
    {
        projectionTile.SetActive(false);
    }
}
