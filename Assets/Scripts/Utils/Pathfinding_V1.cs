using System.Collections.Generic;
using UnityEngine;

public class Pathfinding_V1
{
    private float bottomLeftPosX, bottomLeftPosY, topRightPosX, topRightPosY;
    public int countX, countY;
    public float sizeX, sizeY;
    public float realSizeX, realSizeY;

    public Node[,] graph;
    public Pathfinding_V1(float bottomLeftPosX, float bottomLeftPosY, float topRightPosX, float topRightPosY, float sizeX, float sizeY)
    {
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
        graph = new Node[countX + 1, countY + 1];
        float posX = bottomLeftPosX;
        float posY = bottomLeftPosY;

        for (int i = 0; i < countX + 1; i++) //Colonne
        {
            for (int j = 0; j < countY + 1; j++) //Ligne
            {
                Node node = new Node(posX, posY, i, j);
                node.setValid(sizeX, sizeY);
                graph[i, j] = node;
                posY += realSizeY;
            }
            posY = bottomLeftPosY;
            posX += realSizeX;
        }
    }


    /**
     * Méthode pour avoir la direction dans laquelle l'IA doit se déplacer à partir de l'algo A*
     */
    public Vector2 getMovingDirection(Vector2 startPosition, Vector2 goalPosition, float spaceX, float spaceY)
    {
        SortedSet<Node> input = new SortedSet<Node>(new NodeComparator());
        List<Node> output = new List<Node>();

        initNodes(goalPosition, spaceX, spaceY);

        Node start = getNearestNode(startPosition);
        Node end = getNearestNode(goalPosition);

        if (start == null || end == null) return Vector2.zero;

        float cost;

        start.setG(0);
        start.setH(getDistance(new Vector2(start.x, start.y), goalPosition));
        start.setF(start.g + start.h);

        input.Add(start);
        Node current;

        while (input.Count != 0)
        {
            current = input.Min;
            input.Remove(current);
            output.Add(current);

            if (nodeIsReached(end, current))
            {
                return getMouvVectFromNode(current, startPosition);
            }

            foreach (Node neigh in getValidNeighbours(current))
            {
                if (output.Contains(neigh)) continue;
                cost = neigh.g + getDistance(new Vector2(current.x, current.y), new Vector2(neigh.x, neigh.y));

                if (!input.Contains(neigh))
                {
                    neigh.setParent(current);
                    neigh.setG(cost);
                    neigh.setF(neigh.g + neigh.h);

                    input.Add(neigh);
                }
                else
                {
                    if (cost < neigh.g)
                    {
                        input.Remove(neigh);
                        input.Add(neigh);
                    }
                }
            }

        }

        return Vector2.zero; //Il n'y a pas de solution
    }

    public Vector2 getValidPos(float spaceX, float spaceY)
    {
        string[] layers = { "Enemies", "Players", "Obstacle" };
        int mask = LayerMask.GetMask(layers);
        Node current;
        for (int i = 0; i < countX + 1; i++) //Colonne
        {
            for (int j = 0; j < countY + 1; j++) //Ligne
            {
                //Debug.Log("x " + i + " y " + j);
                current = graph[i, j];
                if (current.isValid && !Physics2D.OverlapCapsule(new Vector2(current.x, current.y), new Vector2(spaceX * 1.1f, spaceY * 1.1f), CapsuleDirection2D.Horizontal, 0f, mask)) return new Vector2(current.x, current.y);
            }
        }
        return Vector2.zero;
    }


    private void initValid(float spaceX, float spaceY)
    {
        Node current;
        for (int i = 0; i < countX + 1; i++) //Colonne
        {
            for (int j = 0; j < countY + 1; j++) //Ligne
            {
                current = graph[i, j];
                current.setValid(spaceX, spaceY);
            }

        }
    }
    float getDistance(Vector2 position1, Vector2 position2)
    {
        return Vector2.Distance(position1, position2);
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


    Vector2 getMouvVectFromNode(Node end, Vector2 playerPos)
    {
        List<Node> path = getPathFromNode(end);
        Node current;
        int i = 0;
        if (path.Count != 0)
        {
            current = path[i];
            while (nodeIsReached(current, playerPos) && i < path.Count)
            {
                current = path[i];
                i++;
            }

            return (new Vector2(current.x, current.y) - playerPos).normalized;
        }
        else { return Vector2.zero; } //Il n'y a pas de chemin à l'objectif on ne bouge pas
    }


    List<Node> getPathFromNode(Node node)
    {
        List<Node> path = new List<Node>();
        Node current = node;
        while (current != null)
        {
            path.Add(current);
            current = current.parent;

        }
        path.Reverse();
        if (path.Count > 1) Debug.DrawLine(new Vector2(path[0].x, path[0].y), new Vector2(path[1].x, path[1].y));

        return path;
    }
    public Node getNearestNode(Vector2 playerPos)
    {
        float poseX = playerPos.x - bottomLeftPosX;
        float poseY = playerPos.y - bottomLeftPosY;

        if (poseX < 0 || poseY < 0 || poseX > topRightPosX - bottomLeftPosX || poseY > topRightPosY - bottomLeftPosY)
        {
            // Debug.LogError("Un ennemi un joueur se trouve hors de la map");
            return null;
        }

        int nearestX = Mathf.RoundToInt(poseX / realSizeX);
        int nearestY = Mathf.RoundToInt(poseY / realSizeY);

        //Debug.Log("Player posX " + position.x + " posY " + position.y + " nearestX " + nearestX + " nearestY " + nearestY + " PoseX " +poseX + " PoseY " + poseY);
        Node nearest = findNearestToPlayerNode(graph[nearestX, nearestY], playerPos);
        //Debug.Log("On node " + graph[nearestX, nearestY].matrixX + " " + graph[nearestX, nearestY].matrixY); //Affiche l'index du noeud sur lequel l'ennemi est
        //Debug.Log("Nearest " + nearest.x + " " + nearest.y);
        return nearest;
    }

    Node findNearestToPlayerNode(Node current, Vector2 playerPos)
    {
        if (current.isValid) return current; // Le joueur est sur un noeud courant qui est valide

        Node valid = null;
        List<Node> validNeigh = getValidNeighbours(current);
        if (validNeigh.Count != 0)
        {
            //Si le noeud courant a des voisins valides on retourne le plus proche de sa position
            float distMin = getDistance(new Vector2(validNeigh[0].x, validNeigh[0].y), playerPos);
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


    private void initNodes(Vector2 goal, float spaceX, float spaceY)
    {
        Node current;
        for (int i = 0; i < countX + 1; i++) //Colonne
        {
            for (int j = 0; j < countY + 1; j++) //Ligne
            {
                current = graph[i, j];
                current.setF(0);
                current.setG(0);
                current.setH(getDistance(new Vector2(current.x, current.y), goal));
                current.setParent(null);
                current.setValid(spaceX, spaceY);
            }

        }
    }

    private bool nodeIsReached(Node goal, Vector2 position)
    {
        float distX = Mathf.Abs(goal.x - position.x);
        float distY = Mathf.Abs(goal.y - position.y);
        return distX <= 0.6f * realSizeX && distY < 0.6f * realSizeY;
    }

    private bool nodeIsReached(Node goal, Node current)
    {
        float distX = Mathf.Abs(goal.x - current.x);
        float distY = Mathf.Abs(goal.y - current.y);
        return distX <= 0.6f * realSizeX && distY <= 0.6f * realSizeY;
    }

}

