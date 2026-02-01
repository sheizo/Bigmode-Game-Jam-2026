
using System;
using System.Collections.Generic;

[Serializable]
public class Upgrade
{
    public UpgradeType UpgradeType;
    public List<object> UpgradeValues = new List<object>();
    public List<int> UpgradeCosts = new List<int>();
}
