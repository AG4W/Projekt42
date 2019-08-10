using System.Collections.Generic;

public class Stat
{
    int baseValue;

    List<PermanentModifier> modifiers = new List<PermanentModifier>();

    public int max { get; private set; }
    public int current { get; private set; }

    public float inPercent { get { return current / max; } }

    public StatType type { get; private set; }

    public Stat(StatType type, int baseValue)
    {
        this.type = type;
        this.baseValue = baseValue;

        max = baseValue;
        current = baseValue;
    }

    public void UpdateCurrent(int amount)
    {
        current -= amount;

        if (current > max)
            current = max;
        else if (current <= 0)
            OnZero?.Invoke(this);

        OnChanged?.Invoke(this);
    }
    public void SetMax(int value, bool resetCurrent = false)
    {
        max = value;

        if (resetCurrent)
            current = max;

        OnChanged?.Invoke(this);
    }

    public void AddModifier(PermanentModifier pm)
    {
        modifiers.Add(pm);
        OnModifiersChanged();
    }
    public void RemoveModifier(PermanentModifier pm)
    {
        modifiers.Remove(pm);
        OnModifiersChanged();
    }
    void OnModifiersChanged()
    {
        max = baseValue;

        for (int i = 0; i < modifiers.Count; i++)
            max += modifiers[i].value;

        if (current > max)
            current = max;

        OnChanged?.Invoke(this);
    }

    public delegate void OnStatChanged(Stat stat);
    public event OnStatChanged OnChanged;
    public event OnStatChanged OnZero;
}
public enum StatType
{
    Health,
    Damage
}
