using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManagerMob : MonoBehaviour
{
    public List<Texture2D> presetList;
    public List<Texture2D> pathList;


    public int columns = 12;
    public int rows = 12;
    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();

    public GameObject[] floor_tiles;
    public GameObject[] floor_variant1_tiles;
    public GameObject[] floor_variant2_tiles;
    
    private int localisation_player = 0;

    public GameObject[] wallTop;
    public GameObject[] wallTopVariant1;
    public GameObject[] wallTopVariant2;
    public GameObject[] fontainTopRed;
    public GameObject[] wallBottom;
    public GameObject[] wallBottomVariant1;
    public GameObject[] wallBottomVariant2;
    public GameObject[] wallBorderLeft;
    public GameObject[] wallBorderRight;

    public GameObject[] wallBorderCornerTopRight;
    public GameObject[] wallBorderCornerTopLeft;
    public GameObject[] wallCornerBottomLeft;
    public GameObject[] wallCornerBottomRight;

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

    public GameObject[] descendingStairsLeftBoss;
    public GameObject[] descendingStairsRightBoss;
    public GameObject[] descendingStairsTopBoss;
    public GameObject[] descendingStairsBottomBoss;

    public GameObject[] gridLeft;
    public GameObject[] gridRight;
    public GameObject[] gridTop;
    public GameObject[] gridBottom;

    public GameObject[] wallInsadeHorizontalLeft;
    public GameObject[] wallInsadeHorizontalMiddle;
    public GameObject[] wallInsadeHorizontalRight;

    public GameObject[] wallInsadeVerticalBot;
    public GameObject[] wallInsadeVerticalMiddle;
    public GameObject[] wallInsadeVerticalTop;

    public GameObject[] wallInsadeTopLeft;
    public GameObject[] wallInsadeTopMiddle;
    public GameObject[] wallInsadeTopRight;

    public GameObject[] wallInsadeLeft;
    public GameObject[] wallInsadeRight;
    public GameObject[] wallInsadeCenter;

    public GameObject[] wallInsadeBotLeft;
    public GameObject[] wallInsadeBotMiddle;
    public GameObject[] wallInsadeBotMiddleVariant1;
    public GameObject[] wallInsadeBotMiddleVariant2;
    public GameObject[] wallInsadeBotRight;

    public GameObject[] wallInsadeSpecialBotLeft;
    public GameObject[] wallInsadeSpecialBotRight;
    public GameObject[] wallInsadeSpecialTopRight;
    public GameObject[] wallInsadeSpecialTopLeft;

    public GameObject[] hole;
    public GameObject[] holeTop;
    public GameObject[] floorHoleTop;
    public GameObject[] floorHoleBot;
    public GameObject[] floorHoleLeft;
    public GameObject[] floorHoleRight;
    public GameObject[] floorHoleCornerBotRight;
    public GameObject[] floorHoleCornerBotLeft;
    public GameObject[] floorHoleCornerTopRight;
    public GameObject[] floorHoleCornerTopLeft;
    
    
    public GameObject[] Columns;


    private bool merchant = false; // Une seule salle qui donne sur un marchant par niveau 
    private bool fontainRed = false;
    private int probMerchant = GameData.probMerchantActual;

    public List<GameObject> availableFloor;


    public List<GameObject> ennemyPrefabs;
    public PathManager pathManager;


    private float bottomLeftPosX = -0.5f;
    private float bottomLeftPosY = -0.5f;
    private float topRightPosX;
    private float topRightPosY;

    void InitialiseList()
    {
        availableFloor = new List<GameObject>();
        GameData.enemies = new List<GameObject>();
        gridPositions.Clear();
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    public void SetupScene() // départ = escaliser d'arriver du joueur, 0 = gauche; 1 = haut, 2 = droite; 3 = bas
    {
        InitialiseList();
        localisation_player = GameData.position;
        InitPlayer(localisation_player);
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
        if(GameData.level!= 9 && GameData.probMerchantActual > 1) GameData.probMerchantActual--;
    }


    public void CreateEnnemies()
    {
        List<GameObject> toSummon = new List<GameObject>();
        float max_score = GameData.level % 10 == 0 ? 10 * GameData.level: (GameData.level % 10) * 10 + GameData.level; 
        //Debug.Log("max_score" + max_score);
        float tot_score = 0;
        float current;
        GameObject enemy;
        while (tot_score < max_score)
        {
            enemy = ennemyPrefabs[Random.Range(0, ennemyPrefabs.Count)]; //On sélectionne un ennemi au hasard
            current = enemy.GetComponent<IA_controller>().scoreValue; //On récupère son score

            if (tot_score + current > max_score)
            {
                float min = current;
                GameObject smallest = enemy;
                GameObject newEnnemy;
                float new_score;
                for (int i = 0; i < 10; i++)
                {
                    newEnnemy = ennemyPrefabs[Random.Range(0, ennemyPrefabs.Count)];
                    new_score = newEnnemy.GetComponent<IA_controller>().scoreValue;
                    if (new_score + tot_score < max_score)
                    {
                        current = new_score;
                        break;
                    }
                    else if (new_score < min)
                    {
                        smallest = newEnnemy;
                        min = new_score;
                        current = new_score;
                    }
                }
                enemy = smallest;
            }
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
                Boolean isWallLeft = false;
                Boolean isWallRight = false;
                Boolean isWallTop = false;
                Boolean isWallBot = false;

                if (GameData.roomType.Contains("Merchant"))
                {
                    toInstantiate = floor_tiles[GameData.world];
                    instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                    instance.transform.SetParent(boardHolder);
                }
                else
                {
                    if (map[x, y].Equals("floor"))
                    {
                        int rand = Random.Range(0, 100);
                        if (rand < 2) toInstantiate = floor_variant1_tiles[GameData.world];
                        else if (rand <4) toInstantiate = floor_variant2_tiles[GameData.world];
                        else toInstantiate = floor_tiles[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                        
                        if (x != 0)
                            if (map[x - 1, y].Equals("void"))
                            {
                                toInstantiate = floorHoleRight[GameData.world];
                                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                            }
                        if (x != columns - 1)
                            if (map[x + 1, y].Equals("void"))
                            {
                                toInstantiate = floorHoleLeft[GameData.world];
                                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                            }
                        // if (y != 0)
                        //     if (map[x, y - 1].Equals("void"))
                        //     {
                        //         toInstantiate = floorHoleBot[GameData.world];
                        //         instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        //         instance.transform.SetParent(boardHolder);
                        //     }
                        
                        if( y  != rows  -1 )
                            if (map[x, y + 1 ].Equals("void") )
                            {
                                toInstantiate = floorHoleTop[GameData.world];
                                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                            }
                        if( y  != 0)
                        if (map[x, y - 1 ].Equals("void") )
                        {
                            toInstantiate = floorHoleBot[GameData.world];
                            instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                        }
                        
                        if( y  != rows  -1 && x != columns  -1)
                            if (map[x+1, y + 1 ].Equals("void") && map[x+1, y  ].Equals("floor")&& map[x, y+1  ].Equals("floor"))
                            {
                                toInstantiate = floorHoleCornerBotLeft[GameData.world];
                                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                            }
                        
                        if( y  != rows  -1 && x != 0)
                            if (map[x-1, y + 1 ].Equals("void") && map[x-1, y  ].Equals("floor")&& map[x, y+1  ].Equals("floor"))
                            {
                                toInstantiate = floorHoleCornerBotRight[GameData.world];
                                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                            }
                        
                        if( y  != 0&& x != 0)
                            if (map[x-1, y - 1 ].Equals("void") && map[x-1, y  ].Equals("floor")&& map[x, y-1  ].Equals("floor"))
                            {
                                toInstantiate = floorHoleCornerTopRight[GameData.world];
                                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                            }
                        
                        if( y  != 0 && x != columns-1)
                            if (map[x+1, y - 1 ].Equals("void") && map[x+1, y  ].Equals("floor")&& map[x, y-1  ].Equals("floor"))
                            {
                                toInstantiate = floorHoleCornerTopLeft[GameData.world];
                                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                            }
                        
                        
                    }
                    else if (map[x, y].Equals("obstacle"))
                    {
                        if (x != 0)
                            if (map[x - 1, y].Equals("obstacle"))
                                isWallLeft = true;
                        if (x != columns - 1)
                            if (map[x + 1, y].Equals("obstacle"))
                                isWallRight = true;
                        if (y != 0)
                            if (map[x, y - 1].Equals("obstacle"))
                                isWallBot = true;
                        if (y != rows - 1)
                            if (map[x, y + 1].Equals("obstacle"))
                                isWallTop = true;

                        if (!isWallLeft && !isWallRight && !isWallBot && !isWallTop)
                        {
                            toInstantiate = floor_tiles[GameData.world];
                            instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                            toInstantiate = Columns[GameData.world];
                        }

                        if (isWallLeft && !isWallRight && !isWallBot && !isWallTop)
                            toInstantiate =
                                wallInsadeHorizontalLeft[GameData.world];
                        if (isWallLeft && isWallRight && !isWallBot && !isWallTop)
                            toInstantiate =
                                wallInsadeHorizontalMiddle[GameData.world];
                        if (!isWallLeft && isWallRight && !isWallBot && !isWallTop)
                            toInstantiate =
                                wallInsadeHorizontalRight[GameData.world];

                        ////////////////////////////////////////////////////////////////////////

                        if (!isWallLeft && !isWallRight && isWallBot && !isWallTop)
                            toInstantiate =
                                wallInsadeVerticalTop[GameData.world];
                        if (!isWallLeft && !isWallRight && isWallBot && isWallTop)
                            toInstantiate =
                                wallInsadeVerticalMiddle[GameData.world];
                        if (!isWallLeft && !isWallRight && !isWallBot && isWallTop)
                            toInstantiate =
                                wallInsadeVerticalBot[GameData.world];
                        ////////////////////////////////////////////////////////////////////////

                        if (!isWallLeft && isWallRight && !isWallBot && isWallTop)
                            if (map[x + 1, y + 1].Equals("obstacle"))
                                toInstantiate =
                                    wallInsadeBotLeft[GameData.world];
                            else
                                toInstantiate =
                                    wallInsadeSpecialBotLeft[GameData.world];
                        if (isWallLeft && isWallRight && !isWallBot && isWallTop)
                        {
                            int rand = Random.Range(0, 100);
                            if (rand < 33) toInstantiate = wallInsadeBotMiddle[GameData.world];
                            else if (rand < 66) toInstantiate = wallInsadeBotMiddleVariant1[GameData.world];
                            else toInstantiate = wallInsadeBotMiddleVariant2[GameData.world];
                        }


                        if (isWallLeft && !isWallRight && !isWallBot && isWallTop)
                            if (map[x - 1, y + 1].Equals("obstacle"))
                                toInstantiate =
                                    wallInsadeBotRight[GameData.world];
                            else
                                toInstantiate =
                                    wallInsadeSpecialBotRight[GameData.world];

                        ////////////////////////////////////////////////////////////////////////

                        if (!isWallLeft && isWallRight && isWallBot && isWallTop)
                            toInstantiate =
                                wallInsadeLeft[GameData.world];
                        if (isWallLeft && isWallRight && isWallBot && isWallTop)
                            toInstantiate =
                                wallInsadeCenter[GameData.world];
                        if (isWallLeft && !isWallRight && isWallBot && isWallTop)
                            toInstantiate =
                                wallInsadeRight[GameData.world];

                        ////////////////////////////////////////////////////////////////////////

                        if (!isWallLeft && isWallRight && isWallBot && !isWallTop)
                            if (map[x + 1, y - 1].Equals("obstacle"))
                                toInstantiate =
                                    wallInsadeTopLeft[GameData.world];
                            else
                                toInstantiate =
                                    wallInsadeSpecialTopLeft[GameData.world];
                        if (isWallLeft && isWallRight && isWallBot && !isWallTop)
                            toInstantiate =
                                wallInsadeTopMiddle[GameData.world];
                        if (isWallLeft && !isWallRight && isWallBot && !isWallTop)
                            if (map[x - 1, y - 1].Equals("obstacle"))
                                toInstantiate =
                                    wallInsadeTopRight[GameData.world];
                            else
                                toInstantiate =
                                    wallInsadeSpecialTopRight[GameData.world];
                        ////////////////////////////////////////////////////////////////////////

                        int rand2 = Random.Range(0, 100);
                        if (y != 0)
                            if (rand2 < 1 && !fontainRed && map[x, y - 1].Equals("floor"))
                            {
                                instance =
                                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                                fontainRed = true;
                                toInstantiate = fontainTopRed[0];
                            }

                    }
                    else if (map[x, y].Equals("void"))
                    {
                        if (y + 1 >= rows || !map[x, y + 1].Equals("void"))
                        {
                            toInstantiate = holeTop[GameData.world];
                            instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                        }
                        else
                        {
                            toInstantiate = hole[GameData.world];
                            instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                        }

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

    /*
    private void setupMapOutside (string[,] map) {
        
        // Z H H 3 G G Y
        // A           F
        // A           F
        // 0           2
        // B           E
        // B           E
        // W C C 1 D D X

        //A
        for (int x = 0; x < columns / 2 - 1; x++) {
            int y = 0;

        }
    }
    */


    private void setupMapOutside(string[,] map)
    {
        GameObject toInstantiate = null;
        GameObject instance = null;
        bool fontainRed = false;
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows + 1; y++)
            {
                if (x == 0)
                {
                    if (rows / 2 - 2 < y && y < rows / 2 + 2 &&
                        ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 ||
                         localisation_player == 0 || localisation_player == 2 ||
                         GameData.level == 1))
                    {
                        if (y == rows / 2)
                        {
                            toInstantiate = gridLeft[GameData.world];
                            instance =
                                Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);


                            if (localisation_player != 0 && GameData.level != 1)
                            {
                                if ((GameData.level - 9) % 10 == 0)
                                {
                                    toInstantiate =
                                        descendingStairsLeftBoss[GameData.world];
                                }
                                else
                                {
                                    if (Random.Range(0, probMerchant) == 1 && !merchant && GameData.level != 9)
                                    {
                                        toInstantiate =
                                            descendingStairsLeftMerchant[
                                                GameData.world];
                                        merchant = true;
                                    }
                                    else
                                        toInstantiate =
                                            descendingStairsLeftMob[GameData.world];
                                }
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
                    if (rows / 2 - 2 < y && y < rows / 2 + 2 &&
                        ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 ||
                         localisation_player == 0 || localisation_player == 2 ||
                         GameData.level == 1))
                    {
                        if (y == rows / 2)
                        {
                            toInstantiate = gridRight[GameData.world];
                            instance =
                                Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);


                            if (localisation_player != 2 || GameData.level == 1)
                            {
                                if ((GameData.level - 9) % 10 == 0)
                                {
                                    toInstantiate =
                                        descendingStairsRightBoss[GameData.world];
                                }
                                else
                                {
                                    if (Random.Range(0, probMerchant) == 1 && !merchant && GameData.level != 9)
                                    {
                                        toInstantiate =
                                            descendingStairsRightMerchant[
                                                GameData.world];
                                        merchant = true;
                                    }
                                    else
                                        toInstantiate =
                                            descendingStairsRightMob[GameData.world];
                                }
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
                    }
                    else if (x == columns - 1 && y == 0)
                    {
                        toInstantiate = wallCornerBottomRight[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                    }
                    else if (x == columns - 1 && y == rows)
                    {
                        toInstantiate = wallBorderCornerTopRight[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                    }
                    else if (x != 0 && x != columns - 1 && y == rows)

                    {
                        if (columns / 2 - 2 < x && x < columns / 2 + 2 &&
                            ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 ||
                             localisation_player == 3 ||
                             localisation_player == 1 || GameData.level == 1))
                        {
                            if (x == columns / 2)
                            {
                                toInstantiate = gridTop[GameData.world];
                                instance =
                                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);

                                if (localisation_player != 1)
                                {
                                    if ((GameData.level - 9) % 10 == 0)
                                    {
                                        toInstantiate =
                                            descendingStairsTopBoss[GameData.world];
                                    }
                                    else
                                    {
                                        if (Random.Range(0, probMerchant) == 1 && !merchant && GameData.level != 9)
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

                            if (rand < 1 && !fontainRed && map[x, y - 1].Equals("floor"))
                            {
                                toInstantiate = wallTop[GameData.world];
                                instance =
                                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                                fontainRed = true;
                                toInstantiate = fontainTopRed[0];

                            }
                            else {
                       
                                int rand2 = Random.Range(0, 100);
                            
                                if (rand2 < 90) toInstantiate = wallTop[GameData.world];
                                else if (rand2 <95) toInstantiate = wallTopVariant1[GameData.world];
                                else toInstantiate = wallTopVariant2[GameData.world]; 
                            
                                instance =
                                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                        
                            }

                            instance =
                                Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                        }
                    }
                    else if (x != 0 && x != columns - 1 && y == 0)
                    {
                        if (columns / 2 - 2 < x && x < columns / 2 + 2 &&
                            ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 ||
                             localisation_player == 3 ||
                             localisation_player == 1 || GameData.level == 1))
                        {
                            if (x == columns / 2)
                            {
                                toInstantiate = gridBottom[GameData.world];
                                instance =
                                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);

                                if (localisation_player != 3)
                                {
                                    if ((GameData.level - 9) % 10 == 0)
                                    {
                                        toInstantiate =
                                            descendingStairsBottomBoss[GameData.world];
                                    }
                                    else
                                    {
                                        if (Random.Range(0, probMerchant) == 1 && !merchant && GameData.level != 9)
                                        {
                                            toInstantiate =
                                                descendingStairsBottomMerchant[GameData.world];
                                            merchant = true;
                                        }
                                        else
                                            toInstantiate =
                                                descendingStairsBottomMob
                                                    [GameData.world];
                                    }
                                }
                                else
                                    toInstantiate =
                                        ascendingStairsBottom[GameData.world];

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