using UnityEngine;

public class NpcBehaviour : ActorBehaviour
{
    [SerializeField]int maxHealth = 20;
    int currentHealth;
    // Tog bort int damage
    // Använder istället weapon.GetDamage() / PlayerData.GetDamage()
    [SerializeField]Weapon weapon;

    ActorBehaviour player;

    Animator playerAnimator;
    Animator animator;

    void Start()
    {
        if (weapon == null)
            weapon = Resources.Load<Weapon>("Data/Items/Weapons/Fists");

        player = GameManager.player;
        currentHealth = maxHealth;

        playerAnimator = player.GetComponentInChildren<Animator>();
        animator = this.GetComponentInChildren<Animator>();

        //request UI healthbar etc
        this.GetComponent<ActorHealthbar>().Initialize(this);

        OnHealthChanged(currentHealth, maxHealth);
    }

    public override void SetPosition(Tile tile)
    {
        if (base.tile != null)
            base.tile.SetType(TileType.Floor);

        base.SetPosition(tile);
        base.tile.SetType(TileType.Npc);
    }
    public override void UpdateHealth(int amount)
    {
        playerAnimator.SetTrigger("playerAttack");

        currentHealth -= amount;
        OnHealthChanged(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            PlayerData.OnEnemyKilled();
            if (animator != null)
            {
                animator.SetTrigger("dead");
            }
            base.OnDeath();
        }

        base.UpdateHealth(amount);
    }

    public override void StartTurn()
    {
        //kalla basmetod
        base.StartTurn();

        //hitta vinkeln till spelaren, normalisera för att kunna jämföra korrekt
        Vector2 directionToPlayer = (player.transform.position - this.transform.position).normalized;
        //nollställ vår bestmove/vinkel för jämförelser nedan
        Vector2 bestMoveDirection = Vector2.zero;
        float a = Mathf.Infinity;

        //cardinals är upp, höger, ner, vänster
        /*eller mer matematiskt korrekt
         * 0, 1
         * 1, 0
         * 0, -1
         * -1, 0
        */
        //vi måste kolla vilken som är närmast vår riktning till spelaren
        for (int i = 0; i < Directions.cardinals.Length; i++)
        {
            //summan av vår nuvarande position + cardinalen vi kollar på
            Vector2 p = base.tile.position + Directions.cardinals[i];

            //ifall position är utanför kartan, prova nästa cardinal
            if (p.x < 0f || p.x > GameManager.room.width || p.y < 0f || p.y > GameManager.room.height)
                continue;

            //hämta tilen vi är intresserade
            Tile t = GameManager.room.Get((int)p.x, (int)p.y);

            //om denna tile är blockerad eller en annan fiende men inte är spelaren, prova nästa cardinal
            if ((!t.isTraversable || t.type == TileType.Npc) && !(t.type == TileType.Player))
                continue;

            float angleBetween = Vector2.Angle(directionToPlayer, Directions.cardinals[i]);

            //ifall vinkeln mellan spelare/fiende och cardinal är mindre än nuvarande, ersätt nuvarande
            if (bestMoveDirection == Vector2.zero || angleBetween < a)
            {
                bestMoveDirection = Directions.cardinals[i];
                a = angleBetween;
            }
        }

        //Debug.LogWarning(this.name + " x=" + bestMoveDirection.x + " y=" + bestMoveDirection.y);
        Tile nt = GetTileFromDirection(bestMoveDirection);

        //fångar konstiga errors, typ ifall AIn är omringad osv
        //den kommer då automatiskt att skicka turen vidare.
        if (bestMoveDirection == Vector2.zero)
        {
            Debug.LogWarning(this.name + " found no possible move!");
            base.EndTurn();
        }
        //om spelaren står på denna tile attackerar fienden spelaren
        //har ändrat så att vi ej behöver använda "Interact()" som damage-metod, det kändes konstigt.
        else if (GameManager.player.tile == nt)
        {
            ((ActorBehaviour)nt.entity).UpdateHealth(weapon.GetDamage(GameManager.room.random));
            base.EndTurn();
        }
        //fienden flyttar till denna nya tile
        else
        { 
            base.Move(bestMoveDirection);
            //markarerar att fienden står på denna nya tile
            nt.SetType(TileType.Npc);
            base.EndTurn();
        }
    }

    public delegate void HealthChangeEvent(int current, int max);
    public event HealthChangeEvent OnHealthChanged;
}
