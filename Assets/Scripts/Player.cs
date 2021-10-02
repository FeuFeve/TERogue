using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : Character {
    public string Up;
    public string Left;
    public string Down;
    public string Right;
    public string FireUp;
    public string FireLeft;
    public string FireDown;
    public string FireRight;

    public static List<GameObject> playerList = new List<GameObject>();
    public static List<GameObject> deadPlayerList = new List<GameObject>();

    public int ID;

    public Weapon weapon;
    Animator animator;

    private int _gold = 0;
    public int gold {
        get {
            goldUI.text = "" + _gold;
            return _gold;
        }
        set {
            _gold = value;
            goldUI.text = "" + _gold;
        }
    }

    // UI
    public TextMeshProUGUI nameUI;
    public TextMeshProUGUI goldUI;
    public Bar healthBar;
    private SpriteRenderer sprite;
    private Color baseSpriteColor;
    public GameObject statsBar;
    private TextMeshProUGUI damagesUI;
    private TextMeshProUGUI attackSpeedUI;
    private TextMeshProUGUI critChancesUI;
    private TextMeshProUGUI evasionUI;


    public void Awake() {
        isHumanPlayer = true;
    }

    public override void Start() {
        base.Start();
        
        gold = 75;

        sprite = transform.Find("Skin").GetComponent<SpriteRenderer>();
        baseSpriteColor = sprite.color;

        if (ID != 1 && !GameData.isMultiplayer) { // No multiplayer
            gameObject.SetActive(false); // Deactivate Player 2
            GameObject.Find("Player 2 GUI").SetActive(false); // Hide its UI
        }
        else {
            weapon = GetComponent<Weapon>();
            if (!playerList.Contains(gameObject))
                playerList.Add(gameObject);
            animator = GetComponentInChildren<Animator>();

            InitPlayerUI();
        }
    }



    // Update is called once per frame
    public override void Update() {
        InputMove();
        InputShoot();
        Animate();
        base.Update();
        UpdatePlayerUI(); // Update UI last
    }

    void InputMove() {
        Vector3 direction = Vector3.zero;
        direction.x = Input.GetAxis(Right) - Input.GetAxis(Left);
        direction.y = Input.GetAxis(Up) - Input.GetAxis(Down);
        Move(direction);
    }

    Vector3 shootDirection = new Vector3();
    void InputShoot() {
        Vector3 newShootDirection = new Vector3();
        newShootDirection.x = Input.GetAxis(FireRight) - Input.GetAxis(FireLeft);
        newShootDirection.y = Input.GetAxis(FireUp) - Input.GetAxis(FireDown);
        if (newShootDirection != Vector3.zero) shootDirection = newShootDirection;
        //Up
        if (Input.GetButtonDown(FireUp) || Input.GetButtonDown(FireRight) || Input.GetButtonDown(FireDown) || Input.GetButtonDown(FireLeft)) {
            weapon.StartShooting();
        }

        /*
        Vector3 velocity = new Vector3();
        if (!(weapon.projectilePrefab.GetComponent<Projectile>() is LaserProjectile)) {
            velocity.x += transform.GetComponent<Rigidbody2D>().velocity.x;
            velocity.y += transform.GetComponent<Rigidbody2D>().velocity.y;
            velocity = velocity.normalized / 2;
        }*/
        weapon.ShootDirection(shootDirection/* + velocity */);
        if (!Input.GetButton(FireUp) && !Input.GetButton(FireRight) && !Input.GetButton(FireDown) && !Input.GetButton(FireLeft)) {
            weapon.StopShooting();
        }
    }

    private void Animate() {
        float currentSpeed = rb.velocity.magnitude;
        //animator.SetFloat("Speed", (Mathf.Max(currentSpeed - 0.1f, 0) / (data as CharacterData).speed));
        animator.SetFloat("Speed", Mathf.Min(currentSpeed - 0.1f, 10f) / (data as CharacterData).speed);
    }

    public override void Die() {
        deadPlayerList.Add(gameObject);
        playerList.Remove(gameObject);

        if (playerList.Count == 0) { // All players are dead
            FindObjectOfType<AudioManager>().Play(SFX.GameOver);
            FindObjectOfType<AudioManager>().SetMusicTheme(MusicTheme.None);
            FindObjectOfType<GameOver>().Trigger();
        }
        else { // Multiplayer is ON and another player is still alive
            FindObjectOfType<AudioManager>().Play(SFX.GameOver, 0.3f);
            FindObjectOfType<ScoreManager>().ApplyPlayerDeathPenality();
        }

        gameObject.SetActive(false);
        //Destroy(gameObject); // TODO: don't destroy, only deactivate (easier to revive the player)
    }

    public virtual void Rez() {
        print("EXEC!");
        
        gameObject.SetActive(true);

        Player.playerList.Add(gameObject);
        Player.deadPlayerList.Remove(gameObject);
    
        Heal((data as CharacterData).maxHP, HpChangesType.normalHeal);
    }

    public static void DontDestroyChildOnLoad(GameObject child) {
        Transform parentTransform = child.transform;

        // If this object doesn't have a parent then its the root transform.
        while (parentTransform.parent != null) {
            // Keep going up the chain.
            parentTransform = parentTransform.parent;
        }
        GameObject.DontDestroyOnLoad(parentTransform.gameObject);
    }

    // Initialize the player's UI when he is created
    private void InitPlayerUI() {
        if (GameData.player1Name == null) { // Launched the game without using the main menu
            if (ID == 1) // Player 1
                nameUI.text = "PL1";
            else         // Player 2
                nameUI.text = "PL2";
        }
        else {
            if (ID == 1) // Player 1
                nameUI.text = GameData.player1Name;
            else         // Player 2
                nameUI.text = GameData.player2Name;
        }

        healthBar.SetMax((data as CharacterData).maxHP);
        healthBar.SetValue(HP);

        // Player stats
        damagesUI = statsBar.transform.Find("Damages").Find("Value (TMP)").GetComponent<TextMeshProUGUI>();
        attackSpeedUI = statsBar.transform.Find("Attack Speed").Find("Value (TMP)").GetComponent<TextMeshProUGUI>();
        critChancesUI = statsBar.transform.Find("Crit Chances").Find("Value (TMP)").GetComponent<TextMeshProUGUI>();
        evasionUI = statsBar.transform.Find("Evasion").Find("Value (TMP)").GetComponent<TextMeshProUGUI>();
    }

    // Contains the code to update all the UI stuff related to the current player
    private void UpdatePlayerUI() {
        healthBar.SetMax((data as CharacterData).maxHP);
        healthBar.SetValue(HP);

        if (isInvicible) { // Invincibility animation for the player's sprite
            float lerpValue = Mathf.PingPong(6 * (invincibilityEndTime - Time.time), 1) / 2f + 0.25f; // Values between [0.25, 0.75]
            sprite.color = Color.Lerp(new Color(1, 1, 1, 0), baseSpriteColor, lerpValue);
            healthBar.transform.parent.GetComponent<CanvasGroup>().alpha = lerpValue;
        }
        else {
            sprite.color = baseSpriteColor;
            healthBar.transform.parent.GetComponent<CanvasGroup>().alpha = 1;
        }

        // Player stats
        damagesUI.text = "" + weapon.projectileData.damage * weapon.damageMult;
        attackSpeedUI.text = "" + Math.Round(1 / ((weapon.data as Weapon.WeaponData).shotDelay * weapon.delayMult), 2);
        critChancesUI.text = Math.Round(weapon.projectileData.critChance * 100) + "%";
        evasionUI.text = Math.Round(Math.Tanh((data as CharacterData).evasion) * 100) + "%";
    }
}
