using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.SceneManagement; // Allows us to use Lists.
using Random = UnityEngine.Random;

public class BoardManagerBoss : MonoBehaviour
{
    public List<Texture2D> presetList;
    public List<Texture2D> pathList;


    public int columns = 12; // Number of columns in our game board.
    public int rows = 12; // Number of rows in our game board.
    private Transform boardHolder; // A variable to store a reference to the transform of our Board object.
    private List<Vector3> gridPositions = new List<Vector3>(); // A list of possible locations to place tiles.

    public GameObject[] floor_tiles;
    public GameObject[] floor_variant1_tiles;
    public GameObject[] floor_variant2_tiles;
    
    private int localisation_player = 0;

    public GameObject[] wallTop;
    public GameObject[] wallTopVariant1;
    public GameObject[] wallTopVariant2;
    public GameObject[] wallBottom;
    public GameObject[] wallBottomVariant1;
    public GameObject[] wallBottomVariant2;
    public GameObject[] wallBorderLeft;
    public GameObject[] wallBorderRight;

    public GameObject[] wallBorderCornerTopRight;
    public GameObject[] wallBorderCornerTopLeft;
    public GameObject[] wallCornerBottomLeft;
    public GameObject[] wallCornerBottomRight;


    public GameObject[] spriteHole;
    public GameObject[] spriteHoleTop;
    public GameObject[] spriteColumns;


    public GameObject[] ascendingStairsLeft;
    public GameObject[] ascendingStairsRight;
    public GameObject[] ascendingStairsTop;
    public GameObject[] ascendingStairsBottom;

    public GameObject[] descendingStairsLeftMob;
    public GameObject[] descendingStairsRightMob;
    public GameObject[] descendingStairsTopMob;
    public GameObject[] descendingStairsBottomMob;

    public GameObject[] descendingStairsLeftMerchant;
    public GameObject[] descendingStairsRightMerchant;
    public GameObject[] descendingStairsTopMerchant;
    public GameObject[] descendingStairsBottomMerchant;


    public GameObject[] gridLeft;
    public GameObject[] gridRight;
    public GameObject[] gridTop;
    public GameObject[] gridBottom;


    private bool merchant = false; // Une seule salle qui donne sur un marchant par niveau 
    private bool inRoomMerchant = false; // dans une seule dans marchant 
    private int probMerchant = GameData.probMerchantActual;

    public List<GameObject> availableFloor;
    public List<GameObject> ennemies;


    public List<GameObject> ennemiesPrefab;
    public PathManager pathManager;

    public GameObject reward;
    private bool rewarded;
    private bool created = false;


    private float bottomLeftPosX = -0.5f;
    private float bottomLeftPosY = -0.5f;
    private float topRightPosX;
    private float topRightPosY;

