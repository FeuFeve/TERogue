using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Node[,] graph;
    List<Node> currentPath;
    PathManager manager;
    public int rayCount = 16;
    public float esq_range = 1f;
    private float cellSizeX, cellSizeY;
    private float bottomLeftPosX, bottomLeftPosY, topRightPosX, topRightPosY;
    public Collider2D collision_collider; //Le collider de déplacement du personnage


    Node currentToPlayer, currentToTarget;

    [SerializeField]
    private Transform target;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //A chaque frame on enregistre le pathfinding pour une update du chemin, le manager donne le feu vert pour le calcul quand il le souhaite afin de limiter la charge de calcul 
        if (manager != null)
        {
            manager.registerForUpdate(this);
        }
    }

    public void registerManager(PathManager manager)
    {
        this.manager = manager;
        this.cellSizeX = manager.realSizeX; //TODO ici connard manager.realSizeX
        this.cellSizeY = manager.realSizeY; //TODO ici connard
        this.bottomLeftPosX = manager.bottomLeftPosX;
        this.bottomLeftPosY = manager.bottomLeftPosY;
        this.topRightPosX = manager.topRightPosX;
        this.topRightPosY = manager.topRightPosY;
        this.graph = cloneGridNode(manager.getGraph());

        initGrid(); //On initialise une seule fois l'accessibilité des points du graph
    }

    public PathManager GetPathManager()
    {
        return this.manager;
    }

    public virtual void initGrid()
    {

        Node current;
        for (int i = 0; i < graph.GetLength(0); i++) //Colonne
        {
            for (int j = 0; j < graph.GetLength(1); j++) //Ligne
            {
                current = graph[i, j];
                current.setValid(collision_collider.bounds.size.x, collision_collider.bounds.size.y);
                if (i == graph.GetLength(0) - 1 || j == graph.GetLength(0) - 1 || i == 0 || j == 0)
                {
                    current.isValid = false;
                }
            }
        }
    }

    public virtual Node[,] cloneGridNode(Node[,] toClone)
    {
        Node[,] newGraph = new Node[toClone.GetLength(0), toClone.GetLength(1)];
        Node node;
        for (int x = 0; x < toClone.GetLength(0); x += 1)
        {
            for (int y = 0; y < toClone.GetLength(1); y += 1)
            {
                node = toClone[x, y];
                newGraph[x, y] = new Node(node.x, node.y, node.matrixX, node.matrixY);
            }
        }
        return newGraph;
    }

    public void updatePath()
    {
        if (target != null) this.currentPath = calculatePath();
    }

    private void initNodes(Vector2 goal)
    {
        Node current;
        for (int i = 0; i < graph.GetLength(0); i++) //Colonne
        {
            for (int j = 0; j < graph.GetLength(1); j++) //Ligne
            {
                current = graph[i, j];
                current.setF(0);
                current.setG(0);
                current.setH(getDistance(new Vector2(current.x, current.y), goal));
                current.setParent(null);
                current.isReached = false;
            }

        }
    }

    float getDistance(Vector2 position1, Vector2 position2)
    {
        return Vector2.Distance(position1, position2);
    }

    private List<Node> calculatePath()
    {
        SortedSet<Node> input = new SortedSet<Node>(new NodeComparator());
        initNodes(target.transform.position);

        Node start = getNearestNode((Vector2)collision_collider.transform.position + collision_collider.offset);
        Node end = getNearestNode(target.position);


        currentToPlayer = start;
        currentToTarget = end;

        if (start == null || end == null) return currentPath;

        float cost;

        start.setG(0);
        start.setH(getDistance(new Vector2(start.x, start.y), target.position));
        start.setF(start.g + start.h);

        input.Add(start);
        Node current;

        while (input.Count != 0)
        {
            current = input.Min;
            input.Remove(current);
            current.isReached = true;

            if (current == end)
            {
                return composePath(current);
            }

            foreach (Node neigh in getValidNeighbours(current))
            {
                if (neigh.isReached) continue;
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

        return currentPath; //Il n'y a pas de solution
    }

    private List<Node> composePath(Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;
        while (current != null)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Reverse();
        return path;
    }

    public Node getNearestNode(Vector2 playerPos)
    {
        float poseX = playerPos.x - bottomLeftPosX;
        float poseY = playerPos.y - bottomLeftPosY;

        float graphPosRight = topRightPosX - bottomLeftPosX;
        float graphPosTop = topRightPosY - bottomLeftPosY;

        int nearestX = Mathf.RoundToInt(poseX / cellSizeX);
        int nearestY = Mathf.RoundToInt(poseY / cellSizeY);

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
            else if (poseX > graphPosRight && poseY < 0) //En bas à droite 
            {
                return findNearestToPlayerNode(graph[graph.GetLength(0) - 1, 0], playerPos);
            }
            else if (poseX > graphPosRight && poseY > graphPosTop) //En haut à droite
            {
                return findNearestToPlayerNode(graph[graph.GetLength(0) - 1, graph.GetLength(1) - 1], playerPos);
            }
            else if (poseX < 0 && poseY > graphPosTop) //En haut à gauche
            {
                return findNearestToPlayerNode(graph[0, graph.GetLength(1) - 1], playerPos);
            }
            else if (poseX < 0 && poseY < 0) //En bas à gauche
            { return findNearestToPlayerNode(graph[0, 0], playerPos); }
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

    /**
     * Permet de retourner un vecteur de mouvement afin d'atteindre l'objectif spécifié 
     */
    public Vector2 getMouvementVector()
    {
        if (currentPath == null || currentPath.Count == 0)
        {
            if (target == null)
            {
                return Vector2.zero;
            }
            else
            {
                return getMouvementByVector(target.position, (Vector2)collision_collider.transform.position + collision_collider.offset);
            }
        }

        if (target == null)
        {
            return Vector2.zero;
        }
        else
        {
            if (currentPath.Count == 0) return getMouvementByVector(target.position, gameObject.transform.position);
        }

        //On cherche le noeud du chemin qui n'est pas atteint et on retourne le vecteur pour l'atteindre

        Node current;


        if (currentPath.Count != 0)
        {

            for (int i = 0; i < currentPath.Count; i++)
            {
                if (!nodeIsReached(currentPath[i], (Vector2)collision_collider.transform.position + collision_collider.offset))
                {
                    return (new Vector2(currentPath[i].x, currentPath[i].y) - new Vector2(collision_collider.transform.position.x + collision_collider.offset.x, collision_collider.transform.position.y + collision_collider.offset.y)).normalized;
                }
                else
                {
                    currentPath.Remove(currentPath[i]);
                }
            }

            return Vector2.zero;
        }

        //Si le bout du chemin est atteint on retourne le vecteur zero pour ne plus se déplacer
        return Vector2.zero;
    }

    Vector2 getMouvVectFromNode(Node end, Vector2 playerPos)
    {
        List<Node> path = composePath(end);
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

    /**
     * Calcul vectoriel par cast de collider afin de gérer les déplacements de l'ennemi s'il n'a pas de chemin
     */
    public Vector2 getMouvementByVector(Vector2 objectif, Vector2 currentPos)
    {
        Vector2 res = Vector2.zero;
        List<Vector2> noHitVectors = new List<Vector2>();
        int hited;
        for (int i = 0; i < rayCount; i++)
        {

            var rotation = this.transform.rotation;
            var rotation_mod = Quaternion.AngleAxis((i / (float)rayCount) * 360, this.transform.forward);
            var direction = rotation * rotation_mod * Vector2.right;
            //RaycastHit2D hitInfo;
            string[] masks = { "Obstacle", "VoidObstacle" };
            LayerMask mask = LayerMask.GetMask(masks);
            ContactFilter2D filter = new ContactFilter2D();
            filter.SetLayerMask(mask);
            RaycastHit2D[] results = new RaycastHit2D[10];
            //hitInfo = Physics2D.CapsuleCast(capsule.transform.position, capsule.size * 0.95f, capsule.direction, capsule.transform.rotation.z, direction, esq_range, obstacleMask)
            hited = collision_collider.Cast(direction, filter, results, esq_range, true);
            if (hited == 0)
            {
                noHitVectors.Add(direction);
            }
        }

        if (noHitVectors.Count != 0)
        {
            float angleToObjec = Vector2.Angle(noHitVectors[0], objectif - currentPos);
            res = noHitVectors[0];

            foreach (Vector2 noCollision in noHitVectors)
            {
                float angle = Vector2.Angle(noCollision, objectif - currentPos);
                //Debug.Log("Angle " + angle);
                if (angle < angleToObjec)
                {
                    res = noCollision;
                    angleToObjec = angle;
                }
            }
        }
        return res.normalized;
    }

    public void setTarget(Transform target)
    {
        this.target = target;
    }

    private bool nodeIsReached(Node goal, Vector2 position)
    {
        float dist = Vector2.Distance(new Vector2(goal.x, goal.y), new Vector2(position.x, position.y));
        return dist < 0.5f;
    }

    private bool nodeIsReached(Node goal, Node current)
    {
        float distX = Mathf.Abs(goal.x - current.x);
        float distY = Mathf.Abs(goal.y - current.y);
        return distX <= 0.7f * cellSizeX && distY <= 0.7f * cellSizeY;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        if (currentPath != null && currentPath.Count > 0)
        {
            Node previous = currentPath[0];
            foreach (Node node in currentPath)
            {
                Gizmos.DrawLine(new Vector2(node.x, node.y), new Vector2(previous.x, previous.y));
                previous = node;
            }
        }

        /**

        Gizmos.color = Color.red;

        if (currentToPlayer != null)
        {
            Gizmos.DrawSphere(new Vector2(currentToPlayer.x, currentToPlayer.y), 0.2f);
        }

        if (currentToTarget != null)
        {
            Gizmos.DrawSphere(new Vector2(currentToTarget.x, currentToTarget.y), 0.2f);
        }

        Node current;
        try
        {
            {
                for (int i = 0; i < graph.GetLength(0); i++) //Colonne
                {
                    for (int j = 0; j < graph.GetLength(1); j++) //Ligne
                    {
                        current = graph[i, j];
                        if (current.isValid)
                        {
                            Gizmos.color = Color.white;
                            Gizmos.DrawSphere(new Vector2(current.x, current.y), 0.05f);
                        }
                        else
                        
                        if (!current.isValid)
                        {
                            Gizmos.color = Color.red;
                            Gizmos.DrawSphere(new Vector2(current.x, current.y), 0.05f);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {

        } **/

    }

    public Vector2 getValidSpawnPos(Vector2 currentPos)
    {
        Node nearest = getNearestNode(currentPos);
        if (nearest != null)
        {
            return new Vector2(nearest.x, nearest.y);
        }
        else
        {
            throw new Exception("No valid position for spawning");
        }
    }
}
