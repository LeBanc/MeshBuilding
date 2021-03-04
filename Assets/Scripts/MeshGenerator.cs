using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    public float squareSize = 1f; // Distance between 2 nodes
    public MeshFilter objectMesh; // Empty mesh to create

    [HideInInspector]
    public List<Vector3> vertices = new List<Vector3>(); // list of vertices for the mesh
    [HideInInspector]
    public List<int> triangles = new List<int>(); // list of triangles for the mesh
    [HideInInspector]
    public List<ControlNode> controlNodes = new List<ControlNode>(); // list of all control nodes

    [HideInInspector]
    public Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>(); // dictionnary to link  nodes and triangles
    [HideInInspector]
    public List<CubeGrid> cubes = new List<CubeGrid>(); // list of small cubes making the whole object

    // public function to create all the nodes and the cube grid
    public virtual void CreateNodes(int[,,] obj)
    {
        // specific for Inside and Outside configurations
    }

    // function to make the triangles of the mesh from the list of cubes in the grid
    public virtual void MakeTriangles()
    {
        triangleDictionary.Clear();
        vertices.Clear();
        triangles.Clear();

        // Reset the node index
        foreach (ControlNode node in controlNodes)
        {
            node.vertexIndex = -1;
        }

        // for each cube in the grid, create triangles from the cube value (depending on active nodes)
        CreateTrianglesInCube();

        // Create a new mesh and pass it to the mesh object
        Mesh mesh = new Mesh();
        objectMesh.mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        objectMesh.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public virtual void CreateTrianglesInCube()
    {
        // specific for Inside and Outside configurations
    }

    // Function to create a triangle from 3 nodes (visibility is clockwise)
    public virtual void CreateTriangle(ControlNode a, ControlNode b, ControlNode c)
    {
        // If a vertex is not already used, add it to the list of vertex with the index of the last position +1 in the list
        if (a.vertexIndex == -1) // Comment to use the vertex as a new vertex to sharpen the angles else, the same vertex is use multiple times and the normal is the sum of all normals.
        {
            a.vertexIndex = vertices.Count;
            vertices.Add(a.position);
        }
        if (b.vertexIndex == -1) // Comment to use the vertex as a new vertex to sharpen the angles else, the same vertex is use multiple times and the normal is the sum of all normals.
        {
            b.vertexIndex = vertices.Count;
            vertices.Add(b.position);
        }
        if (c.vertexIndex == -1) // Comment to use the vertex as a new vertex to sharpen the angles else, the same vertex is use multiple times and the normal is the sum of all normals.
        {
            c.vertexIndex = vertices.Count;
            vertices.Add(c.position);
        }

        // The list of triangles is the list of the 3 vertices of each triangle
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        // Create a new triangle to link it to the vertices in the dictionnary
        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }

    // Function to add the triangle to the vertex in the dictionnary
    public virtual void AddTriangleToDictionary(int vertexIndexKey, Triangle triangle)
    {
        if (triangleDictionary.ContainsKey(vertexIndexKey))
        {
            triangleDictionary[vertexIndexKey].Add(triangle);
        }
        else
        {
            List<Triangle> triangleList = new List<Triangle>();
            triangleList.Add(triangle);
            triangleDictionary.Add(vertexIndexKey, triangleList);
        }
    }

    // Class to define a cube grid
    public class CubeGrid
    {
        // A cube is defined with 8 nodes
        public ControlNode frontUpLeft, frontUpRight, frontLowRight, frontLowLeft, backUpLeft, backUpRight, backLowRight, backLowLeft;
        public bool active; // A cube is active if one of its vertex is
        public int value; // The value of the cube defined its shape from active vertices

        // Function to get the value of the cube from the active vertices (each vertex has a power of 2 value)
        public void GetValue()
        {
            value = 0;
            if (frontUpLeft.active) value += 128;
            if (frontUpRight.active) value += 64;
            if (frontLowRight.active) value += 32;
            if (frontLowLeft.active) value += 16;
            if (backUpLeft.active) value += 8;
            if (backUpRight.active) value += 4;
            if (backLowRight.active) value += 2;
            if (backLowLeft.active) value += 1;
        }

        // Function to clear all the vertices indices of the cube, it allows to sharpen the angles of the mesh
        public void ClearVerticesIndex()
        {
            frontUpLeft.vertexIndex = -1;
            frontUpRight.vertexIndex = -1;
            frontLowRight.vertexIndex = -1;
            frontLowLeft.vertexIndex = -1;
            backUpLeft.vertexIndex = -1;
            backUpRight.vertexIndex = -1;
            backLowRight.vertexIndex = -1;
            backLowLeft.vertexIndex = -1;
        }

        // CubeGrid creator from the 8 vertices
        public CubeGrid(ControlNode fll, ControlNode flr, ControlNode fur, ControlNode ful, ControlNode bll, ControlNode blr, ControlNode bur, ControlNode bul)
        {
            frontUpLeft = ful;
            frontUpRight = fur;
            frontLowRight = flr;
            frontLowLeft = fll;
            backUpLeft = bul;
            backUpRight = bur;
            backLowRight = blr;
            backLowLeft = bll;

            GetValue();
            active = value == 0 ? false : true;
        }

        // CubeGrid creator from the FrontLowLeft vertex, the list of all nodes and the y and z sizes of the grid
        public CubeGrid(ControlNode fll, List<ControlNode> allNodes, int sizeY, int sizeZ)
        {
            frontLowLeft = fll;
            int index = allNodes.IndexOf(frontLowLeft);

            // Get the other vertices from the index of the FrontLowLeft vertex of the cube
            frontLowRight = allNodes[index + sizeZ * sizeY];
            frontUpRight = allNodes[index + sizeZ * sizeY + sizeZ];
            frontUpLeft = allNodes[index + sizeZ];
            backLowLeft = allNodes[index + 1];
            backLowRight = allNodes[index + sizeZ * sizeY + 1];
            backUpRight = allNodes[index + sizeZ * sizeY + sizeZ + 1];
            backUpLeft = allNodes[index + sizeZ + 1];

            GetValue();
            active = value == 0 ? false : true;
        }
    }

    // Class to define a ControlNode
    public class ControlNode
    {
        // A ControlNode is at a 3D position, is active or not and has a vertexIndex different from -1 if used in the vertices list
        // It also has neighbours: other control nodes at a distance of a single 'squaresize"
        public bool active;
        public Vector3 position;
        public int vertexIndex = -1;
        public List<ControlNode> neighbours;

        // Creator of ControlNode from a position and a state
        public ControlNode(Vector3 _pos, bool _active)
        {
            position = _pos;
            active = _active;
            neighbours = new List<ControlNode>();
        }

        // Function to get the neighbour nodes with the squareSize
        public void GetNeighbours(List<ControlNode> allNodes, float squareSize)
        {
            neighbours = new List<ControlNode>();
            // For each node in the global list, check if the distance from the selected node is less than a bit more of a 'squareSize'
            foreach (ControlNode node in allNodes)
            {
                float distance = (position - node.position).magnitude;
                if (distance <= 1.3 * squareSize + float.Epsilon && distance > float.Epsilon) // 1.3 because gt. than 1 and less than sqrt(2)
                {
                    neighbours.Add(node);
                }
                if (neighbours.Count >= 6) break; // if 6 neighbours are found, there won't be another and we can quit the loop
            }
        }
    }

    // Structure to define a triangle
    public struct Triangle
    {
        // A triangle is made of 3 vertices (and the link to their indices in the vertices list)
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        int[] vertices;

        // Creator of a tringle from 3 vertices
        public Triangle(int a, int b, int c)
        {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;

            vertices = new int[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
        }

        // Public function to get a vertex of a triangle from its index in the triangle vertices array
        public int this[int i]
        {
            get
            {
                return vertices[i];
            }
        }

        // Public function to know if a vertex is part of a triangle
        public bool Contains(int vertexIndex)
        {
            return vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC;
        }
    }
}
