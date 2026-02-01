using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Shop : MonoBehaviour
{
    [SerializeField] private PlayerProgressHubSO _playerProgressHub;
    [SerializeField] private UpgradeValuesSO _upgradeValues;
    [SerializeField] private List<BuyableUpgrade> _buyableUpgrades;

    
    
    
    private void OnEnable(){
        foreach (BuyableUpgrade buyableUpgrade in _buyableUpgrades){
            buyableUpgrade.OnClicked += TryBuying;
        }
    }


    private bool TryBuying(UpgradeType upgradeType){
        string upgradeName = upgradeType.ToString();
        FieldInfo upgradeLevelField = typeof(PlayerStats).GetField(upgradeName);
        FieldInfo upgradeValuesField = typeof(UpgradeValuesSO).GetField(upgradeName);

        if (upgradeValuesField == null || upgradeLevelField == null){
            Debug.LogError("Invalid upgrade field");
            return false;
        }
        
        int upgradeLevel = (int) upgradeLevelField.GetValue(_playerProgressHub.LiveData);
        IList upgradeValues = (IList) upgradeValuesField.GetValue(_upgradeValues);
        
        if (upgradeLevel >= upgradeValues.Count){
            Debug.Log($"Upgrade {upgradeName} maxed out");
            return false;
        }

        upgradeLevel++;
        upgradeLevelField.SetValue(_playerProgressHub.LiveData, upgradeLevel);
        
        _playerProgressHub.Save();
        
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