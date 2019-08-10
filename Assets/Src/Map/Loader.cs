using UnityEngine;

public class Loader : MonoBehaviour
{
    [SerializeField]int minWidth = 10;
    [SerializeField]int maxWidth = 20;

    [SerializeField]int minHeight = 15;
    [SerializeField]int maxHeight = 25;

    void Start()
    {
        //starta
        GameManager.Initialize(minWidth, maxWidth, minHeight, maxHeight);
    }
}
