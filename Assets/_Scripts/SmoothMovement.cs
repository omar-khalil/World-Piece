using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMovement : MonoBehaviour
{
    public float movementSpeed;
    public AnimationCurve distanceOverTime;

    [HideInInspector]
    public Vector3 positionTarget;
    [HideInInspector]
    public Vector3 rotationTarget;

    private Vector3 startPosition;
    private Vector3 startRotation;

    private bool isMoving;
    private float movingT;
    private bool isRotating;
    private float rotatingT;

    // Start is called before the first frame update
    void Start()
    {
        isMoving = false;
        isRotating = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving)
        {
            movingT += (movementSpeed * Time.deltaTime);
            float time = distanceOverTime.Evaluate(movingT);
            transform.position = Vector3.Lerp(startPosition, positionTarget, time);
            if (movingT > 1)
            {
                isMoving = false;
            }
        }
        if (isRotating)
        {
            rotatingT += (movementSpeed * Time.deltaTime);
            float time = distanceOverTime.Evaluate(rotatingT);
            transform.localEulerAngles = Vector3.Lerp(startRotation, rotationTarget, time);
            if (rotatingT > 1)
            {
                isRotating = false;
            }
        }
    }

    public void MoveTo(Vector3 targetPosition)
    {
        startPosition = transform.position;
        positionTarget = targetPosition;
        isMoving = true;
        movingT = 0;
    }

    public void RotateTo(Vector3 targetRotation)
    {
        startRotation = transform.localEulerAngles;
        rotationTarget = targetRotation;
        isRotating = true;
        rotatingT = 0;
    }
}
