using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour {
    public static string player1Name = "PL1";
    public static string player2Name = "PL2";
    public static bool isMultiplayer = false;

    public static int level = 0;
    public static int position = 0;
    public static string predRoomType = "EntryRoom";
    public static string roomType = "EntryRoom";
    public static int probMerchantMax = 25;
    public static int probMerchantActual = probMerchantMax;
    
    public static List<GameObject> enemies = new List<GameObject>();

    public static int max_number_world = 2;
    public static int[] levelOrder = { 1, 0, 2 };
    public static int currentWorld = 0; // levelOrder iterator
    public static int world = levelOrder[currentWorld];

    public static void Init() {
        level = 0;
        position = 0;
        predRoomType = "EntryRoom";
        roomType = "EntryRoom";
        enemies = new List<GameObject>();
        max_number_world = 2;
        levelOrder = new int[] { 1, 0, 2 };
        currentWorld = 0;
        world = levelOrder[currentWorld];
    }
}
