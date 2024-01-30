using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private FadeToBlack fadeToBlack;
    [SerializeField] private PlayerManager playerManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ResetGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }

    public void Defeat()
    {
        fadeToBlack.FadeToBlackForDefeat();
        playerManager.OnGameEnd();
    }

    public void Victory()
    {
        fadeToBlack.FadeToBlackForVictory();
        playerManager.OnGameEnd();
    }
}