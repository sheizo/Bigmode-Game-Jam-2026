public struct RunStats
{
    public float DistanceTravelled;
    public int RareNPCSHit, CommonNPCSHit;
    public int StainsHit;

    public void SetDistanceTravelled(float distance){
        DistanceTravelled = distance;
    }
    
    public void AddNpcHit(bool rare){
        if(rare) RareNPCSHit++;
        else CommonNPCSHit++;
    }

    public void AddStainHit(){
        StainsHit++;
    }
}
