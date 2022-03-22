using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class LeaderboardManager : MonoBehaviour {

    
    [Header("Leaderboard Interface")]
    [SerializeField] private TMP_Text messageText;
    public GameObject rowPrefab;
    public Transform rowsParent;


    [Header("Player Stats")]
    public GameObject winsEntry;
    public GameObject lossesEntry;
    public Transform winsParent;
    public Transform lossesParent;

    void Start() {
        HideErrorMessage();
    }

    private void HideErrorMessage() {
        messageText.text = default;
    }

    private void OnError(PlayFabError error) {
        Debug.Log("Error while adding score to leaderboard");
        messageText.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }

    void OnLeaderBoardUpdate(UpdatePlayerStatisticsResult result) {
        Debug.Log("Successful score added");
    }

    public void SendLeaderboard(int winScore) {
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "WinScore",
                    Value = winScore
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate, OnError);
    }

    public void GetLeaderBoard() {
        var request = new GetLeaderboardRequest {
            StatisticName = "WinScore",
            StartPosition = 0,
            MaxResultsCount = 10
        };
        PlayFabClientAPI.GetLeaderboard(request, OnLeaderBoardGet, OnError);
    }

    void OnLeaderBoardGet(GetLeaderboardResult result) {

        foreach (Transform item in rowsParent) {
            Destroy(item.gameObject);
        }

        foreach (var item in result.Leaderboard) {

            GameObject row = Instantiate(rowPrefab, rowsParent);
            TMP_Text[] texts = row.GetComponentsInChildren<TMP_Text>();
            texts[0].text = (item.Position + 1).ToString();
            texts[1].text = item.DisplayName;
            texts[2].text = item.StatValue.ToString();

            Debug.Log("Leaderboard Position: " + item.Position + " ID: " + item.PlayFabId + " Score: " + item.StatValue);
        }
    }

    public void SendLossesLeaderboard(int loseScore) {
        var request = new UpdatePlayerStatisticsRequest {
            Statistics = new List<StatisticUpdate> {
                new StatisticUpdate {
                    StatisticName = "LoseScore",
                    Value = loseScore
                }
            }
        };
        PlayFabClientAPI.UpdatePlayerStatistics(request, OnLeaderBoardUpdate, OnError);
    }

    public void GetStats() {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(), OnGetStats, error => Debug.LogError(error.GenerateErrorReport()));
    }

    void OnGetStats(GetPlayerStatisticsResult result) {
        foreach (Transform item in winsParent) {
            Destroy(item.gameObject);
        }

        foreach (var stat in result.Statistics) {
            GameObject wins = Instantiate(winsEntry, winsParent);
            TMP_Text winText = wins.GetComponentInChildren<TMP_Text>();
            winText.text = stat.Value.ToString();
        }
    }
}
