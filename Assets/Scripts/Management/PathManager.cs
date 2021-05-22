using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class PathManager : MonoBehaviour
{

    Queue<Pathfinding> updateQueue;

    public float bottomLeftPosX, bottomLeftPosY, topRightPosX, topRightPosY;
    int countX, countY;
    public float sizeX, sizeY;
    public float realSizeX, realSizeY;
    bool created = false;

    private Node[,] graph;

    public void Start()
    {

    }

    public void init(float bottomLeftPosX, float bottomLeftPosY, float topRightPosX, float topRightPosY, float sizeX, float sizeY)
    {
        //Debug.LogError("Initiating pathmanager");
        this.bottomLeftPosX = bottomLeftPosX;
        this.bottomLeftPosY = bottomLeftPosY;
        this.topRightPosX = topRightPosX;
        this.topRightPosY = topRightPosY;
        this.sizeX = sizeX; //Ici on peut modifier l'espacement des points en x
        this.sizeY = sizeY; //Ici on peut modifier l'espacement des points en y
        this.countX = (int)((topRightPosX - bottomLeftPosX) / this.sizeX);
        this.countY = (int)((topRightPosY - bottomLeftPosY) / this.sizeY);
        this.realSizeX = (float)(Mathf.Abs(topRightPosX - bottomLeftPosX)) / countX;
        this.realSizeY = (float)(Mathf.Abs(topRightPosY - bottomLeftPosY)) / countY;
        this.updateQueue = new Queue<Pathfinding>();
        this.graph = new Node[countX + 1, countY + 1];
        float posX = bottomLeftPosX;
        float posY = bottomLeftPosY;

        for (int i = 0; i < countX + 1; i++) //Colonne
        {
            for (int j = 0; j < countY + 1; j++) //Ligne
            {
                Node node = new Node(posX, posY, i, j);
                node.setValid(1.02f, 1.02f);
                this.graph[i, j] = node;
                posY += realSizeY;
            }
            posY = bottomLeftPosY;
            posX += realSizeX;
        }
        created = true;
        //Debug.Log("Bottom Left x : " + graph[0, 0].x + " y : " + graph[0, 0].y);
    }

    public void registerForUpdate(Pathfinding pathfinder)
    {
        if (!updateQueue.Contains(pathfinder))
        {
            updateQueue.Enqueue(pathfinder);
        }
    }

    public Node[,] getGraph()
    {
        return graph;
    }

    public void Update()
    {
        if (created)
        {
            Pathfinding current;
            int counter = 0;

            while (counter < 5 && updateQueue.Count != 0)
            {
                counter++;
                current = updateQueue.Dequeue();
                if (current != null) current.updatePath();
            }
        }
    }

    public void OnDrawGizmos()
    {
        try
        {
            Node current;
            for (int i = 0; i < graph.GetLength(0); i++)

            {
                for (int j = 0; j < graph.GetLength(1); j++)
                {
                    current = graph[i, j];

                    if (current == null) Debug.LogError("Mauvaise initialisation des noeuds du pathManager");
                    //Debug.Log("Current x :" + current.x + " y : " + current.y);
                    if (current.isValid)
                    {
                        Gizmos.color = Color.green;
                        Gizmos.DrawSphere(new Vector2(current.x, current.y), 0.02f);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawSphere(new Vector2(current.x, current.y), 0.02f);
                    }

                }
            }
        }
        catch (Exception e)
        {

        }
    }

    public Vector2 getSpawnPos(Vector2 playerPosition)
    {
        List<Node> validNodes = new List<Node>();
        Node current;
        for (int i = 0; i < graph.GetLength(0); i++)

        {
            for (int j = 0; j < graph.GetLength(1); j++)
            {
                current = graph[i, j];
                if (getDistance(playerPosition, new Vector2(current.x, current.y)) > 6f && current.isValid) //Le noeud est suffisament éloigné de la position du joueur
                {
                    validNodes.Add(current);
                }
            }
        }

        if (validNodes.Count == 0) throw new Exception("No valid spawn position");

        Node pos = validNodes[Random.Range(0, validNodes.Count)];
        return new Vector2(pos.x, pos.y);
    }

    public Node getNearestNode(Vector2 playerPos)
    {
        float poseX = playerPos.x - bottomLeftPosX;
        float poseY = playerPos.y - bottomLeftPosY;

        float graphPosRight = topRightPosX - bottomLeftPosX;
        float graphPosTop = topRightPosY - bottomLeftPosY;

        int nearestX = Mathf.RoundToInt(poseX / realSizeX);
        int nearestY = Mathf.RoundToInt(poseY / realSizeY);

        //if (poseX < 0 || poseY < 0 || poseX > topRightPosX || poseY > topRightPosY)
        if (poseX < 0 || poseY < 0 || poseX > graphPosRight || poseY > graphPosTop)
        {
            if (isBetween(poseY, 0, graphPosTop))
            {
                if (poseX < 0) return findNearestToPlayerNode(graph[0, nearestY], playerPos);
                if (poseX > graphPosRight)
                {
                    return findNearestToPlayerNode(graph[graph.GetLength(0) - 1, nearestY], playerPos);

                }
            }
            else if (isBetween(poseX, 0, graphPosRight))
            {
                if (poseY > graphPosTop) return findNearestToPlayerNode(graph[nearestX, graph.GetLength(1) - 1], playerPos);
                if (poseY < 0) return findNearestToPlayerNode(graph[nearestX, 0], playerPos);
            }
            return null;
        }

        Node nearest = findNearestToPlayerNode(graph[nearestX, nearestY], playerPos);
        return nearest;
    }

    public bool isBetween(float val, float low, float up)
    {
        return val <= up && val >= low;
    }

    Node findNearestToPlayerNode(Node current, Vector2 playerPos)
    {
        if (current.isValid)// Le joueur est sur un noeud courant qui est valide
        {
            return current;
        }
        //else Debug.Log("Sur un noeud non valide x : " + current.matrixX + " y : " + current.matrixY);

        Node valid = null;
        List<Node> validNeigh = getValidNeighbours(current);
        if (validNeigh.Count != 0)
        {
            //Si le noeud courant a des voisins valides on retourne le plus proche de sa position
            float distMin = getDistance(new Vector2(validNeigh[0].x, validNeigh[0].y), playerPos);
            valid = validNeigh[0];
            foreach (Node neigh in validNeigh)
            {
                if (getDistance(new Vector2(neigh.x, neigh.y), playerPos) < distMin)
                {
                    distMin = getDistance(new Vector2(neigh.x, neigh.y), playerPos);
                    valid = neigh;
                }
            }
            return valid;
        }
        else
        {
            List<Node> toSearch = getNeighbours(current);
            int index = 0;
            while (index < toSearch.Count)
            {
                if (toSearch[index].isValid) return toSearch[index];
                foreach (Node neigh in getNeighbours(toSearch[index]))
                {
                    if (neigh.isValid) return neigh;
                    if (!toSearch.Contains(neigh)) toSearch.Add(neigh);
                }
                index++;
            }
        }
        return null;
    }

    public List<Node> getValidNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        if (node.matrixX - 1 >= 0)
        {
            if (graph[node.matrixX - 1, node.matrixY].isValid) neighbours.Add(graph[node.matrixX - 1, node.matrixY]); //Voisin gauche
        }

        if (node.matrixX + 1 < graph.GetLength(0))
        {
            if (graph[node.matrixX + 1, node.matrixY].isValid) neighbours.Add(graph[node.matrixX + 1, node.matrixY]); //Voisin droite
        }

        if (node.matrixY - 1 >= 0)
        {
            if (graph[node.matrixX, node.matrixY - 1].isValid) neighbours.Add(graph[node.matrixX, node.matrixY - 1]); //Voisin dessous
        }

        if (node.matrixY + 1 < graph.GetLength(1))
        {
            if (graph[node.matrixX, node.matrixY + 1].isValid) neighbours.Add(graph[node.matrixX, node.matrixY + 1]); //Voisin dessus
        }

        return neighbours;
    }

    public List<Node> getNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        if (node.matrixX - 1 >= 0)
        {
            neighbours.Add(graph[node.matrixX - 1, node.matrixY]); //Voisin gauche
        }

        if (node.matrixX + 1 < graph.GetLength(0))
        {
            neighbours.Add(graph[node.matrixX + 1, node.matrixY]); //Voisin droite
        }

        if (node.matrixY - 1 >= 0)
        {
            neighbours.Add(graph[node.matrixX, node.matrixY - 1]); //Voisin dessous
        }

        if (node.matrixY + 1 < graph.GetLength(1))
        {
            neighbours.Add(graph[node.matrixX, node.matrixY + 1]); //Voisin dessus
        }

        return neighbours;
    }

    float getDistance(Vector2 position1, Vector2 position2)
    {
        return Vector2.Distance(position1, position2);
    }
}
