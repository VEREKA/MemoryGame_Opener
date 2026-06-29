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
        SetPanelState(firstPanel, true);
        SetPanelState(secondPanel, false);
    }

    public void ShowSecondPanel()
    {
        SetPanelState(firstPanel, false);
        SetPanelState(secondPanel, true);
    }

    public void ShowFirstPanel()
    {
        SetPanelState(secondPanel, false);
        SetPanelState(firstPanel, true);
    }

    private void SetPanelState(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
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
