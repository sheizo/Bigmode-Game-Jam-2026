using UnityEngine;

public class LeaderboardEntryRow : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _nameText;
    [SerializeField] private TMPro.TextMeshProUGUI _scoreText;

    public void SetEntry(LeaderboardEntry entry)
    {
        _nameText.text = entry.player_name;
        _scoreText.text = entry.score.ToString();
    }
}
