using TMPro;
using UnityEngine;

class RewardDisplayObject : WorldObject
{
    [SerializeField] TextMeshProUGUI UIText;
    public override void RestartAlive(Vector2 newPos, Vector2 dir)
    {
        gameObject.SetActive(true);
        (transform as RectTransform).position = newPos;
    }

    public override void SetData(SpawnData spawnData)
    {
        UIText.text = "+" + spawnData.GetRewardPerKill();
    }


    public void EndAnimation()
    {
        Die(Data);
    }
}
