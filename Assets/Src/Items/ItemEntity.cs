using UnityEngine;

public class ItemEntity : MapEntity
{
    [SerializeField]Item item;
    [SerializeField]AudioClip[] interactSounds;

    [SerializeField]bool destroyAfterInteract = true;

    public override void Interact(ActorBehaviour interactee)
    {
        //bara spelaren kan interacta med items?
        if (interactee != GameManager.player)
            return;

        if (item is Weapon)
            PlayerData.SetWeapon(item as Weapon);
        //vi sparar inte consumables permanent
        else if(!item.isConsumable)
            PlayerData.AddItems(item);

        //äckligt att behöva invertera såhär
        PlayerData.GetStat(StatType.Health).UpdateCurrent(-item.hpModifier);

        if (interactSounds != null && interactSounds.Length > 0)
            AudioManager.Play(SFXType.Oneshot, interactSounds.RandomItem(null), Random.Range(.75f, 1.25f));

        if (destroyAfterInteract)
        {
            base.tile.SetType(TileType.Floor);
            base.tile.SetEntity(null);

            Destroy(this.gameObject);
        }
    }
}
