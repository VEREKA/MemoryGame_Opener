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
}
