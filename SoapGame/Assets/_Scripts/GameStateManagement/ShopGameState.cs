using UnityEngine;

public class ShopGameState : GameStateBase
{
    public override void OnEntered()
    {
        base.OnEntered();

        Debug.Log("Entered Shop game state");
    }
}
