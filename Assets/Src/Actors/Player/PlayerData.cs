using UnityEngine;
using UnityEngine.SceneManagement;

using Random = System.Random;
using System.Collections.Generic;

public static class PlayerData
{
    static UIManager ui;
    static bool newGame = true;

    static Stat health;
    static Stat damage;

    static Random random;

    public static int enemiesKilled { get; private set; }

    public static Weapon weapon { get; private set; }
    public static List<Item> items { get; private set; }

    public static Stat[] stats { get; private set; }

    public static void Initialize(int maxHealth)
    {
        ui = Object.FindObjectOfType<UIManager>();

        if (newGame)
        {
            newGame = false;
            random = new Random();
            items = new List<Item>();
            enemiesKilled = 0;

            InitializeStats(maxHealth);
            SetWeapon(Resources.LoadAll<Weapon>("Data/Items/Weapons/").RandomItem(random));
        }

        ui.UpdatePlayerHealthUI(GetStat(StatType.Health).current, GetStat(StatType.Health).max);
    }
    static void InitializeStats(int maxHealth)
    {
        health = new Stat(StatType.Health, maxHealth);
        damage = new Stat(StatType.Damage, 0);

        //denna array måste vara skapad i samma ordning som StatType-enumen
        //för att GetStats() ska fungera korrekt.
        stats = new Stat[] { health, damage };

        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].OnChanged += OnStatChanged;
            stats[i].OnZero += OnStatZero;
        }
    }

    static void OnStatChanged(Stat stat)
    {
        if (stat.type != StatType.Health)
            return;

        ui.UpdatePlayerHealthUI(stat.current, stat.max);
    }
    static void OnStatZero(Stat stat)
    {
        if (stat.type == StatType.Health)
            OnDeath();
    }
    static void OnDeath()
    {
        for (int i = 0; i < stats.Length; i++)
        {
            stats[i].OnChanged -= OnStatChanged;
            stats[i].OnZero -= OnStatZero;
        }

        newGame = true;

        SceneTransitionData.Reset();
        SceneManager.LoadScene("Menu");
    }

    public static int GetDamage()
    {
        int dmg = 0;
        //basskadan är vapnets randomroll
        int rdmg = weapon.GetDamage(random);

        dmg += rdmg;
        //lägg till damage modifiers
        dmg += GetStat(StatType.Damage).max;

        //cappa vid 0 så att vi inte healar fiender
        if (dmg < 0)
            dmg = 0;

        Debug.Log(dmg + ", (" + rdmg + " + " + GetStat(StatType.Damage).current + ")");
        return dmg;
    }

    public static void SetWeapon(Weapon weapon)
    {
        //cleara modifiers ifrån gamla vapnet
        if(PlayerData.weapon != null)
            RemovePermanentModifiers(PlayerData.weapon.permanentModifiers);

        PlayerData.weapon = weapon;
        AddPermanentModifiers(PlayerData.weapon.permanentModifiers);
    }
    public static void AddItems(params Item[] items)
    {
        PlayerData.items.AddRange(items);

        for (int i = 0; i < items.Length; i++)
            AddPermanentModifiers(items[i].permanentModifiers);
    }

    public static void OnEnemyKilled()
    {
        enemiesKilled++;
        Debug.Log("enemies killed: " + enemiesKilled);
    }

    public static Stat GetStat(StatType type)
    {
        return stats[(int)type];
    }

    public static void AddPermanentModifiers(params PermanentModifier[] pm)
    {
        if (pm == null)
            return;

        for (int i = 0; i < pm.Length; i++)
            GetStat(pm[i].type).AddModifier(pm[i]);
    }
    public static void RemovePermanentModifiers(params PermanentModifier[] pm)
    {
        if (pm == null)
            return;

        for (int i = 0; i < pm.Length; i++)
            GetStat(pm[i].type).RemoveModifier(pm[i]);
    }
}
