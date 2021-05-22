using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingPathfinding : Pathfinding
{
   

    public override Node[,] cloneGridNode(Node[,] toClone)
    {
        Node[,] newGraph = new Node[toClone.GetLength(0), toClone.GetLength(1)];
        Node node;
        for (int x = 0; x < toClone.GetLength(0); x += 1)
        {
            for (int y = 0; y < toClone.GetLength(1); y += 1)
            {
                node = toClone[x, y];
                newGraph[x, y] = new FlyingNode(node.x, node.y, node.matrixX, node.matrixY);
            }
        }
        return newGraph;
    }

    public override void initGrid()
    {
        FlyingNode current;
        for (int i = 0; i < graph.GetLength(0); i++) //Colonne
        {
            for (int j = 0; j < graph.GetLength(1); j++) //Ligne
            {
                current = (FlyingNode) graph[i, j];
                current.setValid(collision_collider.bounds.size.x, collision_collider.bounds.size.y);
            }
        }
    }

}
