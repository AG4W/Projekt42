using UnityEngine;

using System.Collections;

public class MapEntity : MonoBehaviour
{
    [SerializeField]string interactHeader = "Interact";

    public Tile tile { get; private set; }

    public virtual void SetPosition(Tile tile)
    {
        if(this.tile != null)
            this.tile.SetEntity(null);

        this.tile = tile;
        this.tile.SetEntity(this);
        this.transform.position = tile.position;
    }
    public void MoveToPosition(Tile tile)
    {
        if(this.tile != null)
        {
            StartCoroutine(MoveWithFixedSpeed(this.tile, tile, 7.5f));
            this.tile.SetEntity(null);
        }

        this.tile = tile;
        this.tile.SetEntity(this);
    }

    public virtual void Interact(ActorBehaviour interactee)
    {
        Debug.Log(interactee.name + " interacted with " + this.name);
    }

    public string GetInteractHeader()
    {
        return interactHeader;
    }

    IEnumerator MoveWithFixedSpeed(Tile a, Tile b, float speed)
    {
        float s = (speed / (a.position - b.position).magnitude) * Time.fixedDeltaTime;
        float t = 0f;

        //interpolera objektets position mellan tile a och tile b över tid
        while (t <= 1f)
        {
            t += s;
            this.transform.position = Vector3.Lerp(a.position, b.position, t);
            yield return new WaitForFixedUpdate();
        }
    }
}
