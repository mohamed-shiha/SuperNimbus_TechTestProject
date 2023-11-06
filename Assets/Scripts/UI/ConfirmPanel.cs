using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmPanel : MonoBehaviour
{
    [SerializeField] Button ConfirmButton;
    [SerializeField] Button CancelButton;
    [SerializeField] TextMeshProUGUI MessageText;

    public void Show(Action onConfirm, Action callBack = null, string message = "Confirm Purchase ?")
    {
        gameObject.SetActive(true);
        MessageText.text = message;
        ConfirmButton.onClick.RemoveAllListeners();
        ConfirmButton.onClick.AddListener(() => 
        {
            onConfirm?.Invoke();
            callBack?.Invoke();
            gameObject.SetActive(false);
        });
    }
}
