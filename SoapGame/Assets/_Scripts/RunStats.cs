using System;
using System.Collections.Generic;
using Freya;

public struct RunStats
{
    public float DistanceTravelled;
    public int Stains, NPCs, Objects;

    public int TotalMoneyEarned;
    
    public void SetDistanceTravelled(float distance){
        DistanceTravelled = distance;
    }
    

    public void AddCleaned(CleanedType cleanedType){
        switch (cleanedType){
            case CleanedType.NPC:
                NPCs++;
                break;
            case CleanedType.OBJECT:
                Objects++;
                break;
            case CleanedType.STAIN:
                Stains++;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(cleanedType), cleanedType, null);
        }
        
        UpdateTotalMoneyEarned();
    }

    public void UpdateTotalMoneyEarned(){
        
        float moneyTotal = 
            NPCs * GameManager.NpcValue +
            Objects * GameManager.ObjectValue +
            Stains * GameManager.StainValue +
            DistanceTravelled * GameManager.DistanceMValue;
        
        TotalMoneyEarned = moneyTotal.CeilToInt();
    }

}
