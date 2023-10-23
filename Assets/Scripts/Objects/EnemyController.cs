
public class EnemyController : WorldObject
{
    public override void OnDeath(SpawnData killer)
    {
        base.OnDeath(killer);
        //TODO:spawn decals
    }

}
