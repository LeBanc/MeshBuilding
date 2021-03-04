using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundGenerator : MonoBehaviour
{
    // Background generator to generate a list of 3D points: the object is abstract, the physical size will be set in the mesh generator components

    public int size = 2;
    public bool inside = false;

    private List<Vector3> points;
    private int[,,] obj;

    void Start()
    {
        if (inside)
        {
            // Configuration is "inside"
            GenerateObject(); // Generate the object (a cube of "size" length)
            MeshGeneratorInside meshGenIn = GetComponent<MeshGeneratorInside>();
            meshGenIn.CreateNodes(obj); // Pass it to the MeshGeneratorInside for inside configuration
        }
        else
        {
            // Configuration is "outside"
            GenerateObject(); // Generate the object (a cube of "size" length)
            MeshGeneratorOutside meshGenOut = GetComponent<MeshGeneratorOutside>();
            meshGenOut.CreateNodes(obj); // Pass it to the MeshGeneratorOutside for outside configuration
        }
    }

    void GenerateObject()
    {
        // The generated object is a cube of "size" length but only the external points are set as active (1), the other are set to (0)
        // This allow to generate an empty romm for "inside" configuration and to limit the number of polygons in "outside" configuration
        obj = new int[size, size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                for (int z = 0; z < size; z++)
                {
                    if( x*y*z==0 || x == (size -1 ) || y == (size - 1) || z == (size - 1))
                    {
                        obj[x, y, z] = 1;
                    }
                    else
                    {
                        obj[x, y, z] = 0;
                    }
                }
            }
        }
    }

}
