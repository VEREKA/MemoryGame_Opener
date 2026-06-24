using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuPanelController : MonoBehaviour
{
    [SerializeField] private GameObject firstPanel;
    [SerializeField] private GameObject secondPanel;

    [Header("Nickname / Start")]
    [SerializeField] private TMP_InputField nickInput;
    [SerializeField] private string gameSceneName;

    void Start()
    {
        if (firstPanel != null) firstPanel.SetActive(true);
        if (secondPanel != null) secondPanel.SetActive(false);
    }

    public void ShowSecondPanel()
    {
        if (firstPanel != null) firstPanel.SetActive(false);
        if (secondPanel != null) secondPanel.SetActive(true);
    }

    public void ShowFirstPanel()
    {
        if (secondPanel != null) secondPanel.SetActive(false);
        if (firstPanel != null) firstPanel.SetActive(true);
    }

    public void SaveNickAndPlay()
    {
        if (nickInput != null)
        {
            GameSession.PlayerNick = nickInput.text.Trim();
        }

        if (string.IsNullOrEmpty(GameSession.PlayerNick))
            GameSession.PlayerNick = "Player";

        GameSession.SaveToPrefs();

        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
            return;
        }

        if (GameManagerCard.Instance != null)
        {
            GameManagerCard.Instance.LoadGameScene();
        }
    }
}
