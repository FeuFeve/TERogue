using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class BoardManagerMerchant : MonoBehaviour
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
    public GameObject[] wallBottom;
    public GameObject[] wallBottomVariant1;
    public GameObject[] wallBottomVariant2;
    public GameObject[] wallBorderLeft;
    public GameObject[] wallBorderRight;

    public GameObject[] ascendingStairsLeft;
    public GameObject[] ascendingStairsRight;
    public GameObject[] ascendingStairsTop;
    public GameObject[] ascendingStairsBottom;
    
    public GameObject[] wallBorderCornerTopRight;
    public GameObject[] wallBorderCornerTopLeft;
    public GameObject[] wallCornerBottomLeft;
    public GameObject[] wallCornerBottomRight;

    public GameObject[] descendingStairsLeftMob;
    public GameObject[] descendingStairsRightMob;
    public GameObject[] descendingStairsTopMob;
    public GameObject[] descendingStairsBottomMob;
    
    public GameObject[] descendingStairsLeftBoss;
    public GameObject[] descendingStairsRightBoss;
    public GameObject[] descendingStairsTopBoss;
    public GameObject[] descendingStairsBottomBoss;
    
    public GameObject[] gridLeft;
    public GameObject[] gridRight;
    public GameObject[] gridTop;
    public GameObject[] gridBottom;

    void InitialiseList()
    {
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
        BoardSetup();
        InitPlayer(localisation_player);
    }


    
    void BoardSetup()
    {
        string[,] map = MapGenerator.nextMap(presetList, pathList, 3, 1, 0, columns, rows);

        GameObject toInstantiate = null;
        GameObject instance = null;
        boardHolder = new GameObject("Board").transform;
        boardHolder.parent = this.transform;
        boardHolder.gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        boardHolder.gameObject.AddComponent<CompositeCollider2D>();
        setupMapInside(map);
        setupMapOutside(map);
        GameData.probMerchantActual = GameData.probMerchantMax;
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
                else if (rand <4) toInstantiate = floor_variant2_tiles[GameData.world];
                else toInstantiate = floor_tiles[GameData.world];
                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
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
                            
                            if (localisation_player != 0 && GameData.level != 1)
                            {
                                if ((GameData.level - 9) % 10 == 0)
                                {
                                    toInstantiate =
                                        descendingStairsLeftBoss[GameData.world];
                                }
                                else
                                {
                                        toInstantiate =
                                        descendingStairsLeftMob[GameData.world];
                                }
                            }
                            else
                            {
                                
                                toInstantiate = gridLeft[GameData.world];
                                instance =
                                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                                toInstantiate = ascendingStairsLeft[GameData.world];
                            }

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
                else if (x == columns -1 )
                {
                    if (rows / 2 - 2 < y && y < rows / 2 + 2 && ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 ||
                                                                 localisation_player == 0 || localisation_player == 2 ||
                                                                 GameData.level == 1))
                    {
                        if (y == rows / 2)
                        {


                            if (localisation_player != 2 || GameData.level == 1)
                            {
                                if ((GameData.level - 9) % 10 == 0) toInstantiate =
                                        descendingStairsRightBoss[GameData.world];
                                
                                else
                                {
                                    toInstantiate =
                                        descendingStairsRightMob[GameData.world];
                                }
                            }
                            else
                            {
                                toInstantiate = gridRight[GameData.world];
                                instance =
                                    Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);
                                toInstantiate = ascendingStairsRight[GameData.world];
                            }
                               

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
                    else if (x == columns -1  && y == 0)
                    {
                        toInstantiate = wallCornerBottomRight[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                    }
                    else if (x == columns -1  && y == rows)
                    {
                        toInstantiate = wallBorderCornerTopRight[GameData.world];
                        instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                        instance.transform.SetParent(boardHolder);
                        
                    }
                    else if (x != 0 && x != columns -1  && y == rows)

                    {
                        if (columns / 2 - 2 < x && x < columns / 2 + 2 &&
                            ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 || localisation_player == 3 ||
                             localisation_player == 1 || GameData.level == 1))
                        {
                            if (x == columns / 2)
                            {


                                if (localisation_player != 1)
                                {
                                    if ((GameData.level - 9) % 10 == 0)
                                    {
                                        toInstantiate =
                                            descendingStairsTopBoss[GameData.world];
                                    }
                                    else
                                    {

                                        toInstantiate =
                                            descendingStairsTopMob[GameData.world];
                                    }
                                }
                                else
                                {
                                    toInstantiate = gridTop[GameData.world];
                                    instance =
                                        Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                    instance.transform.SetParent(boardHolder);
                                    toInstantiate = ascendingStairsTop[GameData.world];
                                }

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
                            else if (rand <95) toInstantiate = wallTopVariant1[GameData.world];
                            else toInstantiate = wallTopVariant2[GameData.world]; 
                            
                            instance =
                                Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                        }
                    }
                    else if (x != 0 && x != columns -1 && y == 0)
                    {
                        if (columns / 2 - 2 < x && x < columns / 2 + 2 &&
                            ((GameData.level - 9) % 10 != 0 && (GameData.level - 10) % 10 != 0 || localisation_player == 3 ||
                             localisation_player == 1 || GameData.level == 1))
                        {
                            if (x == columns / 2)
                            {

                                if (localisation_player != 3)
                                {
                                    if ((GameData.level - 9) % 10 == 0)
                                    {
                                        toInstantiate =
                                            descendingStairsBottomBoss[
                                                GameData.world];
                                    }
                                    else
                                    {

                                        toInstantiate =
                                            descendingStairsBottomMob
                                                [GameData.world];
                                    }
                                }
                                else
                                {
                                    toInstantiate = gridBottom[GameData.world];
                                    instance =
                                        Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                    instance.transform.SetParent(boardHolder);
                                    toInstantiate =
                                        ascendingStairsBottom[GameData.world];

                                }
                                    
                                instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                                instance.transform.SetParent(boardHolder);

                            }
                        }
                        else
                        {
                            int rand = Random.Range(0, 100);
                            
                            if (rand < 80) toInstantiate = wallBottom[GameData.world];
                            else if (rand <90) toInstantiate = wallBottomVariant1[GameData.world];
                            else toInstantiate = wallBottomVariant2[GameData.world]; 

                            instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                            instance.transform.SetParent(boardHolder);
                        }
                    }
                }
            }
        }
    }
}
