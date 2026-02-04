using TMPro;
using UnityEngine;

public class StatsRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _statLabel;

    public void Init(string label, int amount, int moneyPerAmount)
    {
        _statLabel.text = $"{label} - {amount} x {moneyPerAmount}";
    }
    public void Init(string label, int amount)
    {
        _statLabel.text = label + " " + amount.ToString();
    }
}
