
public class EnemyController : WorldObject
{

    public override void OnDeath()
    {
        base.OnDeath();
        //give the player a reward
        GameManager.Instance.RewardPlayer(Data.GetRewardPerKill());
        //TODO:spawn decals
    }
}
