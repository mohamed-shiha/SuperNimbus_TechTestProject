using TMPro;
using UnityEngine;

class WinLoseScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI LevelResultText;
    [SerializeField] ResutlItem ResutlItemPrefab;
    [SerializeField] Transform ItesmParent;

    public void UpdateResult(string text, string[] results)
    {
        LevelResultText.text = text;
        if(ItesmParent.childCount >0)
        {
            for (int i = 0; i < ItesmParent.childCount; i++)
            {
                Destroy(ItesmParent.GetChild(i).gameObject);
            }
        }
        foreach (var item in results)
        {
            Instantiate(ResutlItemPrefab, ItesmParent)
                .SetText(item.ToString());
        }
    }

}
