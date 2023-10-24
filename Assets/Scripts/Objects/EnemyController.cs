
public class EnemyController : WorldObject
{
    public override void OnDeath(SpawnData killer)
    {
        base.OnDeath(killer);
        //TODO:spawn decals
    }

}

public enum EnemyNames
{
    ButterFlast = 6,
    TophBluf,
    PotBot,
    TankHank = 7,
}
