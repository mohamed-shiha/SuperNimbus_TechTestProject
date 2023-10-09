using UnityEngine;

public class EnemyController : WorldObject
{
    public override void OnDeath()
    {
        base.OnDeath();
        //give the player a reward
        GameManager.Instance.RewardPlayer(Data.RewardPerKill);
        //TODO:spawn decals
    }
}
