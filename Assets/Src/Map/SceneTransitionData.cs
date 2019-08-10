using UnityEngine;

/// <summary>
/// Denna klass används för att hålla data som behöver överleva mellan scener/reloads.
/// </summary>
public static class SceneTransitionData
{
    public static int lastRoomSeed { get; private set; } = -1;
    public static Vector2 entranceDirection { get; private set; } = Vector2.right;

    public static void SetLastRoomSeed(int seed)
    {
        lastRoomSeed = seed;
    }
    public static void SetEntranceDirection(Vector2 direction)
    {
        entranceDirection = direction;
    }

    public static void Reset()
    {
        lastRoomSeed = -1;
        entranceDirection = Vector2.right;
    }
}
