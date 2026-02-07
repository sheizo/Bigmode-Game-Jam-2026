using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<BuyableUpgrade> _buyableUpgrades;

    public void Init()
    {
        foreach(BuyableUpgrade buyableUpgrade in _buyableUpgrades){
            buyableUpgrade.Init();
            buyableUpgrade.OnClicked += TryBuying;
        }
    }

    private bool TryBuying(BuyableUpgrade buyableUpgrade){
        int money = GameManager.PlayerStats.Money;
        
        int upgradeCost = buyableUpgrade.Upgrade.NextLevelCost();

        if (money < upgradeCost){
            
            return false;
        }
        GameManager.PlayerStats.Money -= upgradeCost;

        bool success = buyableUpgrade.Upgrade.Upgrade();
        if (success){
            AudioManager.Instance.PlayOneShot("ca_ching");
            buyableUpgrade.UpdateUI();
            GameManager.SaveGame();
            GameManager.UIManager.UpdateMoney(GameManager.PlayerStats.Money);
        }

        return success;
    }

    [ContextMenu("Populate Buyable Upgrades")]
    private void PopulateBuyableUpgrades(){
        _buyableUpgrades = new List<BuyableUpgrade>();
        foreach (Transform child in transform){
            if(child.TryGetComponent(out BuyableUpgrade buyableUpgrade))
                _buyableUpgrades.Add(buyableUpgrade);
        }
    }

    
    private void OnDisable(){
        foreach (BuyableUpgrade buyableUpgrade in _buyableUpgrades){
            buyableUpgrade.OnClicked -= TryBuying;
        }
    }
}