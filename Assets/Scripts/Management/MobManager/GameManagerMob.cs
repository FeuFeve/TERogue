using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Allows us to use Lists. 

public class GameManagerMob : MonoBehaviour {

    public static GameManagerMob instance = null; // Static instance of GameManager which allows it to be accessed by any other script.
    private BoardManagerMob boardScript; // Store a reference to our BoardManager which will set up the level.
    private int level;

    // Awake is always called before any Start functions
    void Start() {
        instance = this;

        if (instance == null) // Check if instance already exists
            instance = this;
        else if (instance != this) // If instance already exists and it's not this
            Destroy(gameObject);
        
        //DontDestroyOnLoad(gameObject);

        // Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManagerMob>();

        // Call the InitGame function to initialize the first level 
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame() {
        // Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene();
    }

}