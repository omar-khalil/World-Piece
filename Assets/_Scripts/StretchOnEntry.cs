using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StretchOnEntry : MonoBehaviour
{
    bool stretching;
    float stretchTime;
    Transform childTransform;
    float initialYScale;
    // Start is called before the first frame update
    void Start()
    {
        stretching = false;
        childTransform = transform.GetChild(0);
        initialYScale = childTransform.localScale.y;
        childTransform.localScale = new Vector3(childTransform.localScale.x, 0f, childTransform.localScale.z);
        childTransform.gameObject.SetActive(false);
        StartCoroutine(SpawnAnimationDelay());
    }

    // Update is called once per frame
    void Update()
    {
        if (stretching)
        {
            stretchTime += SpawnManager.instance.stretchSpeed * Time.deltaTime;
            childTransform.localScale = new Vector3(childTransform.localScale.x, SpawnManager.instance.propEntryYScaleOverTime.Evaluate(stretchTime) * initialYScale, childTransform.localScale.z);
            if (stretchTime > 1)
            {
                stretching = false;
            }
        }
    }
    void StartStretching()
    {
        childTransform.gameObject.SetActive(true);
        stretching = true;
    }

    IEnumerator SpawnAnimationDelay()
    {
        yield return new WaitForSeconds(Random.Range(0f, 0.4f));
        StartStretching();
    }

}