    void InitialiseList()
    {
        availableFloor = new List<GameObject>();
        ennemies = new List<GameObject>();
        //Clear our list gridPositions.
        gridPositions.Clear();

        //Loop through x axis (columns).
        for (int x = 1; x < columns - 1; x++)
        {
            //Within each column, loop through y axis (rows).
            for (int y = 1; y < rows - 1; y++)
            {
                //At each index add a new Vector3 to our list with the x and y coordinates of that position.
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    public void SetupScene() // départ = escaliser d'arriver du joueur, 0 = gauche; 1 = haut, 2 = droite; 3 = bas
    {
        this.rewarded = false;
        //Reset our list of gridpositions.
        InitialiseList();

        localisation_player = GameData.position;
        if (GameData.roomType.Contains("Merchant")) inRoomMerchant = true; // pas deux marchands d'affilée

        InitPlayer(localisation_player);

        //Creates the outer walls and floor.
        BoardSetup();

    }


    void BoardSetup()
    {
        topRightPosX = columns + 0.5f;
        topRightPosY = rows + 0.5f;

        string[,] map = MapGenerator.nextMap(presetList, pathList, 3, 1, 0, columns, rows);

        GameObject toInstantiate = null;
        GameObject instance = null;

        boardHolder = new GameObject("Board").transform;
        boardHolder.parent = this.transform;

        boardHolder.gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        boardHolder.gameObject.AddComponent<CompositeCollider2D>();
        setupMapInside(map);
        setupMapOutside(map);

        pathManager.init(bottomLeftPosX, bottomLeftPosY, topRightPosX, topRightPosY, 0.2f, 0.2f);
        if (!GameData.roomType.Contains("Merchant"))
        {
            CreateEnnemies();
        }

        if(GameData.probMerchantActual != 1)  GameData.probMerchantActual--;
    }


    public void CreateEnnemies()
    {
        List<GameObject> toSummon = new List<GameObject>();
        float max_score = GameData.level % 10 == 0 ? 10 * GameData.level : (GameData.level % 10) * 10 + GameData.level;
        Debug.Log("max_score" + max_score);
        float tot_score = 0;
        float current;
        GameObject enemy;
        while (tot_score < max_score)
        {
            enemy = ennemiesPrefab[Random.Range(0, ennemiesPrefab.Count)]; //On sélectionne un ennemi au hasard
            current = enemy.GetComponent<IA_controller>().scoreValue; //On récupère son score
            toSummon.Add(enemy);
            tot_score += current;
        }

        foreach (GameObject summon in toSummon)
        {
            Vector2 spawnPos = pathManager.getSpawnPos((Vector2)Player.playerList[0].gameObject.transform.position);
            GameObject npc = Instantiate(summon, spawnPos, Quaternion.identity);
            npc.transform.parent = transform;
            GameData.enemies.Add(npc);
        }

        foreach (GameObject ennemy in GameData.enemies)
        {
            ennemy.BroadcastMessage("registerPathManager", pathManager);
        }
        created = true;
    }


    private void InitPlayer(int localisation_player)
    {
        foreach (GameObject p in Player.playerList)
        {
            switch (localisation_player)
            {
                case 0:
                    p.transform.position = new Vector3(0, rows / 2, 0f);
                    break;

                case 1:
                    p.transform.position = new Vector3(columns / 2, rows - 1, 0f);

                    break;

                case 2:
                    p.transform.position = new Vector3(columns - 1, rows / 2, 0f);
                    break;

                case 3:
                    p.transform.position = new Vector3(columns / 2, 0, 0f);
                    break;
            }
        }
    }

    private void setupMapInside(string[,] map)
    {
        GameObject toInstantiate = null;
        GameObject instance = null;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (GameData.roomType.Contains("Merchant") || GameData.roomType.Contains("Boss"))
                {
                    int rand = Random.Range(0, 100);
                    if (rand < 2) toInstantiate = floor_variant1_tiles[GameData.world];
                    else if (rand <4) toInstantiate = floor_variant2_tiles[GameData.world];
                    else toInstantiate = floor_tiles[GameData.world];
                    instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                    instance.transform.SetParent(boardHolder);
                }
                else
                {
                    if (x == 0 || y == 0 || x == columns || y == rows)
                    {
                        toInstantiate = floor_tiles[GameData.world];
                    }
                    else if (map[x, y].Equals("floor"))
                    {
                        if (Random.Range(0, 100) < 5) toInstantiate = floor_tiles[GameData.world];
                        else toInstantiate = floor_tiles[GameData.world];
                    }
                    else if (map[x, y].Equals("obstacle"))
                    {
                        toInstantiate = floor_tiles[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);

                        toInstantiate = spriteColumns[GameData.world];
                    }
                    else if (map[x, y].Equals("void"))
                    {
                        if (y + 1 >= rows || !map[x, y + 1].Equals("void"))
                            toInstantiate = spriteHoleTop[GameData.world];
                        else
                            toInstantiate = spriteHole[GameData.world];
                    }

                    if (toInstantiate != null)
                    {
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                    }

                    if (x >= 0 && y >= 0 && x < columns && y < rows && !map[x, y].Equals("obstacle") &&
                        !map[x, y].Equals("void")) availableFloor.Add(instance);
                }
            }
        }
    }

    private void setupMapOutside(string[,] map)
    {
        GameObject toInstantiate = null;
        GameObject instance = null;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows + 1; y++)
            {
                if (x == 0)
                {
                    if (rows / 2 - 2 < y && y < rows / 2 + 2 && ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 ||
                                                                 localisation_player == 0 || localisation_player == 2 ||
                                                                 GameData.level == 1))
                    {
                        if (y == rows / 2)
                        {
                            if (!inRoomMerchant)// pas de grille dans la salle de marchant 
                            {
                                toInstantiate = gridLeft[GameData.world];
                                instance =
                                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                            }

                            if (localisation_player != 0 && GameData.level != 1)
                            {

                                if (Random.Range(0, probMerchant) == 1 && !merchant && GameData.level != 9 && !inRoomMerchant)
                                {
                                    toInstantiate =
                                        descendingStairsLeftMerchant[
                                            GameData.world];
                                    merchant = true;
                                }
                                else
                                    toInstantiate = descendingStairsLeftMob[GameData.world];
                            }

                            else toInstantiate = ascendingStairsLeft[GameData.world];

                            instance =
                                Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);

                        }
                    }
                    else
                    {
                        toInstantiate = wallBorderLeft[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                    }
                }
                else if (x == columns - 1)
                {
                    if (rows / 2 - 2 < y && y < rows / 2 + 2 && ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 ||
                                                                 localisation_player == 0 || localisation_player == 2 ||
                                                                 GameData.level == 1))
                    {
                        if (y == rows / 2)
                        {
                            if (!inRoomMerchant) // pas de grille dans la salle de marchant 
                            {
                                toInstantiate = gridRight[GameData.world];
                                instance =
                                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);

                            }

                            if (localisation_player != 2 || GameData.level == 1)
                            {

                                if (Random.Range(0, probMerchant) == 1 && !merchant && GameData.level != 9 &&
                                    !inRoomMerchant)
                                {
                                    toInstantiate =
                                        descendingStairsRightMerchant[
                                            Random.Range(0, descendingStairsRightMerchant.Length)];
                                    merchant = true;
                                }
                                else
                                    toInstantiate =
                                        descendingStairsRightMob[GameData.world];
                            }
                            else
                                toInstantiate = ascendingStairsRight[GameData.world];

                            instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);

                        }

                    }
                    else
                    {
                        toInstantiate = wallBorderRight[GameData.world];
                        instance =
                            Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                    }
                }

                if (y == 0 || y == rows)
                {
                    if (x == 0 && y == 0)
                    {
                        toInstantiate = wallCornerBottomLeft[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                    }
                    else if (x == 0 && y == rows)
                    {
                        toInstantiate = wallBorderCornerTopLeft[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);

                        //On a 2 sprites en une, pas besoin de la bordure
                        //toInstantiate = wallCornerTopLeft[Random.Range(0, wallCornerTopLeft.Length)];
                    }
                    else if (x == columns -1 && y == 0)
                    {
                        toInstantiate = wallCornerBottomRight[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                    }
                    else if (x == columns-1 && y == rows)
                    {
                        toInstantiate = wallBorderCornerTopRight[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);

                        //On a 2 sprites en une, pas besoin de la bordure
                        //toInstantiate = wallCornerTopRight[Random.Range(0, wallCornerTopRight.Length)];
                    }
                    else if (x != 0 && x != columns && y == rows)

                    {
                        if (columns / 2 - 2 < x && x < columns / 2 + 2 &&
                            ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 || localisation_player == 3 ||
                             localisation_player == 1 || GameData.level == 1))
                        {
                            if (x == columns / 2)
                            {
                                if (!inRoomMerchant)// pas de grille dans la salle de marchant 
                                {
                                    toInstantiate = gridTop[GameData.world];
                                    instance =
                                        Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                    instance.transform.SetParent(boardHolder);
                                }

                                if (localisation_player != 1)
                                {

                                    if (Random.Range(0, probMerchant) == 1 && !merchant && GameData.level != 9 && !inRoomMerchant)
                                    {
                                        toInstantiate =
                                            descendingStairsTopMerchant[
                                                GameData.world];
                                        merchant = true;
                                    }
                                    else
                                        toInstantiate =
                                            descendingStairsTopMob[GameData.world];

                                }
                                else toInstantiate = ascendingStairsTop[GameData.world];

                                instance =
                                    Instantiate(toInstantiate, new Vector3(x, y, 0f),
                                        Quaternion.identity);
                                instance.transform.SetParent(boardHolder);

                            }
                        }
                        else
                        {
                            int rand = Random.Range(0, 100);

                            if (rand < 90) toInstantiate = wallTop[GameData.world];
                            else if (rand < 95) toInstantiate = wallTopVariant1[GameData.world];
                            else toInstantiate = wallTopVariant2[GameData.world];

                            instance =
                                Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                        }
                    }
                    else if (x != 0 && x != columns && y == 0)
                    {
                        if (columns / 2 - 2 < x && x < columns / 2 + 2 &&
                            ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 || localisation_player == 3 ||
                             localisation_player == 1 || GameData.level == 1))
                        {
                            if (x == columns / 2)
                            {
                                if (!inRoomMerchant)// pas de grille dans la salle de marchant 
                                {
                                    toInstantiate = gridBottom[GameData.world];
                                    instance =
                                        Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                    instance.transform.SetParent(boardHolder);
                                }

                                if (localisation_player != 3)
                                {

                                    if (Random.Range(0, probMerchant) == 1 && !merchant && GameData.level != 9 && !inRoomMerchant)
                                    {
                                        toInstantiate =
                                            descendingStairsBottomMerchant[
                                                GameData.world];
                                        merchant = true;
                                    }
                                    else
                                        toInstantiate =
                                            descendingStairsBottomMob
                                                [GameData.world];

                                }
                                else toInstantiate = ascendingStairsBottom[GameData.world];

                                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);

                            }
                        }
                        else
                        {
                            int rand = Random.Range(0, 100);

                            if (rand < 80) toInstantiate = wallBottom[GameData.world];
                            else if (rand < 90) toInstantiate = wallBottomVariant1[GameData.world];
                            else toInstantiate = wallBottomVariant2[GameData.world];

                            instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                        }
                    }
                }
            }
        }
    }

    public void Start()
    {
        this.rewarded = false;
    }

    public void Update()
    {
        //Debug.Log("Update boss room Ennemies : " + GameData.enemies.Count + " created : " + created + " rewarded : " + rewarded);
        if (GameData.enemies.Count <= 0 && created && !rewarded)
        {
            Vector2 pos = pathManager.getSpawnPos(new Vector2((topRightPosX - bottomLeftPosX) / 2, (topRightPosY - bottomLeftPosY) / 2));
            Instantiate(reward, pos, Quaternion.identity);
            rewarded = true;
            while (Player.deadPlayerList.Count > 0) {
                GameObject p = Player.deadPlayerList[0];
                p.gameObject.SetActive(true);
                p.GetComponent<Player>().Rez();
                print("REZ!");
            }
        }
        
    }
    // void OnDrawGizmos()
    // {
    //     if (pathfinding != null)
    //     {
    //         Node node;
    //         for (int x = 0; x < pathfinding.graph.GetLength(0); x += 1)
    //         {
    //             for (int y = 0; y < pathfinding.graph.GetLength(1); y += 1)
    //             {
    //                 node = pathfinding.graph[x, y];
    //                 Color color = node.isValid ? Color.white : Color.red;
    //                 Gizmos.color = color;
    //                 Gizmos.DrawSphere(new Vector2(node.x, node.y), 0.05f);
    //             }
    //         }
    //     }
    // }
}