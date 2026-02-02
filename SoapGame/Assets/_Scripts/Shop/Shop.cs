using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private UpgradeValuesSO _upgradeValues;
    [SerializeField] private List<BuyableUpgrade> _buyableUpgrades;

    private void OnEnable(){
        foreach (BuyableUpgrade buyableUpgrade in _buyableUpgrades){
            buyableUpgrade.OnClicked += TryBuying;
        }
    }

    private bool TryBuying(string upgradeID){
        PlayerUpgradeManager upgradeManager = PlayerUpgradeManager.Instance;
        GameManager gameManager = GameManager.Instance;
        int money = gameManager.PlayerStats.Money;
        
        UpgradeBase selectedUpgradeBase = null;
        foreach (FieldInfo upgradeBaseField in upgradeManager.GetAllUpgradeFields()){
            UpgradeBase upgradeBase = upgradeBaseField.GetValue(upgradeManager) as UpgradeBase;
            if(upgradeBase?.Id == upgradeID) selectedUpgradeBase = upgradeBase;
        }
        int upgradeCost = selectedUpgradeBase.NextLevelCost();
        
        if(money < upgradeCost) print("broke ass nigga");
        gameManager.PlayerStats.Money-= upgradeCost;
        
        gameManager.SaveGame();
        return true;
    }

    [ContextMenu("Populate Buyable Upgrades")]
    private void PopulateBuyableUpgrades(){
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