using UnityEngine;
using System.Text.RegularExpressions;

public class MenuUtil {
    private const string PlayerPrefsNameKey = "PlayerName"; 
        
    public static bool IsPlayerNameValid(string playerName) {
        if (string.IsNullOrWhiteSpace(playerName)) return false;
        if (playerName.Length < 2 || playerName.Length > 20) return false;
            
        return true;
    }

    public static bool IsValidIPAddress(string ipAddress) {
        if (ipAddress == "localhost") return true;
        var ipRegex = new Regex("^(\\d{1,3}\\.){3}\\d{1,3}$");
        return ipRegex.IsMatch(ipAddress);
    }

    public static string GetNameFromPlayerPrefs() {
        return PlayerPrefs.GetString(PlayerPrefsNameKey);
    }

    public static void SaveNameToPlayerPrefs(string playerName) {
        PlayerPrefs.SetString(PlayerPrefsNameKey, playerName);
    }
}