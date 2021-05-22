using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public enum HpChangesType { normalDamages, criticalDamages, bleedDamages, fireDamages, poisonDamages, normalHeal, lifeLeechHeal, Evasion }

public class Character : Powerupable {
    public Rigidbody2D rb;
    protected float HP;
    protected bool isHumanPlayer = false;

    System.Random rand = new System.Random();

    private List<KeyValuePair<float, HpChangesType>> updateHpChanges = new List<KeyValuePair<float, HpChangesType>>();
    private List<KeyValuePair<float, float>> hpChangesHistory = new List<KeyValuePair<float, float>>();
    private float hpChangesHistoryTotal = 0;

    // Invincibility variables
    protected bool isInvicible = false;
    protected float invincibilityEndTime;
    protected float invincibilityHistoryDuration = 10;
    protected float invincibilityDuration = 2;
    protected float hpLossThresholdForInvincibility; // Value set in Start()


    [System.Serializable]
    public class CharacterData : PowerupableData {
        public float maxHP = 100;
        public float maxShield = 0;
        public float shieldPerRoom = 5;
        public float speed = 5f;
        public float evasion = 0;

        public override void Add(PowerupableData d) {
            if (!(d is CharacterData)) return;
            CharacterData cd = (CharacterData) d;

            maxHP += cd.maxHP;
            maxShield += cd.maxShield;
            shieldPerRoom += cd.shieldPerRoom;
            speed += cd.speed;
            evasion += cd.evasion;

            base.Add(d);
        }

        public override void Mul(PowerupableData d) {
            if (!(d is CharacterData)) return;
            CharacterData cd = (CharacterData) d;

            maxHP *= cd.maxHP;
            maxShield *= cd.maxShield;
            shieldPerRoom *= cd.shieldPerRoom;
            speed *= cd.speed;
            evasion *= cd.evasion;

            base.Mul(d);
        }

        public override void Set(PowerupableData d) {
            if (!(d is CharacterData)) return;
            CharacterData cd = (CharacterData) d;

            if (cd.maxHP != -999) maxHP = cd.maxHP;
            if (cd.maxShield != -999) maxShield = cd.maxShield;
            if (cd.shieldPerRoom != -999) shieldPerRoom = cd.shieldPerRoom;
            if (cd.speed != -999) speed = cd.speed;
            if (cd.evasion != -999) evasion = cd.evasion;

            base.Set(d);
        }

        public override void Check() {
            if (maxHP < 1) maxHP = 1;
            if (maxShield < 0) maxShield = 0;
            if (shieldPerRoom < 5) shieldPerRoom = 5;
            if (speed < 1) speed = 1;
            if (evasion < 0) evasion = 0;
        }

        public override PowerupableData Clone() {
            CharacterData ans = new CharacterData();
            ans.maxHP = maxHP;
            ans.maxShield = maxShield;
            ans.speed = speed;
            ans.evasion = evasion;
            return ans;
        }

    }

    public GameObject floatingDamagesIndicator;

    public float MoveSpeedMult = 1;

    public Dictionary<string, OnHit> onHits = new Dictionary<string, OnHit>();
    public Dictionary<string, OnCrit> onCrits = new Dictionary<string, OnCrit>();


    [SerializeField] private CharacterData cd;

    // Start is called before the first frame update
    public virtual void Start() {
        SetData(cd);
        rb = transform.GetComponent<Rigidbody2D>();
        HP = cd.maxHP;
        hpLossThresholdForInvincibility = 0.5f * cd.maxHP;
    }

    // Update is called once per frame
    public virtual void Update() {
        if (rb.velocity.x < -0.5f) {
            Vector3 scale = transform.localScale;
            scale.x = -1f;
            transform.localScale = scale;
        }
        else if (rb.velocity.x > 0.5f) {
            Vector3 scale = transform.localScale;
            scale.x = 1f;
            transform.localScale = scale;
        }
        CheckForHpChanges();
    }

