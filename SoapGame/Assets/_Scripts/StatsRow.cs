using TMPro;
using UnityEngine;

public class StatsRow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _statLabel;

    public void Init(string label, int amount, float moneyPerAmount)
    {
        _statLabel.text = $"{label} - {amount} x {moneyPerAmount:0.#}";
        _statLabel.overflowMode = TextOverflowModes.Overflow;
    }
    public void Init(string label, int amount)
    {
        _statLabel.text = label + " " + amount.ToString();
        _statLabel.overflowMode = TextOverflowModes.Overflow;
        
    }
}
