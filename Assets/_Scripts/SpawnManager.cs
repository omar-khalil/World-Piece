using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager instance = null;

    public GameObject tilePrefab;
    public int padding;
    private int interval;
    private Vector3 initialSpawnPosition;

    private List<Tile> tileShop;
    public int initialMaxTiles;

    private int tileCounter = 1;

    public Button addMaxButton;
    public Button restockButton;

    private int addMaxCost = 0;
    private int restockCost = 0;

    public GameObject greenBlockPrefab;
    public GameObject yellowBlockPrefab;
    public GameObject whiteBlockPrefab;

    public float currentGreenProbability;
    public float currentWhiteProbability;
    public float currentYellowProbability;

    public float whiteProbabilityIncrement;
    public float yellowProbabilityIncrement;

    public float maxWhiteProbability;
    public float maxYellowProbability;

    public GameObject costContainer;
    public GameObject maxText;

    public float BlockPropSpawnChancePerMatch;
    public GameObject[] greenProps;
    public GameObject[] yellowProps;
    public GameObject[] whiteProps;
    public AnimationCurve propEntryYScaleOverTime;
    public float stretchSpeed;

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
        tileShop = new List<Tile>();
    }

    private void Start()
    {
        initialSpawnPosition = transform.position;
        interval = GridManager.instance.pointInterval + padding;
        for (int i = 0; i < initialMaxTiles; i++)
        {
            tileShop.Add(null);
        }
        Restock(false);
        restockButton.GetComponentInChildren<TextMeshProUGUI>().text = "-" + restockCost.ToString();
        addMaxButton.GetComponentInChildren<TextMeshProUGUI>().text =  "-" + addMaxCost.ToString();
    }

    private void Update()
    {
        restockButton.interactable = CoinManager.instance.coinCount >= restockCost;
        addMaxButton.interactable = tileShop.Count < 12 && CoinManager.instance.coinCount >= addMaxCost;
    }

    public void RestockButton()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.powerButton, true);
        CoinManager.instance.Spend(restockCost);
        Restock(true);
        restockCost += 1;
        restockButton.GetComponentInChildren<TextMeshProUGUI>().text = "-" + restockCost.ToString();

    }

    public void AddMaxButton()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.powerButton, true);
        CoinManager.instance.Spend(addMaxCost);
        IncreaseMax();
        addMaxCost += 1;
        if (tileShop.Count < 12)
        {
            addMaxButton.GetComponentInChildren<TextMeshProUGUI>().text = "-" + addMaxCost.ToString(); 
        } else
        {
            costContainer.SetActive(false);
            maxText.SetActive(true);
        }
    }

    public void CheckToRestock()
    {
        //for (int i = 0; i < tileShop.Count; i++)
        //{
        //    print(tileShop[i]);
        //}
        foreach (Tile tile in tileShop)
        {
            if (tile != null)
            {
                return;
            }
        }
        Restock(false);
    }

    public void Restock(bool deleteAll)
    {
        SoundManager.instance.PlaySound(SoundManager.instance.powerEffect, true);
        if (deleteAll)
        {
            for (int i = 0; i < tileShop.Count; i++)
            {
                RemoveTile(tileShop[i], true);
            }
        }
        for (int i = 0; i < tileShop.Count; i++)
        {
            if (tileShop[i] == null)
            {
                StartCoroutine(SpawnAtIndexDelay(i));
            }
        }
    }

    public void IncreaseProbabilities()
    {
        currentWhiteProbability += whiteProbabilityIncrement;
        currentWhiteProbability = Mathf.Clamp(currentWhiteProbability, 0f, maxWhiteProbability);
        currentYellowProbability += yellowProbabilityIncrement;
        currentYellowProbability = Mathf.Clamp(currentYellowProbability, 0f, maxYellowProbability);
        currentGreenProbability = 1 - (currentWhiteProbability + currentYellowProbability);
    }

    public void SpawnAtIndex(int i)
    {
        float x = initialSpawnPosition.x - (interval * (i / 2));
        float z = initialSpawnPosition.z - (interval * ((i + 1) / 2));
        Vector3 spawnPosition = new Vector3(x, 0f, z);
        GameObject spawnedTileGameObject = Instantiate(tilePrefab, spawnPosition, Quaternion.identity);

        TileType typeToSpawn;
        GameObject blockToSpawn;

        float chance = Random.Range(0f, 1f);

        if (chance <= currentYellowProbability)
        {
            typeToSpawn = TileType.DESSERT;
            blockToSpawn = yellowBlockPrefab;
            print(chance + " yellow");
        }
        else if (chance <= (currentWhiteProbability + currentYellowProbability))
        {
            typeToSpawn = TileType.SNOW;
            blockToSpawn = whiteBlockPrefab;
            print(chance + " white");
        }
        else
        {
            typeToSpawn = TileType.GRASS;
            blockToSpawn = greenBlockPrefab;
            print(chance + " green");
        }
        spawnedTileGameObject.GetComponent<Tile>().RandomizeTile(typeToSpawn, blockToSpawn);
        spawnedTileGameObject.transform.parent = transform;
        spawnedTileGameObject.name = "Tile " + tileCounter;
        CoinManager.instance.AssignCost(spawnedTileGameObject.GetComponent<Tile>());
        tileCounter++;
        tileShop[i] = spawnedTileGameObject.GetComponent<Tile>();
    }

    public void RemoveTile(Tile tile, bool delete = false)
    {
        if (tile != null)
        {
            int index = tileShop.IndexOf(tile);
            if (delete)
            {
                Destroy(tile.gameObject);
            }
            //print(tileShop[index]);
            tileShop[index] = null;
            //print(tileShop[index]); 
        }
    }

    IEnumerator SpawnAtIndexDelay(int i)
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.5f));
        SpawnAtIndex(i);
    }

    public void IncreaseMax()
    {
        tileShop.Add(null);
        SpawnAtIndex(tileShop.Count - 1);
    }

    public GameObject GetPropForTileType(TileType tileType)
    {
        switch (tileType)
        {
            case TileType.GRASS:
                return greenProps[Random.Range(0, greenProps.Length)];
            case TileType.SNOW:
                return whiteProps[Random.Range(0, whiteProps.Length)];
            case TileType.DESSERT:
                return yellowProps[Random.Range(0, yellowProps.Length)];
            default:
                return greenProps[Random.Range(0, greenProps.Length)];
        }
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }
}
