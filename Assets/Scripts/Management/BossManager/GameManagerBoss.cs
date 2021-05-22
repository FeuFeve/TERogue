using System;
using UnityEngine;
using System.Collections;

using System.Collections.Generic;        //Allows us to use Lists. 

public class GameManagerBoss : MonoBehaviour
{

    public static GameManagerBoss instance = null;                //Static instance of GameManager which allows it to be accessed by any other script.
    private BoardManagerBoss boardScript;                        //Store a reference to our BoardManager which will set up the level.
    private int level;

    //Awake is always called before any Start functions
    void Start()
    {
        instance = this;
       
        //Check if instance already exists
        if (instance == null)instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)Destroy(gameObject);    

        
        //DontDestroyOnLoad(gameObject);

        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManagerBoss>();

        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    //Initializes the game for each level.
    void InitGame()
    {
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        
        boardScript.SetupScene();

    }
    
}