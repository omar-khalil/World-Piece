using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EdgeDirection
{
    LEFT, RIGHT, FORWARD, BACK
}
public enum EdgeState
{
    IN, OUT
}

public class Edge : MonoBehaviour
{
    public GameObject blockIn;
    public GameObject blockOut;
    public EdgeDirection edgeDirection;
    public EdgeState edgeState;
    public ParticleSystem goodParticles;
    public ParticleSystem normalParticles;
    public TileType tileType;
    public Tile parentTile;

    public void RandomizeEdge(TileType tileType)
    {
        int random = Random.Range(0, 2);
        blockIn.SetActive(random == 1);
        blockOut.SetActive(random == 1);
        //print(random);
        edgeState = random == 1 ? EdgeState.OUT : EdgeState.IN;
        this.tileType = tileType;
    }

    public void PlayEdgeParticles(bool good)
    {
        if (good)
        {
            goodParticles.Play(); 
        } else
        {
            normalParticles.Play();
        }
    }

    public void SetEdge(EdgeState edgeState)
    {
        this.edgeState = edgeState;
        blockIn.SetActive(edgeState == EdgeState.OUT);
        blockOut.SetActive(edgeState == EdgeState.OUT);
    }
}
