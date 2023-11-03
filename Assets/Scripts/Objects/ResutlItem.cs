using TMPro;
using UnityEngine;

class ResutlItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ResultText;

    public void SetText(string text)
    {
        ResultText.text = text;
    }
}
