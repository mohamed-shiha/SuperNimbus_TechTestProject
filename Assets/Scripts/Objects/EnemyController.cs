
public class EnemyController : WorldObject
{

    public override void OnDeath(SpawnData killer)
    {
        base.OnDeath(killer);
        //give the player a reward
        GameManager.Instance.RewardPlayer(killer.GetRewardPerKill());
        //TODO:spawn decals
    }

}
