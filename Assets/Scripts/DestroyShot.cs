using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyShot : MonoBehaviour
{
    public GameObject destroySound;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MeshObject"))
        {
            Vector3 contact = collision.GetContact(0).point;

            if (GameObject.FindGameObjectWithTag("GameController").GetComponent<BackgroundGenerator>().inside)
            {
                // If configuration is "inside"
                MeshGeneratorInside meshGenIn = GameObject.FindGameObjectWithTag("GameController").GetComponent<MeshGeneratorInside>();
                Vector3 force = -collision.relativeVelocity; // to destroy matter substract the velocity
                // Destroy and create a new node from the impact force
                meshGenIn.ImpactEffect(contact, force);
            }
            else
            {
                // If configuration is "outside"
                MeshGeneratorOutside meshGenOut = GameObject.FindGameObjectWithTag("GameController").GetComponent<MeshGeneratorOutside>();
                Vector3 force = -collision.relativeVelocity.normalized; // to destroy matter substract the velocity
                // Deactivate the nearest existing node from the impact direction and activate its neighbours
                meshGenOut.ImpactRemoveEffect(contact, force);
            }

            // Play the "shot" sound and destroy it after 1 second (it is an instantiation to delete this object and be able to play the sound)
            GameObject sound = Instantiate(destroySound, contact, Quaternion.identity);
            sound.GetComponent<AudioSource>().Play();
            Destroy(sound, 1f);

            // Delete this shot
            Destroy(this.gameObject);
        }
    }
}
