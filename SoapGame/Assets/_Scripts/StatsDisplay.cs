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
        distanceStat.Init("Distance", (int) runStats.DistanceTravelled, 10);
        _rows.Add(distanceStat.gameObject);

        StatsRow npcStat1 = Instantiate(_statsRowPrefab, _statsRowParent);
        npcStat1.Init("Common", runStats.CommonNpcHit, 2);
        _rows.Add(npcStat1.gameObject);

        StatsRow npcStat2 = Instantiate(_statsRowPrefab, _statsRowParent);
        npcStat2.Init("Uncommon", runStats.UncommonNpcHit, 5);
        _rows.Add(npcStat2.gameObject);

        StatsRow npcStat3 = Instantiate(_statsRowPrefab, _statsRowParent);
        npcStat3.Init("Rare", runStats.RareNpcHit, 10);
        _rows.Add(npcStat3.gameObject);

        StatsRow stainStat = Instantiate(_statsRowPrefab, _statsRowParent);
        stainStat.Init("Stains", runStats.StainsHit, 2);
        _rows.Add(stainStat.gameObject);

        _separatorRow.transform.SetAsLastSibling();
        _totalMoneyRow.Init("Total Earned:", runStats.TotalMoneyEarned);
        _totalMoneyRow.transform.SetAsLastSibling();
    }
}
