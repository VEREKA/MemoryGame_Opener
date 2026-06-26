using UnityEngine;
using TMPro;

public class EndSceneController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failPanel;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private string messageTemplate = "Prawie Ci się udało {nick}, spróbuj ponownie";
    [SerializeField] private TextMeshProUGUI successMessageText;
    [SerializeField] private string successTemplate = "{nick}, twój wynik to {time}";

    void Start()
    {
        GameSession.LoadFromPrefs();
        string nick = GameSession.PlayerNick;

        bool success = GameSession.LastRunSuccess;
        if (success)
        {
            if (successPanel != null) successPanel.SetActive(true);
            if (failPanel != null) failPanel.SetActive(false);

            if (successMessageText != null)
            {
                string timeStr = FormatTimeMs(GameSession.LastElapsedTime);
                successMessageText.text = successTemplate.Replace("{nick}", nick).Replace("{time}", timeStr);
                int timeMs = Mathf.FloorToInt(Mathf.Max(0f, GameSession.LastElapsedTime) * 1000f);
                SendResultToBrowser(nick, timeMs.ToString());
            }
        }
        else
        {
            if (failPanel != null) failPanel.SetActive(true);
            if (successPanel != null) successPanel.SetActive(false);

            if (messageText != null)
            {
                messageText.text = messageTemplate.Replace("{nick}", nick);
            }
        }
    }

    public string GetPlayerNick()
    {
        return GameSession.PlayerNick;
    }

    void SendResultToBrowser(string nick, string time)
    {
        Debug.Log($"[EndSceneController] SendResultToBrowser -> nick={nick}, time={time}");

#if UNITY_WEBGL && !UNITY_EDITOR
        SendPlayerResult(nick, time);
#endif
    }

    string FormatTimeMs(float seconds)
    {
        int totalMs = Mathf.FloorToInt(Mathf.Max(0f, seconds) * 1000f);
        int mins = totalMs / 60000;
        int secs = (totalMs % 60000) / 1000;
        int ms = totalMs % 1000;
        return string.Format("{0:00}:{1:00}:{2:000}", mins, secs, ms);
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    [System.Runtime.InteropServices.DllImport("__Internal")]
    private static extern void SendPlayerResult(string nick, string time);
#endif
}
