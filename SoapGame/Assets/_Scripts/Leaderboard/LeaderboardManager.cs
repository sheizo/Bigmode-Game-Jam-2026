using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class LeaderboardManager : Singleton<LeaderboardManager>
{
    [SerializeField] private LeaderboardEntryRow _entryRowPrefab;
    [SerializeField] private Transform _entriesContainer;
    [SerializeField] private int _topScoresCount = 20;

    public List<LeaderboardEntry> TopScores { get; private set; }

    private List<LeaderboardEntryRow> _entryRows = new List<LeaderboardEntryRow>();


    protected override void Awake()
    {
        DontDestroyOnLoad(gameObject);
        base.Awake();
    }

    public void Start()
    {
        _ = RefreshAndDisplayTopScores();
    }

    public async Task RefreshAndDisplayTopScores()
    {
        await RefreshTopScores(_topScoresCount);

        // Clear existing rows
        foreach (var row in _entryRows)
        {
            Destroy(row.gameObject);
        }

        _entryRows.Clear();

        // Create new rows
        foreach (var entry in TopScores)
        {
            var row = Instantiate(_entryRowPrefab, _entriesContainer);
            row.SetEntry(entry);
            _entryRows.Add(row);
        }
    }
    
    public async Task RefreshTopScores(int count = 100)
    {
        TopScores = await LeaderboardClient.GetTopScores(count);
    }

    public async Task SubmitScore(string playerName, int score)
    {
        await LeaderboardClient.PostScore(playerName, score);
    }

    [ContextMenu("Test Post Score")]
    public void TestPostScore()
    {
        _ = LeaderboardClient.PostScore("Test Player", 100);
    }

    [ContextMenu("Test Get Scores")]
    public async void TestGetScores()
    {
        var scores = await LeaderboardClient.GetTopScores(50);
        foreach (var entry in scores)
        {
            Debug.Log($"{entry.player_name}: {entry.score}");
        }
    }
}

public static class LeaderboardClient
{
    private static string BaseUrl = "http://82.165.198.31:3000";
    private const string ApiKey = "SOAPYSOAP";

    public static async Task PostScore(string playerName, int score)
    {
        var payload = JsonUtility.ToJson(new PostScoreBody
        {
            playerName = playerName,
            score = score
        });

        using var request = new UnityWebRequest(
            $"{BaseUrl}/submit-score",
            "POST"
        );

        request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(payload));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("x-api-key", ApiKey);

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            throw new System.Exception(request.error);
    }

    public static async Task<List<LeaderboardEntry>> GetTopScores(int count)
    {
        using var request = UnityWebRequest.Get(
            $"{BaseUrl}/leaderboard?limit={count}"
        );

        request.SetRequestHeader("x-api-key", ApiKey);

        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            throw new System.Exception(request.error);

        // JsonUtility can't parse root arrays → wrap it
        var wrappedJson = $"{{\"entries\":{request.downloadHandler.text}}}";
        var wrapper = JsonUtility.FromJson<LeaderboardWrapper>(wrappedJson);

        List<LeaderboardEntry> entries = new List<LeaderboardEntry>(wrapper.entries);
        return entries;
    }

    [System.Serializable]
    private class PostScoreBody
    {
        public string playerName;
        public int score;
    }
}

[System.Serializable]
public class LeaderboardEntry
{
    public string player_name;
    public int score;
}

[System.Serializable]
class LeaderboardWrapper
{
    public LeaderboardEntry[] entries;
}

[Serializable]
public class ProfanityRequest
{
    public string message;
}