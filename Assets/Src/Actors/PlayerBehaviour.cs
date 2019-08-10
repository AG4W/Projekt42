using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBehaviour : ActorBehaviour
{
    [SerializeField]int maxHealth = 20;

    UIManager ui;

    void Awake()
    {
        PlayerData.Initialize(maxHealth);
        ui = FindObjectOfType<UIManager>();
    }

    public override void Move(Vector2 direction)
    {
        Tile nt = base.GetTileFromDirection(direction);

        if (nt.type == TileType.Door)
        {
            //spara senaste seed
            SceneTransitionData.SetLastRoomSeed(GameManager.room.seed);
            //sätt vilket håll vi gick ut åt
            SceneTransitionData.SetEntranceDirection(GameManager.room.GetEntranceDirection(nt));
            SceneManager.LoadScene("Map");
        }
        //spelaren flyttar till denna nya tile
        else
        {
            base.Move(direction);
            //markarerar att spelaren står på denna nya tile
            nt.SetType(TileType.Player);
            base.EndTurn();
        }
    }

    public override void SetPosition(Tile tile)
    {
        if (base.tile != null)
            base.tile.SetType(TileType.Floor);

        base.SetPosition(tile);
        base.tile.SetType(TileType.Player);
    }

    public override void UpdateHealth(int amount)
    {
        PlayerData.GetStat(StatType.Health).UpdateCurrent(amount);
        base.UpdateHealth(amount);
    }

    public void UpdateUI()
    {
        bool[] canMoveInDirection = new bool[4];
        InteractAction[] canInteractInDirection = new InteractAction[4];

        //update ui
        for (int i = 0; i < Directions.cardinals.Length; i++)
        {
            Tile t = base.GetTileFromDirection(Directions.cardinals[i]);

            if (t.type == TileType.Door || t.isTraversable)
                canMoveInDirection[i] = true;
            else
                canMoveInDirection[i] = false;

            //om fiende
            if (t.entity is ActorBehaviour)
                canInteractInDirection[i] = new InteractAction(
                    t.entity.GetInteractHeader(),
                    delegate
                    {
                        ((ActorBehaviour)t.entity).UpdateHealth(PlayerData.GetDamage());
                        base.EndTurn();
                    },
                    Color.red,
                    Icon.Attack);
            //om icke-levande (loot etc)
            else if (t.entity != null)
                canInteractInDirection[i] = new InteractAction(
                    t.entity.GetInteractHeader(), 
                    delegate
                    {
                        t.entity.Interact(this);
                        base.EndTurn();
                    },
                    Color.yellow,
                    Icon.Interact);
            //om ingenting
            else
                canInteractInDirection[i] = null;
        }

        ui.UpdateMovementPad(canMoveInDirection);
        ui.UpdateInteractionPad(canInteractInDirection);
    }
}
