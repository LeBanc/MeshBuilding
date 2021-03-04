using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneratorOutside : MeshGenerator
{
    private List<ControlNode> activeNodes = new List<ControlNode>(); // list of active control nodes
    private List<ControlNode> activableNodes = new List<ControlNode>(); // list of activable control nodes (is not active nor deactive and can be activated)
    private List<ControlNode> deactivatedNodes = new List<ControlNode>(); // list of deactivated control nodes (deactivated: was active and has been deactivate)

    [HideInInspector] // for debug: comment the HideInInspector for the debug scene to modify the shape of the mesh (single cube size)
    public bool bll,blr,bur,bul,fll,flr,fur,ful = true;

    // public function to create all the nodes and the cube grid
    public override void CreateNodes(int[,,] obj)
    {
        // Clear all lists of nodes and cubes
        controlNodes.Clear();
        activeNodes.Clear();
        activableNodes.Clear();
        deactivatedNodes.Clear();
        cubes.Clear();

        // Offsets to center the object around its (0,0,0) position
        float xOffset = -(float)(obj.GetLength(0) - 1)/2;
        float yOffset = -(float)(obj.GetLength(1) - 1)/2;
        float zOffset = -(float)(obj.GetLength(2) - 1)/2;

        // Creation of all the nodes of the object and the node list
        for (int x=0; x < obj.GetLength(0); x++)
        {
            for (int y = 0; y < obj.GetLength(1); y++)
            {
                for (int z = 0; z < obj.GetLength(2); z++)
                {
                    Vector3 pos = new Vector3((xOffset + x) * squareSize, (yOffset + y) * squareSize, (zOffset + z) * squareSize);
                    bool active = obj[x, y, z] == 1;
                    ControlNode node = new ControlNode(pos, active);
                    controlNodes.Add(node);

                    // Add the node to the right list: active if it is active, activable otherwise (at first, all the unactive nodes are activables)
                    if (node.active)
                    {
                        activeNodes.Add(node);
                    }
                    else
                    {
                        activableNodes.Add(node);
                    }
                }
            }
        }

        // Link all nodes to their neighbours
        foreach (ControlNode node in controlNodes)
        {
            node.GetNeighbours(controlNodes, squareSize);
        }

        // Creation of all the cubes in the grid from the node list
        for (int x = 0; x < obj.GetLength(0)-1; x++)
        {
            for (int y = 0; y < obj.GetLength(1)-1; y++)
            {
                for (int z = 0; z < obj.GetLength(2)-1; z++)
                {
                    CubeGrid cubeG = new CubeGrid(controlNodes[z + obj.GetLength(2) * y + obj.GetLength(1) * obj.GetLength(1) * x], controlNodes, obj.GetLength(1), obj.GetLength(2));
                    cubes.Add(cubeG);
                }
            }
        }

        // Call to create the triangles of the mesh
        MakeTriangles();
    }

    public override void CreateTrianglesInCube()
    {
        // for each cube in the grid, create triangles from the cube value (depending on active nodes)
        foreach (CubeGrid cubeG in cubes)
        {
            cubeG.GetValue();
            switch (cubeG.value)
            {
                // Cases with 0, 1 or 2 active points: cannot draw a triangle so nothing is done
                case 0:
                case 1:
                case 2:
                case 4:
                case 8:
                case 16:
                case 32:
                case 64:
                case 128:
                case 3:
                case 5:
                case 6:
                case 9:
                case 10:
                case 12:
                case 17:
                case 18:
                case 20:
                case 24:
                case 33:
                case 34:
                case 36:
                case 40:
                case 48:
                case 65:
                case 66:
                case 68:
                case 72:
                case 80:
                case 96:
                case 129:
                case 130:
                case 132:
                case 136:
                case 144:
                case 160:
                case 192:
                    break;
                // Cases with 3 active points not drawn in "outside" configuration beacause it only creates volume meshes
                // blr & bll cases
                case 7: //bur,blr,bll
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 11: //bul,blr,bll
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 19: //fll,blr,bll
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 35: //flr,blr,bll
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 67: //fur,blr,bll
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 131: //ful,blr,bll
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // bur & bll cases
                case 13: //bul,bur,bll
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 21: //fll,bur,bll
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 37: //flr,bur,bll
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 69: //fur,bur,bll
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 133: //ful,bur,bll
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // bur & blr cases
                case 14: //bul,bur,blr
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 22: //fll,bur,blr
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 38: //flr,bur,blr
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 70: //fur,bur,blr
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 134: //ful,bur,blr
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // bul & bll cases
                case 25: //fll,bul,bll
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 41: //flr,bul,bll
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 73: //fur,bul,bll
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 137: //ful,bul,bll
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // bul & blr cases
                case 26: //fll,bul,blr
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 42: //flr,bul,blr
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 74: //fur,bul,blr
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 138: //ful,bul,blr
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // bul & bur cases
                case 28: //fll,bul,bur
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 44: //flr,bul,bur
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 76: //fur,bul,bur
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 140: //ful,bul,bur
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // flr & fll cases
                case 49: //flr,fll,bll
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 50: //flr,fll,blr
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 52: //flr,fll,bur
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 56: //flr,fll,bul
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // fur & fll cases
                case 81: //fur,fll,bll
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 82: //fur,fll,blr
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 84: //fur,fll,bur
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 88: //fur,fll,bul
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // fur & flr cases
                case 97: //fur,flr,bll
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 98: //fur,flr,blr
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 100: //fur,flr,bur
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 104: //fur,flr,bul
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 112: //fur,flr,fll
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // ful & fll cases
                case 145: //ful,fll,bll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 146: //ful,fll,blr
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 148: //ful,fll,bur
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 152: //ful,fll,bul
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // ful & flr cases
                case 161: //ful,flr,bll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 162: //ful,flr,blr
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 164: //ful,flr,bur
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 168: //ful,flr,bul
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 176: //ful,flr,fll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // ful & fur cases
                case 193: //ful,fur,bll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 194: //ful,fur,blr
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 196: //ful,fur,bur
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 200: //ful,fur,bul
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 208: //ful,fur,fll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 224: //ful,fur,flr
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;

                // Cases with 4 active points
                case 15: //Back face (both sides)
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.backLowLeft);
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 23: //blr,bur,bll,fll
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 27: //blr,bul,bll,fll
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 29: //bur,bul,bll,fll
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 30: //bur,bul,blr,fll
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 39://blr,bur,bll,flr
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 43://blr,bul,bll,flr
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 45://bur,bul,bll,flr
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 46: //bur,bul,blr,flr
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 51: //Low face (both sides)
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 53: //flr,fll,bll,bur
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 54: //flr,fll,blr,bur
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 57: //flr,fll,bll,bul
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 58: //flr,fll,blr,bul
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 60: // flr,fll,bur,bul - diagonale plan (both faces)
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpRight, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 71: // fur,bur,blr,bll
                    // Triangle fur,bur,blr
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 75: // fur,bul,bll,blr
                    // Triangle fur,bul,bll
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 77: // fur,bul,bll,bur
                    // Triangle fur,bul,bll
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 78: // fur,bur,blr,bul
                    // Triangle fur,bur,blr
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bul
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 83: // fll,blr,bll,fur
                    // Triangle fll,blr,bll
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from fur
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 85: // fur,fll,bur,bll - diagonale plan (both faces)
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 86: // fur,bur,blr,fll
                    // Triangle fur,bur,blr
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from fll
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 89: // fur,fll,bul,bll
                    // Triangle fur,fll,bul
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 90: // fur,fll,bul,blr
                    // Triangle fur,fll,bul
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 92: // fur,fll,bul,bur
                    // Triangle fur,fll,bul
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 99: // flr,blr,bll,fur
                    // Triangle flr,blr,bll
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from fur
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 101: // fur,flr,bur,bll
                    // Triangle fur,flr,bur
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 102: // Right face (both sides)
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpRight, cubeG.frontUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 105: // fur,flr,bul,bll  - diagonale plan (both faces)
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.backLowLeft);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 106: // fur,flr,bul,blr
                    // Triangle fur,flr,bul
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 108: // fur,flr,bul,bur
                    // Triangle fur,flr,bul
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 113: // fur,flr,fll,bll
                    // Triangle fur,flr,fll
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 114: // fur,flr,fll,blr
                    // Triangle fur,flr,fll
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 116: // fur,flr,fll,bur
                    // Triangle fur,flr,fll
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 120: // fur,flr,fll,bul
                    // Triangle fur,flr,fll
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bul
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 135: // ful,blr,bll,bur
                    // Triangle ful,blr,bll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 139: // ful,blr,bll,bul
                    // Triangle ful,blr,bll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bul
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 141: // ful,bul,bur,bll
                    // Triangle ful,bul,bur
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 142: // ful,bul,bur,blr
                    // Triangle ful,bul,bur
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 147: // ful,fll,bll,blr
                    // Triangle ful,fll,bll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 149: // ful,fll,bll,bur
                    // Triangle ful,fll,bll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 150: // ful,fll,bur,blr - diagonale plan (both faces)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 153: //Left face (both sides)
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 154: // ful,fll,bul,blr
                    // Triangle ful,fll,bul
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 156: // ful,fll,bul,bur
                    // Triangle ful,fll,bul
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 163: // flr,blr,bll,ful
                    // Triangle flr,blr,bll
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from ful
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 165: // ful,flr,bur,bll
                    // Triangle ful,flr,bur
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 166: // ful,flr,bur,blr
                    // Triangle ful,flr,bur
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 169: // ful,flr,bul,bll
                    // Triangle ful,flr,bul
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 170: // ful,flr,bul,blr - diagonale plan (both faces)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 172: // ful,flr,bul,bur
                    // Triangle ful,flr,bul
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 177: // ful,flr,fll,bll
                    // Triangle ful,flr,fll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 178: // ful,flr,fll,blr
                    // Triangle ful,flr,fll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 180: // ful,flr,fll,bur
                    // Triangle ful,flr,fll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 184: // ful,flr,fll,bul
                    // Triangle ful,flr,fll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bul
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 195: // fur,ful,blr,bll - diagonale plan (both faces)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 197: // ful,fur,bur,bll
                    // Triangle ful,fur,bur
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 198: // ful,fur,bur,blr
                    // Triangle ful,fur,bur
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 201: // ful,fur,bul,bll
                    // Triangle ful,fur,bul
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 202: // ful,fur,bul,blr
                    // Triangle ful,fur,bul
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 204: //Up face (both sides)
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.frontUpRight);
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 209: // ful,fur,fll,bll
                    // Triangle ful,fur,fll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 210: // ful,fur,fll,blr
                    // Triangle ful,fur,fll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 212: // ful,fur,fll,bur
                    // Triangle ful,fur,fll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 216: // ful,fur,fll,bul
                    // Triangle ful,fur,fll
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bul
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 225: // ful,fur,flr,bll
                    // Triangle ful,fur,flr
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bll
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 226: // ful,fur,flr,blr
                    // Triangle ful,fur,flr
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from blr
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 228: // ful,fur,flr,bur
                    // Triangle ful,fur,flr
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bur
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 232: // ful,fur,flr,bul
                    // Triangle ful,fur,flr
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from bul
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 240: //Front face (both sides)
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;

                // Cases with 5 active points
                case 31: // Back face and FrontLowLeft vertex
                    // Back face
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowLeft point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 47: // Back face and FrontLowRight vertex
                    // Back face
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowRight point
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 55: // Low face and BackUpRight vertex
                    // Low face
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpRight point
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 59: // Low face and BackUpLeft vertex
                    // Low face
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpLeft point
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 61: // Front-Low to Back-Up face and BackLowLeft vertex
                    // Front-Low to Back-Up face (facing front)
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowLeft point
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 62: // Front-Low to Back-Up face and BackLowRight vertex
                    // Front-Low to Back-Up face (facing front)
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.backUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowRight point
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 79: // Back face and FrontUpRight vertex
                    // Back face
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpRight point
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 87: // Low-Left to Up-Right face and BackLowRight vertex
                    // Low-Left to Up-Right face (facing left)
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowRight point
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 91: // Hexaedron (non regular triangular bipyramid)
                    // Triangles from BackLowLeft vertex (along the edge of the squareCube)
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpRight vertex (diagonale of the face of the squareCube)
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 93: // Low-Left to Up-Right face and BackUpLeft vertex
                    // Low-Left to Up-Right face (facing right)
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpLeft point
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 94: // Hexaedron (non regular triangular bipyramid)
                    // Triangles from backUpRight vertex (along the edge of the squareCube)
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowLeft vertex (diagonale of the face of the squareCube)
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 103: // Right face and BackLowLeft vertex
                    // Right face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowLeft point
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 107: // Back-Left to Front-Right face and BackLowRight vertex
                    // Back-Left to Front-Right face (facing left)
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowRight point
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 109: // Back-Left to Front-Right face and BackUpRight vertex
                    // Back-Left to Front-Right face (facing left)
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpRight point
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 110: // Right face and BackUpLeft vertex
                    // Right face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpLeft point
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 115: // Low face and FrontUpRight vertex
                    // Low face
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpRight point
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 117: // Low-Left to Up-Right face and FrontLowRight vertex
                    // Low-Left to Up-Right face (facing left)
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowRight point
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 118: // Right face and FrontLowLeft vertex
                    // Right face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowLeft point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 121: // Back-Left to Front-Right face and FrontLowLeft vertex
                    // Back-Left to Front-Right face (facing right)
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowLeft point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 122: // Hexaedron (non regular triangular bipyramid)
                    // Triangles from FrontLowRight vertex (along the edge of the squareCube)
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpLeft vertex (diagonale of the face of the squareCube)
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 124: // Front-Low to Back-Up face and FrontUpRight vertex
                    // Front-Low to Back-Up face (facing back)
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpRight point
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 143: // Back face and FrontUpLeft vertex
                    // Back face
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpLeft point
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 151: // Front-Left to Back-Right face and BackLowLeft vertex
                    // Front-Left to Back-Right face (facing right)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowLeft point
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 155: // Left face and BackLowRight vertex
                    // Left face
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowRight point
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 157: // Left face and BackUpRight vertex
                    // Left face
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpRight point
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 158: // Front-Left to Back-Right face and BackUpLeft vertex
                    // Front-Left to Back-Right face (facing right)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpLeft point
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 167: // Hexaedron (non regular triangular bipyramid)
                    // Triangles from BackLowRight vertex (along the edge of the squareCube)
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpLeft vertex (diagonale of the face of the squareCube)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 171: // Up-Left to Low-Right face and BackLowLeft vertex
                    // Up-Left to Low-Right face (facing right)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowLeft point
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 173: // Hexaedron (non regular triangular bipyramid)
                    // Triangles from BackUpLeft vertex (along the edge of the squareCube)
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowRight vertex (diagonale of the face of the squareCube)
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 174: // Up-Left to Low-Right face and BackUpRight vertex
                    // Up-Left to Low-Right face (facing left)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpRight point
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 179: // Low face and FrontUpLeft vertex
                    // Low face
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpLeft point
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 181: // Hexaedron (non regular triangular bipyramid)
                    // Triangles from frontLowLeft vertex (along the edge of the squareCube)
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpRight vertex (diagonale of the face of the squareCube)
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 182: // Front-Left to Back-Right face and FrontLowRight vertex
                    // Front-Left to Back-Right face (facing left)
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowRight point
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 185: // Left face and FrontLowRight vertex
                    // Left face
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowRight point
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 186: // Up-Left to Low-Right face and FrontLowLeft vertex
                    // Up-Left to Low-Right face (facing right)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowLeft point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 188: // Front-Low to Back-Up face and FrontUpLeft vertex
                    // Front-Low to Back-Up face (facing back)
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpLeft point
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 199: // Front-Up to Back-Low face and BackUpRight vertex
                    // Front-Up to Back-Low face (facing down)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpRight point
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 203: // Front-Up to Back-Low face and BackUpLeft vertex
                    // Front-Up to Back-Low face (facing down)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpLeft point
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 205: // Up face and BackLowLeft vertex
                    // Up face
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowLeft point
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 206: // Up face and BackLowRight vertex
                    // Up face
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowRight point
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 211: // Front-Up to Back-Low face and FrontLowLeft vertex
                    // Front-Up to Back-Low face (facing up)
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowLeft point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 213: // Low-Left to Up-Right face and FrontUpLeft vertex
                    // Low-Left to Up-Right face (facing right)
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpLeft point
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 214: // Front-Left to Back-Right face and FrontUpRight vertex
                    // Front-Left to Back-Right face (facing left)
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpRight point
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 217: // Left face and FrontUpRight vertex
                    // Left face
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpRight point
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 218: // Hexaedron (non regular triangular bipyramid)
                    // Triangles from FrontUpLeft vertex (along the edge of the squareCube)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowRight vertex (diagonale of the face of the squareCube)
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 220: // Up face and FrontLowLeft vertex
                    // Up face
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowLeft point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 227: // Front-Up to Back-Low face and FrontLowRight vertex
                    // Front-Up to Back-Low face (facing up)
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowRight point
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 229: // Hexaedron (non regular triangular bipyramid)
                    // Triangles from FrontUpRight vertex (along the edge of the squareCube)
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowLeft vertex (diagonale of the face of the squareCube)
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 230: // Right face and FrontUpLeft vertex
                    // Right face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpLeft point
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 233: // Back-Left to Front-Right face and FrontUpLeft vertex
                    // Back-Left to Front-Right face (facing right)
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpLeft point
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 234: // Up-Left to Low-Right face and FrontUpRight vertex
                    // Up-Left to Low-Right face (facing left)
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpRight point
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 236: // Up face and FrontLowRight vertex
                    // Up face
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontLowRight point
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 241: // Front face and BackLowLeft vertex
                    // Front face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowLeft point
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 242: // Front face and BackLowRight vertex
                    // Front face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackLowRight point
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 244: // Front face and BackUpRight vertex
                    // Front face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpRight point
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 248: // Front face and BackUpLeft vertex
                    // Front face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpLeft point
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;

                // Cases with 6 active points
                case 63: // Back & Low faces
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 95: // Back face & front diagonale from low left to up right
                    // Back face
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // triangles from Back face to FrontUpRight point
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Back face to FrontLowLeft point
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Front LowLeft to UpRight points
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 111: //Back & Right faces
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 119: //Low & Right faces
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 123: // Low Face & up diagonale from back left to front right
                    // Low face
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // triangles from Low face to FrontUpRight point
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Low face to BackUpLeft point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Up BackLeft to FrontRight points
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 125: // Octahedron with BackLeft to FrontRight base and FrontLowLeft & BackUpRight vertices
                    // Triangles from FrontLowLeft point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpRight point
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 126: // Right face & left diagonale from back up to front low
                    // Right face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // triangles from Right face to FrontLowLeft point
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Right face to BackUpLeft point
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Left BackUp to FrontLow points
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 159: //Back & Left faces
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 175: // Back face & front diagonale from up left to low right
                    // Back face
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // triangles from Back face to FrontLowRight point
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Back face to FrontUpLeft point
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Front UpLeft to LowRight points
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 183: // Low Face & up diagonale from front left to back right
                    // Low face
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Low face to FrontUpLeft point
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Low face to BackUpRight point
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Up FrontLeft to BackRight points
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 187: //Left & Low faces
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 189: // Left Face & right diagonale from back up to front low
                    // Left Face
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Left face to FrontLowRight point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Left face to BackUpRight point
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Right BackUp to FrontLow points
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 190: // Octahedron with FrontLeft to BackRight base and FrontLowRight & BackUpLeft vertices
                    // Triangles from FrontLowRight point
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpLeft point
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 207: //Up & Back faces
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backLowLeft);
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 215: // Octahedron with FrontUp to BackLow base and FrontLowLeft & BackUpRight vertices
                    // Triangles from FrontLowLeft point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from BackUpRight point
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 219: // Left Face & right diagonale from front up to back low
                    // Left Face
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Left face to FrontUpRight point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Left face to BackLowRight point
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Right FrontUp to BackLow points
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 221: //Up & Left faces
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 222: // Octahedron with FrontLeft to BackRight base and BackUpLeft & FrontUpRight vertices
                    // Triangles from BackUpLeft point
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpRight point
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 231: // Right face & left diagonale from back low to front up
                    // Right face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // triangles from Right face to FrontUpLeft point
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Right face to BackUpLeft point
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Left BackLow to FrontUp points
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 235: // Octahedron with UpLeft to LowRight base and BackLowLeft & FrontUpRight vertices
                    // Triangles from BackLowLeft point
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from FrontUpRight point
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 237: // Up Face & low diagonale from back left to front right
                    // Up Face
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Up face to BackLowLeft point
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Up face to FrontLowRight point
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Low BackLeft to FrontRight points
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 238: //Up & Right faces
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 243: //Front & Low faces
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 245: // Front Face & back diagonale from low left to up right
                    // Front Face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Front face to BackLowLeft point
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Up face to BackUpRight point
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Back LowLeft to UpRight points
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 246: //Front & Right faces
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 249: //Front & Left faces
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 250: // Front Face & back diagonale from up left to low right
                    // Front Face
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Front face to BackUpLeft point
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Up face to BackLowRight point
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    // Triangles from Back LowLeft to UpRight points
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 252: //Front & Up faces
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // add triangles between faces
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // add the diagonale face between them
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // Cases with 7 active points
                case 127: //Back, Low & Right faces
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // triangles between them
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    // and a last triangle to close the shape
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 191: //Back, Low & Left faces
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // triangles between them
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // and a last triangle to close the shape
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 223: //Back, Up & Left faces
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // triangles between them
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    // and a last triangle to close the shape
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 239: //Back, Up & Right faces
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // triangles between them
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontLowRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    // and a last triangle to close the shape
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 247: //Front, Low & Right faces
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // triangles between them
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.frontUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // and a last triangle to close the shape
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 251: //Front, Low & Left faces
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // triangles between them
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    // and a last triangle to close the shape
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 253: //Front, Up & Left faces
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    // triangles between them
                    CreateTriangle(cubeG.backUpLeft, cubeG.backLowLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    // and a last triangle to close the shape
                    CreateTriangle(cubeG.frontLowRight, cubeG.backUpRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 254: //Front, Up & Right faces
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    // triangles between them
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    // and a last triangle to close the shape
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // Case with 8 active points
                case 255: //All faces: Back, Low, Right, Left, Up, Front
                    CreateTriangle(cubeG.backUpRight, cubeG.backLowLeft, cubeG.backLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.backUpLeft, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowLeft, cubeG.frontLowLeft);
                    CreateTriangle(cubeG.frontLowRight, cubeG.backLowRight, cubeG.backLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.backUpLeft, cubeG.backUpRight, cubeG.frontUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
            }
        }
    }

    // Function to find the nearest node from a position, in a dedicated list of nodes
    ControlNode GetNearestNode(Vector3 position, List<ControlNode> nodeList)
    {
        ControlNode nearestNode = new ControlNode(position, true);
        float distance = 2f * squareSize; // distance is initialize to 2 * squareSize to only find a node close to the position

        foreach (ControlNode controlNode in nodeList)
        {
            if ((position - controlNode.position).magnitude < distance)
            {
                distance = (position - controlNode.position).magnitude;
                nearestNode = controlNode;
            }
        }

        // If the distance has not changed, no node is found
        if(distance == 2f * squareSize)
        {
            return null;
        }
        else
        {
            return nearestNode;
        }
    }

    // Function to add the effect of an impact (position, force) to the mesh when we want to detroy the mesh 
    public void ImpactRemoveEffect(Vector3 position, Vector3 force)
    {
        // Get the nearest active node from the impact position
        ControlNode node = GetNearestNode(position, activeNodes);
        // Decativate it, remove it from the active list and add it to the deactivated list
        node.active = false;
        activeNodes.Remove(node);
        deactivatedNodes.Add(node);
        // The activate its (activable) neighbours to close the shape of the mesh
        ActiveNeighbourNodes(node);

        // Call MakeTringles to recreate the vertices and triangles lists and update the mesh
        MakeTriangles();
    }

    // Function to add the effect of an impact (position, force) to the mesh when we want to build the mesh 
    public void ImpactAddEffect(Vector3 position, Vector3 force)
    {
        // Get the nearest deactivated node from the impact position + a small amount of the direction from where it was shot
        ControlNode newNode = GetNearestNode(position + force.normalized * 2 / 3 * squareSize, deactivatedNodes);

        // If no node was found in the deactivated list: do nothing
        if (newNode == null)
        {
            return;
        }

        // If a node was found, activate it, add it to the active nodes list and remove it from the deactivated nodes list
        newNode.active = true;
        activeNodes.Add(newNode);
        deactivatedNodes.Remove(newNode);
        
        // Switch its neighbours from active to activable if needed (active bool of the node and add and remove from the right lists)
        // A neighbour has to be active and meet some conditions to be switched to activable (see CheckActivable function)
        foreach (ControlNode neighbour in newNode.neighbours)
        {
            if (neighbour.active)
            {
                if (CheckActivable(neighbour))
                {
                    neighbour.active = false;
                    activeNodes.Remove(neighbour);
                    activableNodes.Add(neighbour);
                }
            }
        }

        // Call MakeTringles to recreate the vertices and triangles lists and update the mesh
        MakeTriangles();
    }

    // Function to activate the neigbours of a node
    void ActiveNeighbourNodes(ControlNode node)
    {
        // For each neighbour, activate it if it is activable (activable means not active and not deactivated)
        foreach (ControlNode neighbour in node.neighbours)
        {
            // Check if the neighbour is in the activable nodes list and activate it if it is (active bool of the node and add and remove from the right lists)
            if (activableNodes.Contains(neighbour))
            {
                neighbour.active = true;
                activeNodes.Add(neighbour);
                activableNodes.Remove(neighbour);
            }
        }
    }

    // Function to check if a node can be switch from active to activable
    // This is used to reduce the number of triangles when a mesh is rebuilt from a deactivated node:
    // All the neigbours were activated at the destruction and some might be not useful
    // A node can be switch from active to activables if it is surrounded by active or activable neigbours (ie: not on an edge of the mesh)
    // Note: Nodes at the border of the object can't be switched because they don't have 6 neighbours
    bool CheckActivable(ControlNode node)
    {
        int count = 0;
        // For each active or activable neighbour, add 1 to count
        foreach(ControlNode neighbour in node.neighbours)
        {
            if (deactivatedNodes.Contains(neighbour)) return false; // fast exit if a neighbour is deactivated (the count will never be 6)
            if (activeNodes.Contains(neighbour)) count++;
            if (activableNodes.Contains(neighbour)) count++;
        }

        // If all the neighbours are active or activable, return true - else return false
        if (count == 6)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Function UpdateCube is meant to be used in debug mode (from a UI button)
    // It updates the status of the nodes of a single cube from boolean inputs in the interface and update the mesh
    // It allows to easily switch between the 256 values of a cube to check if the triangles are built properly
    public void UpdateCube() // for debug
    {
        controlNodes[0].active = fll;
        controlNodes[1].active = bll;
        controlNodes[2].active = ful;
        controlNodes[3].active = bul;
        controlNodes[4].active = flr;
        controlNodes[5].active = blr;
        controlNodes[6].active = fur;
        controlNodes[7].active = bur;

        cubes[0].GetValue();

        MakeTriangles();
    }
}
