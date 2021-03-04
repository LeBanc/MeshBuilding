using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotator : MonoBehaviour
{
    public float maxSpeed = 1f;

    float xSpeed;
    float ySpeed;
    float zSpeed;

    // Start is called before the first frame update
    void Start()
    {
        xSpeed = Random.Range(0, maxSpeed);
        ySpeed = Random.Range(0, maxSpeed);
        zSpeed = Random.Range(0, maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(xSpeed, ySpeed, zSpeed));
    }
}
