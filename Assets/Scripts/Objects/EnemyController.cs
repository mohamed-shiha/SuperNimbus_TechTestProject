
using System;
using TMPro;
using UnityEngine;

public class EnemyController : WorldObject
{
    [UnityEngine.SerializeField] TextMeshProUGUI text;

    public override void RestartAlive(Vector2 newPos, Vector2 dir)
    {
        base.RestartAlive(newPos, dir);
        text.text = (Data.HitsToDie - CurrentHits).ToString();
    }

    public override void OnDeath(SpawnData killer)
    {
        base.OnDeath(killer);
        //TODO:spawn decals
    }

    public override void TakeDamage(SpawnData hitFrom)
    {
        base.TakeDamage(hitFrom);
        text.text = (Data.HitsToDie - CurrentHits).ToString();
    }
}

public enum EnemyNames
{
    // value is the ID
    ButterFlast = 6,
    TophBluf,
    PotBot,
    TankHank = 7,
}