    public void Move(Vector3 direction) {
        if (direction.magnitude > 1) direction.Normalize();
        rb.velocity = (data as CharacterData).speed * direction;
    }

    public void Move(Vector3 direction, float speed)
    {
        if (direction.magnitude > 1) direction.Normalize();
        rb.velocity = speed * MoveSpeedMult * direction;
    }

    public virtual void Damage(float value, HpChangesType damageType) {
        if (!isInvicible)
            if (rand.Next(100) > Math.Tanh((data as CharacterData).evasion) * 100) //Evasion
                updateHpChanges.Add(new KeyValuePair<float, HpChangesType>(-value, damageType));
            else
                updateHpChanges.Add(new KeyValuePair<float, HpChangesType>(0, HpChangesType.Evasion));
    }

    public void Heal(float value, HpChangesType healType) {
        updateHpChanges.Add(new KeyValuePair<float, HpChangesType>(value, healType));
    }

    public void AddOnHit(OnHit onHit) {
        string title = onHit.title;

        if (onHits.ContainsKey(title)) {
            onHits[title].AddStack();
        }
        else {
            onHits.Add(title, onHit);
        }
    }

    public void AddOnCrit (OnCrit onCrit) {
        string title = onCrit.title;

        if (onCrits.ContainsKey(title)) {
            onCrits[title].AddStack();
        } else {
            onCrits.Add(title, onCrit);
        }
    }

    public void AddStatusEffect(StatusEffect statusEffect, GameObject caster) {
        if (statusEffect == null || caster == null) return;

        string className = statusEffect.GetType().Name;
        System.Type type = statusEffect.GetType();

        StatusEffect c = (StatusEffect) GetComponent(className);
        if (c == null) {
            this.gameObject.SetActive(false);
            c = (StatusEffect) gameObject.AddComponent(type);
            c.caster = caster;
            this.gameObject.SetActive(true);
        }
        else
            c.OnApply();
        c.enabled = true;
    }

    public virtual void Die() {
        Destroy(gameObject);
    }

