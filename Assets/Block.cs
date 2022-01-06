using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    public Tile parentTile;
    public void SpawnProp()
    {
        SoundManager.instance.PlaySound(SoundManager.instance.pop, true);
        GameObject propToSpawn = SpawnManager.instance.GetPropForTileType(parentTile.tileType);
        GameObject prop = Instantiate(propToSpawn, transform.parent);
        prop.transform.localPosition = Vector3.zero;
        prop.transform.localEulerAngles = new Vector3(prop.transform.localEulerAngles.x, Random.Range(0f, 360f), prop.transform.localEulerAngles.z);
        prop.AddComponent<StretchOnEntry>();
    }
}
