using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;

public class LeaderboardManager : MonoBehaviour {
    
    void OnError(PlayFabError error) {
        Debug.Log("Error while adding score to leaderboard");
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
        foreach (var item in result.Leaderboard) {
            Debug.Log("Leaderboard Position: " + item.Position + " ID: " + item.PlayFabId + " Score: " + item.StatValue);
        }
    }
}
