using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    GRASS, SNOW, DESSERT
}

public class Tile : MonoBehaviour
{
    private Plane plane;

    public Edge[] edges;

    [HideInInspector]
    public bool placed = false;

    private Vector3 initialPosition;

    [HideInInspector]
    public float logicalYRotation;

    [HideInInspector]
    public SmoothMovement smoothMovement;

    [HideInInspector]
    public int cost;
    public GameObject coinUICanvas;
    public GameObject coinUIPrefab;
    public Transform coinsUIContainer;
    [HideInInspector]
    public TileType tileType;

    private bool dragging = false;

    private List<GameObject> coinUIs;

    public Block[] allBlocks;

    private void Awake()
    {
        smoothMovement = GetComponent<SmoothMovement>();
        coinUIs = new List<GameObject>();
    }

    private void Start()
    {
        foreach (Block block in allBlocks)
        {
            block.parentTile = this;
        }
        foreach (Edge edge in edges)
        {
            edge.parentTile = this;
        }
        initialPosition = transform.position;
        plane = new Plane(Vector3.up, 0);
    }

    public void RandomizeTile(TileType tileType, GameObject block)
    {
        foreach (Block childBlock in allBlocks)
        {
            GameObject blockObject = Instantiate(block, childBlock.transform);
            blockObject.transform.localPosition = Vector3.zero;
        }
        this.tileType = tileType;
        foreach (Edge edge in edges)
        {
            edge.RandomizeEdge(tileType);
        }
    }

