/*Vi lägger till permanenta upgrades här
 *Sen har vi en bool[] i PlayerData.cs
 *Den håller koll på vilka upgrades vi har, etc
 */

[System.Serializable]
public struct PermanentModifier
{
    public int value;
    public StatType type;
}