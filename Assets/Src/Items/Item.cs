using UnityEngine;

//för lite om hur scriptableobjects funkar
//https://unity3d.com/learn/tutorials/modules/beginner/live-training-archive/scriptable-objects

//namnet på itemet är själva namnet på det här objektet i editorn

[CreateAssetMenu(menuName = "Data/Items/Item")]
public class Item : ScriptableObject
{
    [SerializeField]Sprite sprite;

    [Tooltip("This is purely for comedic effect, it has no gameplay purpose")]
    [TextArea(3, 10)]
    [SerializeField]string description = "A description that describes this item.";

    [Tooltip("Add any non-permanent upgrades the player will receive when picking this item up.")]
    [SerializeField]int healthModifier;

    [Tooltip("Add any permanent upgrades the player will receive when picking this item up.")]
    [SerializeField]PermanentModifier[] modifiers;

    [Tooltip("Will the item be removed on pickup?")]
    [SerializeField]bool singleUse = false;

    public int hpModifier { get { return healthModifier; } }
    public PermanentModifier[] permanentModifiers { get { return modifiers; } }
    public bool isConsumable { get { return singleUse; } }

    public virtual string GetDescription()
    {
        string s = "";

        if (modifiers == null)
            return s;
        
        for (int i = 0; i < modifiers.Length; i++)
            s += "<color=" + (modifiers[i].value > 0 ? "green" : modifiers[i].value == 0 ? "yellow" : "red") + ">" + modifiers[i].value.ToString("+#;-#;") + "</color> " + modifiers[i].type + "\n";

        s += "\n<color=grey><i>" + description + "</i></color>";

        return s;
    }
    public Sprite GetSprite()
    {
        return sprite;
    }
}
