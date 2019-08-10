using UnityEngine;
using UnityEngine.UI;

public class ActorHealthbar : MonoBehaviour
{
    GameObject healthbar;
    Text hpText;
    Image hpImage;

    Camera camera;

    public void Initialize(NpcBehaviour actor)
    {
        camera = Camera.main;

        healthbar = Instantiate(Resources.Load<GameObject>("Prefabs/UI/enemyHealthbar"), FindObjectOfType<Canvas>().transform);
        hpText = healthbar.GetComponentInChildren<Text>();
        hpImage = healthbar.GetComponentInChildren<Image>();

        actor.OnHealthChanged += OnHealthChanged;
    }
    void Update()
    {
        if (healthbar == null)
            return;

        healthbar.transform.position = camera.WorldToScreenPoint(this.transform.position);
    }

    void OnHealthChanged(int current, int max)
    {
        if (current <= 0)
            Destroy(healthbar);
        else
        {
            hpText.text = current + "/" + max;
            hpImage.fillAmount = (float)current / max;
        }
    }
}
