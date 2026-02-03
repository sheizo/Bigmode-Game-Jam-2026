using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private List<BuyableUpgrade> _buyableUpgrades;

    private void OnEnable(){
        foreach (BuyableUpgrade buyableUpgrade in _buyableUpgrades){
            buyableUpgrade.OnClicked += TryBuying;
        }
    }

    private bool TryBuying(BuyableUpgrade buyableUpgrade){
        int money = GameManager.PlayerStats.Money;
        
        int upgradeCost = buyableUpgrade.UpgradeBase.NextLevelCost();

        if (money < upgradeCost){
            
            return false;
        }
        GameManager.PlayerStats.Money -= upgradeCost;

        bool success = buyableUpgrade.UpgradeBase.Upgrade();
        if (success){
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