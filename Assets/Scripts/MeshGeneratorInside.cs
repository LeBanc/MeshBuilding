using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGeneratorInside : MeshGenerator
{
    [Range(1,500)]
    public int impactAttenuator = 100; // Attenuator to minimize the effect of an impact

    // public function to create all the nodes and the cube grid
    public override void CreateNodes(int[,,] obj)
    {
        // Clear all lists of nodes and cubes
        controlNodes.Clear();
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
                }
            }
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
                // Cases with 3 active points
                case 7:
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpRight, cubeG.backLowRight);
                    break;
                case 11:
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backLowRight);
                    break;
                case 13:
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    break;
                case 14:
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpLeft, cubeG.backUpRight);
                    break;
                case 19:
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.backLowRight);
                    break;
                case 21:
                case 22: break; // only making the cube faces for the interior view
                case 25:
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.backUpLeft);
                    break;
                case 26:
                case 28: break; // only making the cube faces for the interior view
                case 35:
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    break;
                case 37: break; // only making the cube faces for the interior view
                case 38:
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.frontLowRight);
                    break;
                case 41:
                case 42:
                case 44: break; // only making the cube faces for the interior view
                case 49:
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowRight, cubeG.frontLowLeft);
                    break;
                case 50:
                    CreateTriangle(cubeG.backLowRight, cubeG.frontLowRight, cubeG.backLowLeft);
                    break;
                case 52:
                case 56:
                case 67:
                case 69: break; // only making the cube faces for the interior view
                case 70:
                    CreateTriangle(cubeG.backLowRight, cubeG.backUpRight, cubeG.frontUpRight);
                    break;
                case 73:
                case 74: break; // only making the cube faces for the interior view
                case 76:
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.backUpLeft);
                    break;
                case 81:
                case 82:
                case 84:
                case 88:
                case 97: break; // only making the cube faces for the interior view
                case 98:
                    CreateTriangle(cubeG.backLowRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    break;
                case 100:
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    break;
                case 104: break; // only making the cube faces for the interior view
                case 112:
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.frontUpRight);
                    break;
                case 131:
                case 133:
                case 134: break; // only making the cube faces for the interior view
                case 137:
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontUpLeft, cubeG.backUpLeft);
                    break;
                case 138: break; // only making the cube faces for the interior view
                case 140:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpRight, cubeG.backUpLeft);
                    break;
                case 145:
                    CreateTriangle(cubeG.backLowLeft, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    break;
                case 146:
                case 148: break; // only making the cube faces for the interior view
                case 152:
                    CreateTriangle(cubeG.backUpLeft, cubeG.frontLowLeft, cubeG.frontUpLeft);
                    break;
                case 161:
                case 162:
                case 164:
                case 168: break; // only making the cube faces for the interior view
                case 176:
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontLowRight, cubeG.frontUpLeft);
                    break;
                case 193:
                case 194: break; // only making the cube faces for the interior view
                case 196:
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.frontUpLeft);
                    break;
                case 200:
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpLeft, cubeG.frontUpLeft);
                    break;
                case 208:
                    CreateTriangle(cubeG.frontLowLeft, cubeG.frontUpRight, cubeG.frontUpLeft);
                    break;
                case 224:
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    break;
                // Cases with 4 active points
                case 15:
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 23:
                case 27:
                case 29:
                case 30:
                case 39:
                case 43:
                case 45:
                case 46: break; // only making the cube faces for the interior view
                case 51:
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 53:
                case 54:
                case 57:
                case 58:
                case 60:
                case 71:
                case 75:
                case 77:
                case 78:
                case 83:
                case 85:
                case 86:
                case 89:
                case 90:
                case 92:
                case 99:
                case 101: break; // only making the cube faces for the interior view
                case 102:
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 105:
                case 106:
                case 108:
                case 113:
                case 114:
                case 116:
                case 120:
                case 135:
                case 139:
                case 141:
                case 142:
                case 147:
                case 149:
                case 150: break; // only making the cube faces for the interior view
                case 153:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 154:
                case 156:
                case 163:
                case 165:
                case 166:
                case 169:
                case 170:
                case 172:
                case 177:
                case 178:
                case 180:
                case 184:
                case 195:
                case 197:
                case 198:
                case 201:
                case 202: break; // only making the cube faces for the interior view
                case 204:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 209:
                case 210:
                case 212:
                case 216:
                case 225:
                case 226:
                case 228:
                case 232: break; // only making the cube faces for the interior view
                case 240:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // Cases with 5 active points
                case 31:
                case 47:
                case 55:
                case 59:
                case 61:
                case 62:
                case 79:
                case 87:
                case 91:
                case 93:
                case 94:
                case 103:
                case 107:
                case 109:
                case 110:
                case 115:
                case 117:
                case 118:
                case 121:
                case 122:
                case 124:
                case 143:
                case 151:
                case 155:
                case 157:
                case 158:
                case 167:
                case 171:
                case 173:
                case 174:
                case 179:
                case 181:
                case 182:
                case 185:
                case 186:
                case 188:
                case 199:
                case 203:
                case 205:
                case 206:
                case 211:
                case 213:
                case 214:
                case 217:
                case 218:
                case 220:
                case 227:
                case 229:
                case 230:
                case 233:
                case 234:
                case 236:
                case 241:
                case 242:
                case 244:
                case 248:
                    break; // no case for interior view

                // Cases with 6 active points
                case 63:
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 95: break;
                case 111:
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 119:
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 123:
                case 125:
                case 126: break;
                case 159:
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 175:
                case 183: break;
                case 187:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 189:
                case 190:
                case 207:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 215:
                case 219: break;
                case 221:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 222:
                case 231:
                case 235:
                case 237: break;
                case 238:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 243:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 245: break;
                case 246:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 249:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 250: break;
                case 252:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;

                // Cases with 7 active points
                case 127:
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;
                case 191:
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 223:
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 239:
                    CreateTriangle(cubeG.backLowRight, cubeG.backLowLeft, cubeG.backUpRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backUpLeft, cubeG.backUpRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 247:
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 251:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontLowLeft, cubeG.backLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backLowLeft, cubeG.backLowRight, cubeG.frontLowRight);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 253:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backUpLeft, cubeG.backLowLeft);
                    CreateTriangle(cubeG.frontUpLeft, cubeG.backLowLeft, cubeG.frontLowLeft);
                    cubeG.ClearVerticesIndex();
                    break;
                case 254:
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontLowLeft, cubeG.frontLowRight);
                    CreateTriangle(cubeG.frontLowRight, cubeG.frontUpRight, cubeG.frontUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.frontUpLeft, cubeG.frontUpRight, cubeG.backUpLeft);
                    CreateTriangle(cubeG.frontUpRight, cubeG.backUpRight, cubeG.backUpLeft);
                    cubeG.ClearVerticesIndex();
                    CreateTriangle(cubeG.backUpRight, cubeG.frontUpRight, cubeG.frontLowRight);
                    CreateTriangle(cubeG.backUpRight, cubeG.frontLowRight, cubeG.backLowRight);
                    cubeG.ClearVerticesIndex();
                    break;

                // Case with 8 active points
                case 255: break; // no need for interior display
            }
        }
    }

    // Function to find the nearest node from a position
    ControlNode GetNearestNode(Vector3 position)
    {
        ControlNode nearestNode = new ControlNode(position, true);
        float distance = float.PositiveInfinity; // distance is initialize to at least find a node

        foreach (ControlNode controlNode in controlNodes)
        {
            if ((position - controlNode.position).magnitude < distance)
            {
                distance = (position - controlNode.position).magnitude;
                nearestNode = controlNode;
            }
        }
        return nearestNode;
    }

    // Function to add the effect of an impact (position, force) to the mesh
    public void ImpactEffect(Vector3 position, Vector3 force)
    {
        // Get the nearest node from the impact and create a new node from it moving it of the force value divided by the impact attenuator parameter
        ControlNode node = GetNearestNode(position);
        ControlNode newNode = new ControlNode(node.position,true);
        newNode.position += force / impactAttenuator;

        // From the first index of the node in the vertices list, replace the node by the new node and try to find the node again (to replace it)
        // Exit the loop when the node is not in the vertices list anymore
        int index = vertices.IndexOf(node.position);
        while(index  != -1)
        {
            vertices[index] = newNode.position;
            index = vertices.IndexOf(node.position);
        }

        // Replace the node by the new node in the ControlNodes list
        index = controlNodes.IndexOf(node);
        controlNodes[index] = newNode;

        // Update the vertices of the mesh (the triangles stay the same), recalculate Normals and update collider mesh as well
        objectMesh.mesh.vertices = vertices.ToArray();
        objectMesh.mesh.RecalculateNormals();
        objectMesh.GetComponent<MeshCollider>().sharedMesh = objectMesh.mesh;

    }
}
