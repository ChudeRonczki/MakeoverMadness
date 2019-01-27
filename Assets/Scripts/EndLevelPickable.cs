using UnityEngine;

public class EndLevelPickable : Pickable
{
    public override void HandlePickedUp(PlayerController player, Collider usedPoint)
    {
        base.HandlePickedUp(player, usedPoint);
        if (pickUpPlayersCount == pickingUp.Count)
            LevelManager.Instance.FinishLevel();
    }
}
