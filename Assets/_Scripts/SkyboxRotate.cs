using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotate : MonoBehaviour
{

    public float speedMultiplier;

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * speedMultiplier);
    }
}
