using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]Button play;
    [SerializeField]Button exit;

    void Start()
    {
        play.onClick.AddListener(() => SceneManager.LoadScene("Map"));
        exit.onClick.AddListener(() => Application.Quit());
    }
}
