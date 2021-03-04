using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    // Simple player controller to move the player and shoot

    public GameObject destroyShot;
    public GameObject createShot;
    public Transform shotSpawn;
    [Range(0.1f,1f)]
    public float translationSpeed = 0.5f;
    [Range(1f,15f)]
    public float rotationSpeed = 10f;

    Rigidbody rb;
    Vector3 velocity;
    Vector3 rotation;
    float delay = 0f;
    AudioSource sound;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.LookAt(Vector3.zero);
        rotation = Vector3.zero;
        sound = GetComponent<AudioSource>();
    }

    void Update()
    {
        // Translation velocity is taken from the key axis inputs
        velocity = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized * translationSpeed;
        velocity = transform.TransformDirection(velocity);

        // Rotation angle is taken from the mouse
        rotation += new Vector3(-Input.GetAxisRaw("Mouse Y"), Input.GetAxisRaw("Mouse X"), 0)* rotationSpeed;

        // The shots are fired with mouse clicks
        // To allow the user to hold down the mouse button, a new shot is only fired after a delay
        // For each shot: instantiate the right shot object, give it some speed, unparent it, reset the delay variable and play the shot sound
        delay += Time.deltaTime;
        if (Input.GetMouseButton(0) && delay>=0.15f)
        {
            GameObject instance = Instantiate(destroyShot, shotSpawn);
            instance.GetComponent<Rigidbody>().velocity = shotSpawn.TransformDirection(new Vector3(0f, 0f, 10f));
            instance.transform.parent = null;
            delay = 0f;
            if(sound != null) sound.Play();
        }

        if (Input.GetMouseButton(1) && delay >= 0.15f)
        {
            GameObject instance = Instantiate(createShot, shotSpawn);
            instance.GetComponent<Rigidbody>().velocity = shotSpawn.TransformDirection(new Vector3(0f, 0f, 10f));
            instance.transform.parent = null;
            delay = 0f;
            if (sound != null) sound.Play();
        }
    }

    void FixedUpdate()
    {
        // The player movement is done in FixedUpdate: translation and rotation
        transform.position += velocity;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z);
    }
}
