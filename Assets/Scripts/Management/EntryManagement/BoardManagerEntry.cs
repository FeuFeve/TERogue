using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine.SceneManagement; // Allows us to use Lists.
using Random = UnityEngine.Random;

public class BoardManagerEntry : MonoBehaviour
{
    public List<Texture2D> presetList;
    public List<Texture2D> pathList;


    public int columns; // Number of columns in our game board.
    public int rows; // Number of rows in our game board.
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

    public GameObject[] descendingStairsRightMob;

    public List<GameObject> ennemyPrefabs;
    public PathManager pathManager;



    public List<GameObject> availableFloor;

    public float bottomLeftPosX = -0.5f;
    public float bottomLeftPosY = -0.5f;
    public float topRightPosX;
    public float topRightPosY;

    void InitialiseList()
    {
        availableFloor = new List<GameObject>();
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

    public void SetupScene()
    {
        InitialiseList();
        BoardSetup();
        foreach (GameObject p in Player.playerList)
        {
            p.transform.position = new Vector3(-1, rows / 2, 0f);
        }

        pathManager.init(bottomLeftPosX, bottomLeftPosY, topRightPosX, topRightPosY, 0.2f, 0.2f);
        CreateEnnemies();
    }


    //Sets up the outer walls and floor (background) of the game board.
    void BoardSetup()
    {
        topRightPosX = columns + 0.5f;
        topRightPosY = rows + 0.5f;
        // print("La position est " + position);

        string[,] map = MapGenerator.nextMap(presetList, pathList, 3, 1, 0, columns, rows);

        GameObject toInstantiate = null;
        GameObject instance = null;

        //Instantiate Board and set boardHolder to its transform.
        boardHolder = new GameObject("Board").transform;
        boardHolder.parent = this.transform;


        setupMapInside(map);
        setupMapOutside(map);
    }


    private void setupMapInside(string[,] map)
    {
        GameObject toInstantiate = null;
        GameObject instance = null;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                int rand = Random.Range(0, 100);
                if (rand < 2) toInstantiate = floor_variant1_tiles[GameData.world];
                else if (rand < 4) toInstantiate = floor_variant2_tiles[GameData.world];
                else toInstantiate = floor_tiles[GameData.world];

                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
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
                    if (rows / 2 - 2 < y && y < rows / 2 + 2)
                    {
                        if (y == rows / 2)
                        {
                            toInstantiate = ascendingStairsLeft[GameData.world];
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
                    if (rows / 2 - 2 < y && y < rows / 2 + 2)
                    {
                        if (y == rows / 2)
                        {
                            toInstantiate =
                                descendingStairsRightMob[GameData.world];
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
                        toInstantiate =
                            wallBorderCornerTopLeft[GameData.world];
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
                        toInstantiate =
                            wallBorderCornerTopRight[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);

                    }
                    else if (x != 0 && x != columns-1 && y == rows)
                    {

                        int rand = Random.Range(0, 100);

                        if (rand < 80) toInstantiate = wallTop[GameData.world];
                        else if (rand < 90) toInstantiate = wallTopVariant1[GameData.world];
                        else toInstantiate = wallTopVariant2[GameData.world];

                        instance =
                            Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);

                    }
                    else if (x != 0 && x != columns -1  && y == 0)
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

    public void CreateEnnemies()
    {
        if (ennemyPrefabs.Count > 0)
        {
            List<GameObject> toSummon = new List<GameObject>();
            float max_score = 1f;
            Debug.Log("max_score" + max_score);
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
                    float new_score;
                    for (int i = 0; i < 10; i++)
                    {
                        enemy = ennemyPrefabs[Random.Range(0, ennemyPrefabs.Count)];
                        new_score = enemy.GetComponent<IA_controller>().scoreValue;
                        if (new_score + tot_score < max_score)
                        {
                            current = new_score;
                            break;
                        }
                        else if (new_score < min)
                        {
                            smallest = enemy;
                            min = new_score;
                            current = new_score;
                        }
                    }
                }
                toSummon.Add(enemy);
                tot_score += current;
            }

            foreach (GameObject summon in toSummon)
            {
                Vector2 spawnPos = pathManager.getSpawnPos(new Vector2(5f, 5f));
                GameObject npc = Instantiate(summon, spawnPos, Quaternion.identity);
                npc.transform.parent = transform;
                GameData.enemies.Add(npc);
            }

            foreach (GameObject ennemy in GameData.enemies)
            {
                ennemy.BroadcastMessage("registerPathManager", pathManager);
            }
        }

    }
}


