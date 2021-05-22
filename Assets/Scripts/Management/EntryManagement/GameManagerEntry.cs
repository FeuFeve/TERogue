using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Allows us to use Lists. 

public class GameManagerEntry : MonoBehaviour {

    public static GameManagerEntry instance = null; // Static instance of GameManager which allows it to be accessed by any other script.
    private BoardManagerEntry boardScript; // Store a reference to our BoardManager which will set up the level.
    private int level;

    //Awake is always called before any Start functions
    void Awake() {
        instance = this;
        
        if (instance == null) // Check if instance already exists
            instance = this;
        else if (instance != this) // If instance already exists and it's not this:
            Destroy(gameObject);
        
        //DontDestroyOnLoad(gameObject);

        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManagerEntry>();

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame() {
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene();
    }
}