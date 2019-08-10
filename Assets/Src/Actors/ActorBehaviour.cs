using UnityEngine;

public class ActorBehaviour : MapEntity
{
    [SerializeField]AudioClip[] hitsounds;

    public bool hasTurn { get; private set; }

    public virtual void Move(Vector2 direction)
    {
        //fångar ett error, detta borde dock aldrig kunna hända
        //sålänge inte något gått tokigt i GameManagers turn-control.
        if (!hasTurn)
        {
            Debug.Log("Not my turn!");
            return;
        }

        Tile nt = GetTileFromDirection(direction);

        if (!nt.isTraversable)
        {
            //ifall det är spelaren som försöker göra detta borde vi lägga in 
            //någon arg screenshake eller något
            //eller ett errorljud, osv
            Debug.Log("I can't go there!");
            return;
        }

        //återställ gamla
        if (base.tile != null)
            base.tile.SetType(TileType.Floor);

        base.MoveToPosition(nt);
    }

    protected Tile GetTileFromDirection(Vector2 direction)
    {
        int x = (int)(tile.position.x + direction.x);
        int y = (int)(tile.position.y + direction.y);

        return GameManager.room.Get(x, y);
    }

    public override void SetPosition(Tile tile)
    {
        if (base.tile != null)
            base.tile.SetType(TileType.Floor);

        base.SetPosition(tile);
        base.tile.SetType(TileType.Occupied);
    }

    public virtual void UpdateHealth(int amount)
    {
        if(hitsounds != null && hitsounds.Length > 0)
            AudioManager.Play(SFXType.Oneshot, hitsounds.RandomItem(null), Random.Range(.75f, 1.25f));
    }
    protected virtual void OnDeath()
    {
        base.tile.SetEntity(null);
        base.tile.SetType(TileType.Floor);

        GameManager.RemoveActor(this);
        Destroy(this.gameObject, 1.0f);
    }

    public virtual void StartTurn()
    {
        hasTurn = true;
    }
    public void EndTurn()
    {
        hasTurn = false;

        GameManager.EndTurn();
    }
}