    public void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            Vector3 pointalongplane = ray.origin + (ray.direction * distance);
            transform.position = pointalongplane;
        }

        if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Space))
        {
            SoundManager.instance.PlaySound(SoundManager.instance.rotateClip, true);
            RotateRight();
        }
    }

    public void RotateRight()
    {
        logicalYRotation += 90f;
        logicalYRotation %= 360f;
        //transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, logicalYRotation, transform.localEulerAngles.z);
        smoothMovement.RotateTo(new Vector3(transform.localEulerAngles.x, logicalYRotation, transform.localEulerAngles.z));
        coinUICanvas.GetComponent<SmoothMovement>().RotateTo(new Vector3(coinUICanvas.transform.localEulerAngles.x, coinUICanvas.transform.localEulerAngles.y - 90f, coinUICanvas.transform.localEulerAngles.z));
    }

    private void SpawnPropsOnBlocks(int matches)
    {
        bool oneSpawned = false;
        foreach (Block block in allBlocks)
        {
            if (block.gameObject.activeSelf && Random.Range(0f, 1f) < (SpawnManager.instance.BlockPropSpawnChancePerMatch * matches))
            {
                block.SpawnProp();
                oneSpawned = true;
            }
        }
        if (!oneSpawned)
        {
            allBlocks[Random.Range(0, allBlocks.Length)].SpawnProp();
        }
    }

    private void PlaceTile(List<Tile> successfulTiles, List<Edge> successfulEdges, bool playParticles)
    {
        SpawnPropsOnBlocks(successfulTiles.Count - 1);
        foreach (Tile tile in successfulTiles)
        {
            tile.SpawnPropsOnBlocks(successfulTiles.Count - 1);
            //CoinManager.instance.Spawn3DCoin(tile.transform.position);
        }
        ProjectionManager.instance.HideProjectionTile();
        SpawnManager.instance.RemoveTile(this);
        GridManager.instance.PlaceTile(this, transform.position);
        if (playParticles)
        {
            foreach (Edge edge in successfulEdges)
            {
                edge.PlayEdgeParticles(successfulEdges.Count > 1);
            }
        }
    }

    public void Drag()
    {
        //Some duplicate code. Sue me.
        Validity validity = GridManager.instance.CheckPointValidity(transform.position.x, transform.position.z);
        if (validity == Validity.VALID)
        {
            List<Tile> adjacentTiles = GridManager.instance.GetAdjacentTiles(transform.position.x, transform.position.z);
            bool noConflicts = adjacentTiles.Count > 0;
            foreach (Tile adjacentTile in adjacentTiles)
            {
                Edge successfulEdge;
                bool colorMatch;
                noConflicts = CheckAdjacentMatch(adjacentTile, out successfulEdge, out colorMatch) && noConflicts;
            }

            if (noConflicts)
            {
                ProjectionManager.instance.ProjectAt(GridManager.instance.GetGridPointAt(transform.position.x, transform.position.z), this, true);
            }
            else
            {
                ProjectionManager.instance.ProjectAt(GridManager.instance.GetGridPointAt(transform.position.x, transform.position.z), this, false);
            }
        }
        else if (validity == Validity.ON_TILE)
        {
            ProjectionManager.instance.ProjectAt(GridManager.instance.GetGridPointAt(transform.position.x, transform.position.z), this, false);
        }
        else
        {
            ProjectionManager.instance.HideProjectionTile();
        }
        Move();
    }

    public bool CheckAdjacentMatch(Tile adjacentTile, out Edge successfulEdge, out bool colorMatch)
    {
        Vector3 thisPointPosition = GridManager.instance.GetGridPointAt(transform.position.x, transform.position.z);
        Vector3 adjacentTilePointPosition = GridManager.instance.GetGridPointAt(adjacentTile.transform.position.x, adjacentTile.transform.position.z);
        Vector3 direction = (adjacentTilePointPosition - thisPointPosition).normalized;
        //print(direction);
        float angle = Vector3.SignedAngle(Vector3.forward, direction, Vector3.up);
        //print(angle);
        Edge thisEdge = GetEdge(this, angle);
        Edge adjacentEdge = GetEdge(adjacentTile, angle + 180f);
        return CheckMatchBetweenBetweenEdges(thisEdge, adjacentEdge, out successfulEdge, out colorMatch);
    }

    public bool CheckMatchBetweenBetweenEdges(Edge edgeA, Edge edgeB, out Edge successfulEdge, out bool colorMatch)
    {
        //print(edgeA.edgeState + " on " + edgeB.edgeState);
        if (edgeA.edgeState != edgeB.edgeState)
        {
            successfulEdge = edgeA;
        }
        else
        {
            successfulEdge = null;
        }
        colorMatch = edgeA.tileType == edgeB.tileType;
        return edgeA.edgeState != edgeB.edgeState; //if one is IN and the other is OUT
    }

    public Edge GetEdge(Tile tile, float angle)
    {
        angle -= tile.logicalYRotation; //In case tile is rotated
        if (angle < 0)
        {
            angle += 360;
        }
        angle %= 360f;
        switch (angle)
        {
            case 0f:
                //print("Top edge of " + tile.name);
                return GetEdge(tile, EdgeDirection.FORWARD);
            case 90f:
                //print("Right edge of " + tile.name);
                return GetEdge(tile, EdgeDirection.RIGHT);
            case 180f:
                //print("Bottom edge of " + tile.name);
                return GetEdge(tile, EdgeDirection.BACK);
            case 270f:
                //print("Left edge of " + tile.name);
                return GetEdge(tile, EdgeDirection.LEFT);
            default:
                //print("How did you get here? Angle: " + angle);
                return GetEdge(tile, EdgeDirection.FORWARD);
        }

    }

    public Edge GetEdge(Tile tile, EdgeDirection edgeDirection)
    {
        foreach (Edge edge in tile.edges)
        {
            if (edge.edgeDirection == edgeDirection)
            {
                return edge;
            }
        }
        return null; //Shouldn't happen if all edges are assigned a different direction
    }

    public void StopDrag()
    {
        Validity validity = GridManager.instance.CheckPointValidity(transform.position.x, transform.position.z);
        if (validity == Validity.VALID)
        {
            List<Tile> adjacentTiles = GridManager.instance.GetAdjacentTiles(transform.position.x, transform.position.z);
            bool noConflicts = adjacentTiles.Count > 0;
            //print("Adjacent tiles = " + adjacentTiles.Count);

            int colorMatchCount = 0;
            List<Edge> successfulEdges = new List<Edge>();
            List<Tile> successfulTiles = new List<Tile>();
            foreach (Tile adjacentTile in adjacentTiles)
            {
                Edge successfulEdge;
                bool colorMatch;
                noConflicts = CheckAdjacentMatch(adjacentTile, out successfulEdge, out colorMatch) && noConflicts;
                if (successfulEdge != null)
                {
                    successfulEdges.Add(successfulEdge);
                }
                if (colorMatch && successfulEdge != null)
                {
                    successfulTiles.Add(adjacentTile);
                }
                colorMatchCount += colorMatch ? 1 : 0;
            }

            if (noConflicts)
            {
                PlaceTile(successfulTiles, successfulEdges, true);
                CoinManager.instance.GiveRewardForMatches(tileType, colorMatchCount, successfulTiles);
            }
            else
            {
                print("Returning because conflict");
                //transform.position = initialPosition;
                smoothMovement.MoveTo(initialPosition);
                SoundManager.instance.PlaySound(SoundManager.instance.invalidClip, true);
                CoinManager.instance.Refund(this);
                ReturnCoins();
            }
        }
        else
        {
            print("Returning because invalid");
            //print(validity);
            //transform.position = initialPosition;
            smoothMovement.MoveTo(initialPosition);
            SoundManager.instance.PlaySound(SoundManager.instance.invalidClip, true);
            CoinManager.instance.Refund(this);
            ReturnCoins();
        }
        dragging = false;
        ProjectionManager.instance.HideProjectionTile();
    }

    private void OnMouseDown()
    {
        if (CoinManager.instance.Spend(this) && !placed)
        {
            RemoveCoins();
            SoundManager.instance.PlaySound(SoundManager.instance.pickupClip, true);
            dragging = true;
        } else
        {
            CoinManager.instance.ErrorCoins();
            SoundManager.instance.PlaySound(SoundManager.instance.invalidClip, true);
        }
    }

    private void OnMouseDrag()
    {
        if (dragging)
        {
            Drag();
            //Check validity 
        }
    }

    private void OnMouseUp()
    {
        if (dragging)
        {
            StopDrag();
        }
    }

    public void AssignCost(int cost)
    {
        this.cost = cost;
        coinUICanvas.SetActive(true);
        for (int i = 0; i < cost; i++)
        {
            GameObject coinUI = Instantiate(coinUIPrefab, coinsUIContainer);
            coinUIs.Add(coinUI);
        }
    }

    public void RemoveCoins()
    {
        foreach (GameObject coinUI in coinUIs)
        {
            coinUI.GetComponent<Animator>().SetTrigger("Spend");
        }
    }

    public void ReturnCoins()
    {
        foreach (GameObject coinUI in coinUIs)
        {
            coinUI.GetComponent<Animator>().SetTrigger("Refund");
        }
    }
}
