using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum Validity
{
    NO_ADJACENT_TILES, ON_TILE, VALID
}

public class GridManager : MonoBehaviour
{
    public static GridManager instance = null;
    public int pointInterval;
    public Tile homeTile;

    private Dictionary<Vector3, Tile> placedTiles;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        placedTiles = new Dictionary<Vector3, Tile>();
        PlaceTile(homeTile, GetGridPointAt(homeTile.transform.position.x, homeTile.transform.position.z));
        homeTile.RandomizeTile(TileType.GRASS, SpawnManager.instance.greenBlockPrefab);
    }

    public void PlaceTile(Tile tile, Vector3 position)
    {
        Vector3 newPoint = GetGridPointAt(position.x, position.z);
        placedTiles.Add(newPoint, tile);
        tile.smoothMovement.MoveTo(newPoint);
        //tile.transform.position = newPoint;
        tile.placed = true;
        SpawnManager.instance.CheckToRestock();
        SpawnManager.instance.IncreaseProbabilities();
    }

    public Vector3 GetGridPointAt(float x, float z)
    {
        float vx = Mathf.Round(x / pointInterval) * pointInterval;
        float vz = Mathf.Round(z / pointInterval) * pointInterval;
        return new Vector3(vx, 0f, vz);
    }

    public List<Tile> GetAdjacentTiles(float x, float z)
    {
        Vector3 point = GetGridPointAt(x, z);
        List<Tile> adjacentTiles = new List<Tile>();
        Vector3[] directions = { Vector3.forward * pointInterval, Vector3.right * pointInterval, Vector3.back * pointInterval, Vector3.left * pointInterval };
        foreach (Vector3 direction in directions)
        {
            Vector3 adjacentPoint = point + direction;
            if (placedTiles.ContainsKey(adjacentPoint))
            {
                adjacentTiles.Add(placedTiles[adjacentPoint]);
            }
        }
        return adjacentTiles;
    }

    public Validity CheckPointValidity(float x, float z)
    {
        Vector3 point = GetGridPointAt(x, z);
        if (placedTiles.ContainsKey(point))
        {
            return Validity.ON_TILE;
        }
        List<Tile> adjacentTiles = GetAdjacentTiles(x, z);
        if (adjacentTiles.Count == 0)
        {
            //print(GetGridPointAt(x, z));

            //print(placedTiles.Keys.Count);
            //Vector3[] keys = new Vector3[placedTiles.Keys.Count];
            //placedTiles.Keys.CopyTo(keys, 0);
            //print(keys[0]);

            return Validity.NO_ADJACENT_TILES;
        }
        return Validity.VALID;
    }
}
