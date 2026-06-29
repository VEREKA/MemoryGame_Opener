using UnityEngine;

public static class GameSession
{
    public static string PlayerNick { get; set; } = "";
    public static float LastElapsedTime { get; set; } = 0f;
    public static bool LastRunSuccess { get; set; } = false;
    public static int LastPairsMatched { get; set; } = 0;
    public static int LastTotalPairs { get; set; } = 0;

    public static void SaveToPrefs()
    {
        PlayerPrefs.SetString("PlayerNick", PlayerNick ?? "");
        PlayerPrefs.Save();
    }

    public static void LoadFromPrefs()
    {
        PlayerNick = PlayerPrefs.GetString("PlayerNick", "");
    }

    public static void Clear()
    {
        PlayerNick = "";
        PlayerPrefs.DeleteKey("PlayerNick");
    }

    public static string FormatTimeMs(float seconds, float maxSeconds = -1f)
    {
        float clampedSeconds = maxSeconds > 0f ? Mathf.Clamp(seconds, 0f, maxSeconds) : Mathf.Max(0f, seconds);
        int totalMs = Mathf.FloorToInt(clampedSeconds * 1000f);
        int mins = totalMs / 60000;
        int secs = (totalMs % 60000) / 1000;
        int ms = totalMs % 1000;
        return string.Format("{0:00}:{1:00}:{2:000}", mins, secs, ms);
    }
}
