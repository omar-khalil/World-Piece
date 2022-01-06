using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autorotate : MonoBehaviour
{
    public float rotationSpeed;
    
    // Update is called once per frame
    void Update()
    {
        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + (rotationSpeed * Time.deltaTime), transform.localEulerAngles.z);     
    }
}
