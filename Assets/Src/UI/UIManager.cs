using UnityEngine;
using UnityEngine.UI;

using System;

public class UIManager : MonoBehaviour
{
    [Header("Healthbar")]
    [SerializeField]Image healthbar;
    [SerializeField]Text healthtext;

    [Header("Movement Pad")]
    [SerializeField]Button moveUp;
    [SerializeField]Button moveRight;
    [SerializeField]Button moveLeft;
    [SerializeField]Button moveDown;

    [Header("Interaction Pad")]
    [SerializeField]Button interactUp;
    [SerializeField]Button interactRight;
    [SerializeField]Button interactLeft;
    [SerializeField]Button interactDown;

    [SerializeField]Sprite attackIcon;
    [SerializeField]Sprite interactIcon;

    [Header("Info")]
    [SerializeField]Button infoToggle;
    [SerializeField]GameObject infoWindow;

    Button[] movementKeys;
    Button[] interactionKeys;

    void Start()
    {
        moveUp.onClick.AddListener(() => GameManager.player.Move(Vector2.up));
        moveRight.onClick.AddListener(() => GameManager.player.Move(Vector2.right));
        moveLeft.onClick.AddListener(() => GameManager.player.Move(Vector2.left));
        moveDown.onClick.AddListener(() => GameManager.player.Move(Vector2.down));

        infoToggle.onClick.AddListener(() => ToggleInfo());
        infoWindow.SetActive(false);

        movementKeys = new Button[] { moveUp, moveRight, moveLeft, moveDown };
        interactionKeys = new Button[] { interactUp, interactRight, interactLeft, interactDown };

        UpdateInteractionPad(null, null, null, null);
    }

    public void UpdatePlayerHealthUI(int current, int max)
    {
        healthbar.fillAmount = current / (float)max;
        healthtext.text = current.ToString() + "/" + max.ToString();
    }
    public void UpdateMovementPad(params bool[] statuses)
    {
        for (int i = 0; i < movementKeys.Length; i++)
            movementKeys[i].interactable = statuses[i];
    }
    public void UpdateInteractionPad(params InteractAction[] actions)
    {
        for (int i = 0; i < interactionKeys.Length; i++)
        {
            int a = i;

            interactionKeys[i].onClick.RemoveAllListeners();
            interactionKeys[i].gameObject.SetActive(actions[a] != null);
            
            if (interactionKeys[i].gameObject.activeSelf)
            {
                interactionKeys[i].GetComponentInChildren<Text>().text = actions[a].header;
                interactionKeys[i].GetComponentInChildren<Image>().color = actions[a].color;
                interactionKeys[i].GetComponentInChildren<Image>().sprite = GetSpriteFromIcon(actions[a].icon);
                interactionKeys[i].onClick.AddListener(() => actions[a].action());
            }
        }
    }

    void ToggleInfo()
    {
        infoWindow.SetActive(!infoWindow.activeSelf);

        if (!infoWindow.activeSelf)
            return;

        Text name = infoWindow.transform.Find("weaponName").GetComponent<Text>();
        Text body = infoWindow.transform.Find("weaponBody").GetComponent<Text>();

        name.text = PlayerData.weapon.name;

        string bt = "";

        bt += PlayerData.weapon.GetDescription() + "\n\n";
        bt += "Permanent Upgrades:\n";

        for (int i = 0; i < PlayerData.items.Count; i++)
            bt += PlayerData.items[i].name + "\n" + PlayerData.items[i].GetDescription() + "\n\n";

        body.text = bt;
    }
    Sprite GetSpriteFromIcon(Icon icon)
    {
        switch (icon)
        {
            case Icon.Interact:
                return interactIcon;
            case Icon.Attack:
                return attackIcon;
            default:
                return null;
        }
    }
}
public class InteractAction
{
    public readonly string header;
    public readonly Action action;
    public readonly Color color;
    public readonly Icon icon;

    public InteractAction(string header, Action action, Color color, Icon icon)
    {
        this.header = header;
        this.action = action;
        this.color = color;
        this.icon = icon;
    }
}
public enum Icon
{
    Interact, 
    Attack
}
