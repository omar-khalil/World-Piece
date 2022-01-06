using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance = null;

    public TextMeshProUGUI coinText;
    public Animator coinIconAnimator;
    public int startingCoins;
    public int grassCost;
    public int snowCost;
    public int dessertCost;
    private int totalPlacedCount = 0;
    [HideInInspector]
    public int coinCount;

    public int grassRewardBase;
    public int snowRewardBase;
    public int dessertRewardBase;

    public GameObject coinPrefab;
    public float coinSpawnMaxDelay;

    public GameObject playAgainButton;

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

    private void Start()
    {
        AddCoins(startingCoins);
    }

    public bool Spend(Tile tile)
    {
        if (coinCount >= tile.cost)
        {
            AddCoins(-tile.cost);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Refund(Tile tile)
    {
        AddCoins(tile.cost);
    }

    public void AssignCost(Tile tile)
    {
        int cost;
        switch (tile.tileType)
        {
            case TileType.GRASS:
                cost = grassCost;
                break;
            case TileType.SNOW:
                cost = snowCost;
                break;
            case TileType.DESSERT:
                cost = dessertCost;
                break;
            default:
                cost = 4;
                break;
        }
        tile.AssignCost(cost);
    }

    public void GiveRewardForMatches(TileType tiletype, int matchCount, List<Tile> matchedTiles)
    {
        AddCoins(CalculateReward(tiletype, matchCount, matchedTiles));
    }

    public int CalculateReward(TileType tileType, int matchCount, List<Tile> matchedTiles)
    {
        int rewardBase;
        switch (tileType)
        {
            case TileType.GRASS:
                rewardBase = grassRewardBase;
                break;
            case TileType.SNOW:
                rewardBase = snowRewardBase;
                break;
            case TileType.DESSERT:
                rewardBase = dessertRewardBase;
                break;
            default:
                rewardBase = 1;
                print("How'd you get here??");
                break;
        }
        if (matchCount == 1)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.matchOk, true);
            rewardBase = 0;
        }
        else if (matchCount == 2)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.matchOk, 1.2f, 1, false);
        }
        else if (matchCount == 3)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.matchOk, 1.4f, 1, false);
        }
        else if (matchCount == 4)
        {
            SoundManager.instance.PlaySound(SoundManager.instance.matchOk, 1.6f, 1, false);
        }
        //print("Reward = " + (matchCount * rewardBase));
        foreach (Tile matchedTile in matchedTiles)
        {
            if (matchCount == 1)
            {
                break;
            }
            else
            {
                Spawn3DCoin(matchedTile.transform.position, rewardBase);
            }
        }
        return matchCount * rewardBase;
    }

    public void Spend(int amount)
    {
        AddCoins(-amount);
    }

    public void Spawn3DCoin(Vector3 position, int count)
    {
        StartCoroutine(Spawn3DCoinDelay(position, count));
    }

    IEnumerator Spawn3DCoinDelay(Vector3 position, int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(Random.Range(0f, coinSpawnMaxDelay));
            SoundManager.instance.PlaySound(SoundManager.instance.coin, true);
            GameObject coin = Instantiate(coinPrefab, position, Quaternion.identity);
            Destroy(coin, 0.67f);
        }
    }

    private void AddCoins(int amount)
    {
        if (amount > 0)
        {
            coinIconAnimator.SetTrigger("Add");
        }
        coinCount += amount;
        coinText.text = coinCount.ToString();
        if (coinCount <= 0)
        {
            playAgainButton.SetActive(true);
        }
    }

    public void ErrorCoins()
    {
        coinIconAnimator.SetTrigger("Error");
    }
}
