using UnityEngine;

public class Tile
{
    GameObject model;
    SpriteRenderer renderer;

    //position i 'gridden'
    public Vector2 position { get; private set; }
    public MapEntity entity { get; private set; }

    //avgör vad för visuell representation denna tile får när rummet spawnas
    //plus lite annat, typ ifall actors kan gå på den, vad som kan vara här etc
    public TileType type { get; private set; }
    //avgör om en actor kan gå på denna tile eller inte
    public bool isTraversable { get { return type == TileType.Floor; } }

    public Tile(int x, int y)
    {
        this.position = new Vector2(x, y);
    }

    //spawnar visuell representation av denna tile
    public GameObject Instantiate(GameObject prefab, Transform parent = null)
    {
        model = Object.Instantiate(prefab, position, Quaternion.identity, parent);
        renderer = model.GetComponentInChildren<SpriteRenderer>();

        SetColor(GetColor(type));
        return model;
    }

    public void SetType(TileType type)
    {
        this.type = type;

        if (renderer != null)
            SetColor(GetColor(type));
    }
    public void SetEntity(MapEntity entity)
    {
        this.entity = entity;
    }
    void SetColor(Color color)
    {
        renderer.material.color = color;
    }

    public static Color GetColor(TileType type)
    {
        switch (type)
        {
            case TileType.Floor:
                return Color.white;
            case TileType.Door:
                return Color.green;
            case TileType.Wall:
                return Color.red;
            case TileType.Occupied:
                return Color.red;
            case TileType.Npc:
                return Color.red;
            case TileType.Player:
                return Color.yellow;
            default:
                Debug.LogWarning("TileType " + type.ToString() + " is missing a switch case in GetColor(), Tile.cs, plz fix!");
                return Color.magenta;
        }
    }
}
public enum TileType
{
    Floor,
    Door,
    Wall,
    Occupied,
    Npc,
    Player
}