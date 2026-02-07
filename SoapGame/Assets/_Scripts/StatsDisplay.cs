using System.Collections.Generic;
using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
    [SerializeField] StatsRow _totalMoneyRow;
    [SerializeField] StatsRow _statsRowPrefab;
    [SerializeField] RectTransform _separatorRow;

    [SerializeField] RectTransform _statsRowParent;

    private List<GameObject> _rows;

    //TODO/CBA: re-use same row objects.
    public void ShowStats(RunStats runStats)
    {
        if (_rows != null)
            foreach(GameObject row in _rows)
                Destroy(row);

        _rows = new();

        StatsRow distanceStat = Instantiate(_statsRowPrefab, _statsRowParent);
        distanceStat.Init("Distance", (int) runStats.DistanceTravelled, GameManager.DistanceMValue);
        _rows.Add(distanceStat.gameObject);

        StatsRow npcStat1 = Instantiate(_statsRowPrefab, _statsRowParent);
        npcStat1.Init("Idiots Cleaned", runStats.NPCs, GameManager.NpcValue);
        _rows.Add(npcStat1.gameObject);

        StatsRow npcStat2 = Instantiate(_statsRowPrefab, _statsRowParent);
        npcStat2.Init("Objects Cleaned", runStats.Objects, GameManager.ObjectValue);
        _rows.Add(npcStat2.gameObject);

        StatsRow npcStat3 = Instantiate(_statsRowPrefab, _statsRowParent);
        npcStat3.Init("Stains Cleaned", runStats.Stains, GameManager.StainValue);
        _rows.Add(npcStat3.gameObject);


        _separatorRow.transform.SetAsLastSibling();
        _totalMoneyRow.Init("Total Earned:", runStats.TotalMoneyEarned);
        _totalMoneyRow.transform.SetAsLastSibling();
    }
}
