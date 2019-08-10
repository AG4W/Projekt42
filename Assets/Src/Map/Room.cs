using UnityEngine;

using Random = System.Random;
using System;

public class Room
{
    Tile[,] tiles;
    
    GameObject[] tilePrefabs;

    public int width { get; private set; }
    public int height { get; private set; }

    public int seed { get; private set; }

    public Random random { get; private set; }

    public Room(int minWidth, int maxWidth, int minHeight, int maxHeight, int seed = -1)
    {
        //om seed = -1 anges så tar vi nuvarande tick, borde vara random nog.
        this.seed = seed == -1 ? (int)DateTime.Now.Ticks : seed;

        //seeda vår random
        random = new Random(this.seed);

        //om vi laddar en gång slipper vi läsa ifrån hårddisk för varje tile
        //literally 1:width*height skillnad i performance
        tilePrefabs = Resources.LoadAll<GameObject>("Prefabs/Floors/");

        this.width = random.Next(minWidth, maxWidth);
        this.height = random.Next(minHeight, maxHeight);

        tiles = new Tile[width, height];
    }

    //genererar rummet, skapar inga visuella element
    public void Generate()
    {
        //för varje tile i x-led
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            //för varje tile i y-led
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                //skapa en ny tile
                tiles[x, y] = new Tile(x, y);

                //detta ser mer komplicerat ut än vad det är
                //om vi är på kanten, och i center för den kanten, skapa en dörr
                if (((x == 0 || x == tiles.GetLength(0) - 1) && y == height / 2) || (y == 0 || y == tiles.GetLength(1) - 1) && x == width / 2)
                    tiles[x, y].SetType(TileType.Door);
                //om vi är på kanten i x/y-led, stoppa spelaren ifrån att gå på dessa tiles
                else if (x == 0 || x == tiles.GetLength(0) - 1 || y == 0 || y == tiles.GetLength(1) - 1)
                    tiles[x, y].SetType(TileType.Wall);
                else
                    tiles[x, y].SetType(TileType.Floor);
            }
        }
    }

    //skapar visuella element
    public void Instantiate()
    {
        //för varje tile i x-led
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            //för varje tile i y-led
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                //skapa dess visuella representation (sprite)
                tiles[x, y].Instantiate(tilePrefabs[random.Next(0, tilePrefabs.Length)]);
            }
        }
    }

    //hämtar en specifik tile med koordinater
    public Tile Get(int x, int y)
    {
        return tiles[x, y];
    }
    public Tile Get(Vector2 position)
    {
        return tiles[(int)position.x, (int)position.y];
    }

    //hämtar en slumpmässig tile, buffrarna är x antal steg ifrån kanten (0 == hela rummet)
    public Tile GetRandom(int bufferX, int bufferY)
    {
        Tile t = null;

        while (t == null)
            t = Get(random.Next(bufferX, width - bufferX), random.Next(bufferY, height - bufferY));

        return t;
    }
    //hämtar en slumpmässig tile som är ledig
    public Tile GetRandomTraversable(int bufferX, int bufferY)
    {
        Tile t = GetRandom(bufferX, bufferY);

        while (!t.isTraversable)
            t = GetRandom(bufferX, bufferY);

        return t;
    }
    //tar emot en ingångsriktning och ger tillbaka en tile att spawna spelaren på
    public Tile GetSpawnPosition(Vector2 entranceDirection)
    {
        if (entranceDirection == Vector2.up)
            return tiles[width / 2, tiles.GetLength(1) - 2];
        else if (entranceDirection == Vector2.right)
            return tiles[tiles.GetLength(0) - 2, height / 2];
        else if (entranceDirection == Vector2.down)
            return tiles[width / 2, 1];
        else if (entranceDirection == Vector2.left)
            return tiles[1, height / 2];
        else
        {
            Debug.LogWarning("Otillåten ingångsvektor i GetSpawnPosition(), Room.cs!");
            return null;
        }
    }
    //tar emot en dörrtile och kollar vilket håll entrancedirection ska bli
    public Vector2 GetEntranceDirection(Tile tile)
    {
        if (tile == tiles[width / 2, tiles.GetLength(1) - 1])
            return Vector2.down;
        else if (tile == tiles[tiles.GetLength(0) - 1, height / 2])
            return Vector2.left;
        else if (tile == tiles[width / 2, 0])
            return Vector2.up;
        else if (tile == tiles[0, height / 2])
            return Vector2.right;
        else
        {
            Debug.LogWarning("Felaktig Tile i GetEntranceDirection(), Room.cs, " + tile.position);
            return Vector2.zero;
        }
    }
}
