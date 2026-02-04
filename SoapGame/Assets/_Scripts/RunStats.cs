using System.Collections.Generic;

public struct RunStats
{
    public float DistanceTravelled;
    public int CommonNpcHit, UncommonNpcHit, RareNpcHit;
    public int StainsHit;

    public int TotalMoneyEarned;

    public void SetDistanceTravelled(float distance){
        DistanceTravelled = distance;
    }
    
    public void AddNpcHit(NpcRarity rarity)
    {
        switch(rarity)
        {
            case NpcRarity.COMMON:
                CommonNpcHit++;
                break;
            case NpcRarity.UNCOMMON:
                UncommonNpcHit++;
                break;
            case NpcRarity.RARE:
                RareNpcHit++;
                break;
        }
    }

    public void AddStainHit(){
        StainsHit++;
    }
}
