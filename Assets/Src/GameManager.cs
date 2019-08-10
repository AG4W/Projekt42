using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public static class GameManager
{
    static int turnIndex;

    static List<ActorBehaviour> actors;

    public static PlayerBehaviour player { get; private set; }
    public static Room room { get; private set; }

    public static void Initialize(int minWidth, int maxWidth, int minHeight, int maxHeight)
    {
        //skapa rum
        room = new Room(minWidth, maxHeight, minHeight, maxHeight);

        room.Generate();
        room.Instantiate();

        actors = new List<ActorBehaviour>();

        CreateObstacles();
        CreateFurniture();
        //spawna spelare bredvid den vänstra dörren
        CreatePlayer();
        CreateEnemies();

        CreateInteractables();

        Start();
    }

    static void Start()
    {
        turnIndex = 0;
        actors[turnIndex].StartTurn();
    }
    public static void EndTurn()
    {
        if (turnIndex < actors.Count - 1)
            turnIndex++;
        else
            turnIndex = 0;

        actors[turnIndex].StartTurn();
        player.UpdateUI();
    }

    public static void AddActor(ActorBehaviour actor)
    {
        actors.Add(actor);
    }
    public static void RemoveActor(ActorBehaviour actor)
    {
        actors.Remove(actor);
    }

    static void CreateObstacles()
    {
        //använd rumstorleken för att skala antalet innerväggar med rumsstorleken
        int innerWallCount = room.random.Next(1, room.width / 3);

        for (int i = 0; i < innerWallCount; i++)
        {
            Tile t = room.GetRandomTraversable(2, 2);
            t.SetType(TileType.Occupied);
        }
    }
    static void CreateFurniture()
    {
        GameObject[] lights = Resources.LoadAll<GameObject>("Prefabs/Furniture/Lights/");

        //vi vill alltid ha minst ett ljus
        //använd rumstorleken för att skala antalet ljus med rumsstorleken
        int lightCount = room.random.Next(1, room.width / 3);

        if (lightCount > 3)
            lightCount = 3;

        for (int i = 0; i < lightCount; i++)
        {
            Tile t = room.GetRandomTraversable(2, 2);
            t.SetType(TileType.Occupied);

            Object.Instantiate(lights.RandomItem(room.random), t.position, Quaternion.identity, null);
        }
    }

    static void CreatePlayer()
    {
        //hitta vår spawnposition
        Tile start = room.GetSpawnPosition(SceneTransitionData.entranceDirection);

        //spawna player sprite
        GameObject p = Object.Instantiate(Resources.Load<GameObject>("player"));

        //sätt position på vår spelares actor
        PlayerBehaviour pb = p.GetComponent<PlayerBehaviour>();
        pb.SetPosition(start);

        //lägg till i turlistan
        AddActor(pb);
        player = pb;
        //sätt kamerans followtarget till spelaren
        Object.FindObjectOfType<CameraManager>().SetTarget(p);
    }
    static void CreateEnemies()
    {
        // Sätter till ett slumpmässigt värde mellan 1-4
        int enemyCount = room.random.Next(1, 2 + 1);

        // modifiera antalet fiender, med antalet fiender spelaren har dödat för att göra spelet svårare
        // (lägg till (PlayerData.enemiesKilled / 20) per rum
        enemyCount += PlayerData.enemiesKilled / 20;

        // Laddar in fiender från minnet
        GameObject[] enemies = Resources.LoadAll<GameObject>("Prefabs/Enemies/");

        // Börjar på noll, körs så länge i är mindre än enemyCount, och slutar när i är lika med enemyCount
        // För varje steg i loopen läggs +1 på i
        for (int i = 0; i < enemyCount; i++)
        {
            // Hämtar en random tile som går att gå på
            Tile s = room.GetRandomTraversable(2, 2);

            // Tar en slumpmässig fiende-prefab och kopierar mallen till positionen från tilen ovan
            GameObject e = Object.Instantiate(enemies[room.random.Next(0,enemies.Length)], s.position, Quaternion.identity, null);

            ActorBehaviour ab = e.GetComponent<ActorBehaviour>();
            actors.Add(ab);
            ab.SetPosition(s);
        }
    }

    static void CreateInteractables()
    {
        //chansen att en permanent upgrade spawnar
        //i %
        int upgradeChance = 25;

        if(room.random.Next(0, 100 + 1) <= upgradeChance)
        {
            Tile s = room.GetRandomTraversable(2, 2);

            GameObject e = Object.Instantiate(Resources.LoadAll<GameObject>("Prefabs/Items/Upgrades/").RandomItem(room.random), s.position, Quaternion.identity, null);
            ItemEntity ie = e.GetComponent<ItemEntity>();

            ie.SetPosition(s);
            s.SetType(TileType.Occupied);
        }

        int consumableCount = room.random.Next(0, 3 + 1);

        GameObject[] consumables = Resources.LoadAll<GameObject>("Prefabs/Items/Consumables/");

        for (int i = 0; i < consumableCount; i++)
        {
            Tile s = room.GetRandomTraversable(2, 2);
            GameObject e = Object.Instantiate(consumables.RandomItem(room.random), s.position, Quaternion.identity, null);
            ItemEntity ie = e.GetComponent<ItemEntity>();

            ie.SetPosition(s);
            s.SetType(TileType.Occupied);
        }
    }
}
