﻿using Assets.Scripts.IAJ.Unity.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Game.NPCs;
using Assets.Scripts.IAJ.Unity.Formations;
using Assets.Scripts.Game;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static class GameConstants
    {
        public const float UPDATE_INTERVAL = 2.0f;
        public const int TIME_LIMIT = 150;
        public const int PICKUP_RANGE = 8;

    }
    //public fields, seen by Unity in Editor

    public AutonomousCharacter Character;

    [Header("UI Objects")]
    public Text HPText;
    public Text ShieldHPText;
    public Text ManaText;
    public Text TimeText;
    public Text XPText;
    public Text LevelText;
    public Text MoneyText;
    public Text DiaryText;
    public GameObject GameEnd;

    [Header("Enemy Settings")]
    public bool SleepingNPCs;
    public bool BehaviourTreeNPCs;
    public bool StochasticWorld = false;

    //fields
    public List<GameObject> chests { get; set; }
    public List<GameObject> healthPotions { get; set; }
    public List<GameObject> manaPotions { get; set; }
    public List<GameObject> skeletons { get; set; }
    public List<GameObject> orcs { get; set; }
    public List<GameObject> dragons { get; set; }
    public List<GameObject> enemies { get; set; }
    public List<FormationManager> Formations { get; set; }
    public Dictionary<string, List<GameObject>> disposableObjects { get; set; }
    public int InitialDisposableObjectsCount { get; set; }
    public bool WorldChanged { get; set; }

    private float nextUpdateTime = 0.0f;
    private float enemyAttackCooldown = 0.0f;
    public bool gameEnded { get; set; } = false;
    public Vector3 initialPosition { get; set; }

    public bool lineFormation = false;
    public bool triangleFormation = false;

    public int runCounter = 0;
    public int deathCounter = 0;
    public int timeoutCounter = 0;
    public int winCounter = 0;

    void Awake()
    {
        InitializeGame();
    }

    private void InitializeGame() {
        Instance = this;
        UpdateDisposableObjects();
        this.InitialDisposableObjectsCount = this.disposableObjects.Count;
        this.WorldChanged = false;
        this.Character = GameObject.FindGameObjectWithTag("Player").GetComponent<AutonomousCharacter>();

        List<Monster> monsters = new List<Monster>();
        if (triangleFormation) {
            monsters.Add(GameObject.Find("TriangleFormationAnchor").GetComponent<OrcAnchorPoint>());
        }
        GameObject leader = GameObject.Find("Orc5");
        if (leader != null) {
            monsters.Add(leader.GetComponent<Orc>());
            monsters.Add(GameObject.Find("Orc4").GetComponent<Orc>());
            monsters.Add(GameObject.Find("Orc3").GetComponent<Orc>());
        }
        Formations = new List<FormationManager>();
        if (lineFormation) {
            Formations.Add(new FormationManager(monsters, new LineFormation(), leader.transform.position, leader.transform.forward));
        }
        else if (triangleFormation) {
            Formations.Add(new FormationManager(monsters, new TriangleFormation(), GameObject.Find("TriangleFormationAnchor").transform.position, GameObject.Find("TriangleFormationAnchor").transform.forward));
        }

        this.initialPosition = this.Character.gameObject.transform.position;
        gameEnded = false;
    }

    public void UpdateDisposableObjects()
    {
        this.enemies = new List<GameObject>();
        this.disposableObjects = new Dictionary<string, List<GameObject>>();
        this.chests = GameObject.FindGameObjectsWithTag("Chest").ToList();
        this.healthPotions = GameObject.FindGameObjectsWithTag("HealthPotion").ToList();
        this.manaPotions = GameObject.FindGameObjectsWithTag("ManaPotion").ToList();
        this.skeletons = GameObject.FindGameObjectsWithTag("Skeleton").ToList();
        this.orcs = GameObject.FindGameObjectsWithTag("Orc").ToList();
        this.dragons = GameObject.FindGameObjectsWithTag("Dragon").ToList();
        this.enemies.AddRange(this.skeletons);
        this.enemies.AddRange(this.orcs);
        this.enemies.AddRange(this.dragons);

     
        //adds all enemies to the disposable objects collection
        foreach (var enemy in this.enemies)
        {
            if (disposableObjects.ContainsKey(enemy.name))
            {
                this.disposableObjects[enemy.name].Add(enemy);
            }
            else this.disposableObjects.Add(enemy.name, new List<GameObject>() { enemy });
        }
        //add all chests to the disposable objects collection
        foreach (var chest in this.chests)
        {
            if (disposableObjects.ContainsKey(chest.name))
            {
                this.disposableObjects[chest.name].Add(chest);
            }
            else this.disposableObjects.Add(chest.name, new List<GameObject>() { chest });
        }
        //adds all health potions to the disposable objects collection
        foreach (var potion in healthPotions)
        {
            if (disposableObjects.ContainsKey(potion.name))
            {
                this.disposableObjects[potion.name].Add(potion);
            }
            else this.disposableObjects.Add(potion.name, new List<GameObject>() { potion });
        }
        //adds all mana potions to the disposable objects collection
        foreach (var potion in manaPotions)
        {
            if (disposableObjects.ContainsKey(potion.name))
            {
                this.disposableObjects[potion.name].Add(potion);
            }
            else this.disposableObjects.Add(potion.name, new List<GameObject>() { potion });
        }
    }

    void FixedUpdate()
    {
        if (!this.gameEnded)
        {
            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + GameConstants.UPDATE_INTERVAL;
                this.Character.baseStats.Time += GameConstants.UPDATE_INTERVAL;
            }

            foreach (FormationManager formation in Formations) {
                formation.UpdateSlots();
            }

            this.HPText.text = "HP: " + this.Character.baseStats.HP;
            this.XPText.text = "XP: " + this.Character.baseStats.XP;
            this.ShieldHPText.text = "Shield HP: " + this.Character.baseStats.ShieldHP;
            this.LevelText.text = "Level: " + this.Character.baseStats.Level;
            this.TimeText.text = "Time: " + this.Character.baseStats.Time;
            this.ManaText.text = "Mana: " + this.Character.baseStats.Mana;
            this.MoneyText.text = "Money: " + this.Character.baseStats.Money;

            if (this.Character.baseStats.HP <= 0 || this.Character.baseStats.Time >= GameConstants.TIME_LIMIT)
            {
                this.gameEnded = true;
                WorldChanged = true;
            }
            else if (this.Character.baseStats.Money >= 25)
            {
                this.gameEnded = true;
                WorldChanged = true;
            }
        }
        else {
            RestartGame();
            gameEnded = false;
        }
    }

    private void RestartGame()  {
        runCounter++;

        foreach (GameObject enemy in this.enemies) {
            enemy.SetActive(true);
            enemy.GetComponent<NPC>().Restart();
        }
        foreach (GameObject chest in this.chests) {
            chest.SetActive(true);
            chest.GetComponent<Consumable>().Reset();
        }
        foreach (GameObject potion in healthPotions) {
            potion.SetActive(true);
            potion.GetComponent<Consumable>().Reset();
        }
        foreach (GameObject potion in manaPotions) {
            potion.SetActive(true);
            potion.GetComponent<Consumable>().Reset();
        }
        this.Character.Restart();

    }

    public void RemoveOrcFromFormation(Monster enemy)
    {
        foreach (FormationManager formation in Formations) {
            formation.SlotAssignment.Remove(enemy);
        }
    }

    public void BreakFormations()
    {
        foreach (FormationManager formation in Formations) {
            formation.BreakFormation();
        }

        Formations.Clear();
    }

    public void SwordAttack(GameObject enemy)
    {
        int damage = 0;

        Monster monster = enemy.GetComponent<Monster>();
        Monster.EnemyStats enemyData = enemy.GetComponent<Monster>().enemyStats;
        
        if (enemy != null && enemy.activeSelf && InMeleeRange(enemy))
        {
            this.Character.AddToDiary(" I Sword Attacked " + enemy.name);

            if (this.StochasticWorld)
            {
                damage = enemy.GetComponent<Monster>().DmgRoll.Invoke();

                //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
                int attackRoll = RandomHelper.RollD20() + 7;

                if (attackRoll >= enemyData.AC)
                {
                    //there was an hit, enemy is destroyed, gain xp
                    RemoveOrcFromFormation(monster);
                    enemy.SetActive(false);
                }
            }
            else
            {
                damage = enemyData.SimpleDamage;
                RemoveOrcFromFormation(monster);
                enemy.SetActive(false);
            }

            this.Character.baseStats.XP += enemyData.XPvalue;

            int remainingDamage = damage - this.Character.baseStats.ShieldHP;
            this.Character.baseStats.ShieldHP = Mathf.Max(0, this.Character.baseStats.ShieldHP - damage);

            if (remainingDamage > 0)
            {
                this.Character.baseStats.HP -= remainingDamage;
            }

            this.WorldChanged = true;
        }
    }

    public void EnemyAttack(GameObject enemy)
    {
        if (Time.time > this.enemyAttackCooldown)
        {

            int damage = 0;

            Monster monster = enemy.GetComponent<Monster>();

            if (enemy.activeSelf && monster.InWeaponRange(GameObject.FindGameObjectWithTag("Player")))
            {

                this.Character.AddToDiary(" I was Attacked by " + enemy.name);
                this.enemyAttackCooldown = Time.time + GameConstants.UPDATE_INTERVAL;

                if (this.StochasticWorld)
                {
                    damage = monster.DmgRoll.Invoke();

                    //attack roll = D20 + attack modifier. Using 7 as attack modifier (+4 str modifier, +3 proficiency bonus)
                    int attackRoll = RandomHelper.RollD20() + 7;

                    if (attackRoll >= monster.enemyStats.AC)
                    {
                        //there was an hit, enemy is destroyed, gain xp
                        RemoveOrcFromFormation(monster);
                        enemy.SetActive(false);
                    }
                }
                else
                {
                    damage = monster.enemyStats.SimpleDamage;
                    RemoveOrcFromFormation(monster);
                    enemy.SetActive(false);
                }

                this.Character.baseStats.XP += monster.enemyStats.XPvalue;

                int remainingDamage = damage - this.Character.baseStats.ShieldHP;
                this.Character.baseStats.ShieldHP = Mathf.Max(0, this.Character.baseStats.ShieldHP - damage);

                if (remainingDamage > 0)
                {
                    this.Character.baseStats.HP -= remainingDamage;
                    this.Character.AddToDiary(" I was wounded with " + remainingDamage + " damage");
                }

                this.WorldChanged = true;
            }
        }
    }

    public void DivineSmite(GameObject enemy)
    {
        if (enemy != null && enemy.activeSelf && InDivineSmiteRange(enemy) && this.Character.baseStats.Mana >= 2)
        {
            if (enemy.CompareTag("Skeleton"))
            {
                this.Character.baseStats.XP += 3;
                this.Character.AddToDiary(" I Smited " + enemy.name);
                enemy.SetActive(false);
            }
            this.Character.baseStats.Mana -= 2;

            this.WorldChanged = true;
        }
    }

    public void ShieldOfFaith()
    {
        /* this.GetComponent<AudioSource>().Play(); */
        if (this.Character.baseStats.Mana >= 5)
        {
            this.Character.baseStats.ShieldHP = 5;
            this.Character.baseStats.Mana -= 5;
            this.Character.AddToDiary(" My Shield of Faith will protect me!");
            this.WorldChanged = true;
        }
    }

    public void PickUpChest(GameObject chest)
    {

        if (chest != null && chest.activeSelf && InChestRange(chest))
        {
            this.Character.AddToDiary(" I opened  " + chest.name);
            chest.SetActive(false);
            this.Character.baseStats.Money += 5;
            this.WorldChanged = true;
        }
    }


    public void GetManaPotion(GameObject manaPotion)
    {
        if (manaPotion != null && manaPotion.activeSelf && InPotionRange(manaPotion))
        {
            this.Character.AddToDiary(" I drank " + manaPotion.name);
            manaPotion.SetActive(false);
            this.Character.baseStats.Mana = 10;
            this.WorldChanged = true;
        }
    }

    public void GetHealthPotion(GameObject potion)
    {
        if (potion != null && potion.activeSelf && InPotionRange(potion))
        {
            this.Character.AddToDiary(" I drank " + potion.name);
            potion.SetActive(false);
            this.Character.baseStats.HP = this.Character.baseStats.MaxHP;
            this.WorldChanged = true;
        }
    }

    public void LevelUp()
    {
        if (this.Character.baseStats.Level >= 4) return;

        if (this.Character.baseStats.XP >= this.Character.baseStats.Level * 10)
        {
            if (!this.Character.LevelingUp)
            {
                this.Character.AddToDiary(" I am trying to level up...");
                this.Character.LevelingUp = true;
                this.Character.StopTime = Time.time + AutonomousCharacter.LEVELING_INTERVAL;
            }
            else if (this.Character.StopTime < Time.time)
            { 
                this.Character.baseStats.Level++;
                this.Character.baseStats.MaxHP += 10;
                this.Character.baseStats.XP = 0;
                this.Character.AddToDiary(" I leveled up to level " + this.Character.baseStats.Level);
                this.Character.LevelingUp = false;
                this.WorldChanged = true;
            }
        }
    }

    public void LayOnHands()
    {
        if (this.Character.baseStats.Level >= 2 && this.Character.baseStats.Mana >= 7)
        {
            this.Character.AddToDiary(" With my Mana I Lay Hands and recovered all my health.");
            this.Character.baseStats.HP = this.Character.baseStats.MaxHP;
            this.Character.baseStats.Mana -= 7;
            this.WorldChanged = true;
        }
    }

    public void DivineWrath()
    {
        if (this.Character.baseStats.Level >= 3 && this.Character.baseStats.Mana >= 10)
        {
            //kill all enemies in the map
            foreach (var enemy in this.enemies)
            {
                this.Character.baseStats.XP += enemy.GetComponent<Monster>().enemyStats.XPvalue;
                this.Character.AddToDiary(" I used the Divine Wrath and all monsters were killed! \nSo ends a day's work...");
                enemy.SetActive(false);
                this.disposableObjects.Remove(enemy.name);
            }

            enemies.Clear();
            this.WorldChanged = true;
        }
    }

    public void Rest()
    {
        if (!this.Character.Resting)
        {
            this.Character.AddToDiary(" I am resting");
            this.Character.Resting = true;
            this.Character.StopTime = Time.time + AutonomousCharacter.RESTING_INTERVAL;
        }
        else if (this.Character.StopTime < Time.time)
        {
            this.Character.baseStats.HP += AutonomousCharacter.REST_HP_RECOVERY;
            this.Character.baseStats.HP = Mathf.Min(this.Character.baseStats.HP, this.Character.baseStats.MaxHP);
            this.Character.Resting = false;
            this.WorldChanged = true;
        }
    }

    public void Teleport()
    {
        if (this.Character.baseStats.Level >= 2 && this.Character.baseStats.Mana >= 5)
        {
            this.Character.AddToDiary(" With my Mana I teleported away from danger.");
            this.Character.transform.position = this.initialPosition;
            this.Character.baseStats.Mana -= 5;
            this.WorldChanged = true;
        }

    }


    private bool CheckRange(GameObject obj, float maximumDistance)
    {
        var distance = (obj.transform.position - this.Character.gameObject.transform.position).sqrMagnitude;
        return distance <= maximumDistance;
    }


    public bool InMeleeRange(GameObject enemy)
    {
        return this.CheckRange(enemy, GameConstants.PICKUP_RANGE);
    }

    public bool InDivineSmiteRange(GameObject enemy)
    {
        return this.CheckRange(enemy, GameConstants.PICKUP_RANGE * 10);
    }

    public bool InChestRange(GameObject chest)
    {

        return this.CheckRange(chest, GameConstants.PICKUP_RANGE);
    }

    public bool InPotionRange(GameObject potion)
    {
        return this.CheckRange(potion, GameConstants.PICKUP_RANGE);
    }

}