    private void CheckForHpChanges() {
        float time = Time.time;

        if (isInvicible && time > invincibilityEndTime) { // Invincibility is over
            isInvicible = false;
            //Debug.Log("Player invincibility off");
        }

        float updateHpChange = 0;

        bool playHealSound = false;
        bool playHitSound = false;
        bool playCritSound = false;
        bool playBleedSound = false;
        bool playFireSound = false;
        bool playPoisonSound = false;
        bool playShieldBlockSound = false;

        // Check for HP gain/loss
        foreach (KeyValuePair<float, HpChangesType> pair in updateHpChanges) {
            updateHpChange += pair.Key;

            int floatingNumberValue = (int) Math.Round(pair.Key);

            // Get the GameObjects and Components
            GameObject floatingIndicator = Instantiate(floatingDamagesIndicator, transform.position, Quaternion.identity) as GameObject;
            TextMeshPro textMesh = floatingIndicator.transform.Find("Text").GetComponent<TextMeshPro>();
            Transform transf = floatingIndicator.transform.Find("Text").GetComponent<Transform>();

            // Change the text
            textMesh.text = floatingNumberValue.ToString();

            // Change the scale (more damage = bigger indicator)
            float scale = Math.Max(0.8f, Math.Min(2.0f, (float) Math.Log10(Math.Abs(floatingNumberValue)))); // Log10 value between 0.8 and 2.0
            transf.localScale = new Vector3(scale, scale, scale);

            // Add a '+' before the value if it is positive
            if (floatingNumberValue >= 0)
                textMesh.text = "+" + textMesh.text;

            switch(pair.Value) {
                // Damages
                case HpChangesType.normalDamages:
                    textMesh.color = new Color(0.8f, 0.8f, 0.8f); playHitSound = true; break; // White-ish = normal damages
                case HpChangesType.criticalDamages:
                    textMesh.color = new Color(0.8f, 0.8f, 0); playCritSound = true; break; // Yellow = critical damages
                case HpChangesType.bleedDamages:
                    textMesh.color = new Color(0.7f, 0, 0); playBleedSound = true; break; // Red = bleed damages
                case HpChangesType.fireDamages:
                    textMesh.color = new Color(0.8f, 0.5f, 0); playFireSound = true; break; // Orange = fire damages
                case HpChangesType.poisonDamages:
                    textMesh.color = new Color(0.45f, 0.25f, 0.55f); playPoisonSound = true; break; // Purple = poison damages

                // Heals
                case HpChangesType.normalHeal:
                    textMesh.color = new Color(0.2f, 0.6f, 0.2f); playHealSound = true; break; // Green = normal heal
                case HpChangesType.lifeLeechHeal:
                    textMesh.color = new Color(0.2f, 0.6f, 0.2f); playHealSound = false; break; // Green = life leech heal

                //Evasion
                case HpChangesType.Evasion:
                    textMesh.color = new Color(0.6f, 0.6f, 0.6f); textMesh.text = "Blocked"; playShieldBlockSound = true; break;
            }
        }

        updateHpChanges.Clear();

        // Play the SFX
        if (isHumanPlayer) {
            if (playHitSound) AudioManager.instance.Play(SFX.PlayerHit);
            if (playHealSound) AudioManager.instance.Play(SFX.PlayerHealingPotion);
            if (playShieldBlockSound) AudioManager.instance.Play(SFX.ShieldBlock);
            // Add the others Play() when the corresponding SFX will be added
        }
        else {
            if (playHitSound) AudioManager.instance.Play(SFX.MobHit);
            // Add the others Play() when the corresponding SFX will be added
        }

        if (updateHpChange == 0)
            return;

        // Player only, check if invincibility and limit the damages taken
        if (isHumanPlayer && !isInvicible) {
            HP += CheckForInvincibility(updateHpChange, time);
        }
        else {
            HP += updateHpChange;
        }

        if (HP <= 0) {
            HP = 0;
            isInvicible = false;
            Die();
        }
        if (HP > (data as CharacterData).maxHP) {
            HP = (data as CharacterData).maxHP;
        }
    }

    /**
     * Check if an invincibility of 1s should be triggered for a player that has taken more than 50% of his max HP in the last 2 seconds.
     * Return the damages taken for the current update.
     */
    private float CheckForInvincibility(float updateHpChange, float time) {
        // Update the list by removing the old entries (> 2s ago)
        int oldEntriesIndex = -1;
        foreach (KeyValuePair<float, float> pair in hpChangesHistory.ToArray()) {
            //float pairHpChanges = pair.Key;
            float pairTime = pair.Value;

            if (time - pairTime >= invincibilityHistoryDuration) { // Damages are too old (taken more than 2s ago)
                oldEntriesIndex++;
            }
            else { // All entries from this one are recent (less than 2s ago), keep them
                break;
            }
        }
        // Remove the old entries
        for (int i = 0; i <= oldEntriesIndex; i++) {
            hpChangesHistoryTotal -= hpChangesHistory[0].Key;
            hpChangesHistory.RemoveAt(0);
        }

        // Add the hpChanges from the current frame
        hpChangesHistory.Add(new KeyValuePair<float, float>(updateHpChange, time));
        hpChangesHistoryTotal += updateHpChange;

        // Apply the invincibility if the conditions are met
        if (-hpChangesHistoryTotal >= hpLossThresholdForInvincibility) {
            isInvicible = true;

            // Limit the updateHpChange so the damages taken in the last 2s don't exceed 50% of the player's max HP
            updateHpChange -= hpChangesHistoryTotal + hpLossThresholdForInvincibility;

            hpChangesHistory.Clear();
            hpChangesHistoryTotal = 0;
            invincibilityEndTime = time + invincibilityDuration; // Invincible for invincibilityDuration seconds
            //Debug.Log("Player invincibility triggered for " + invincibilityDuration + " seconds.");
        }

        return updateHpChange;
    }

    public void SetSpeed (float speed) {
        (data as CharacterData).speed = speed;
    }
}