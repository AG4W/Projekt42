using UnityEngine;

using Random = System.Random;

[CreateAssetMenu(menuName = "Data/Items/Weapon")]
public class Weapon : Item
{
    [Tooltip("Everytime a damage value is needed, a random int between min/max is returned.")]
    [Header("Damage Values")]
    [SerializeField]int minDamage;
    [SerializeField]int maxDamage;

    public int GetDamage(Random random)
    {
        return random.Next(minDamage, maxDamage + 1);
    }

    public override string GetDescription()
    {
        string s = "";

        s += "Min Damage: <color=red>" + minDamage + "</color>\n";
        s += "Max Damage: <color=green>" + maxDamage + "</color>\n";
        s += "Average: <color=yellow>" + (minDamage + maxDamage) / 2f + "</color>\n\n";

        s += base.GetDescription();

        return s;
    }
}
