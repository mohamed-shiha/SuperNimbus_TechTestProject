using System;
using UnityEngine;
using UnityEngine.UI;

public class TowerUnlockItem : MonoBehaviour
{
    [SerializeField] Button UnlockButton;
    [SerializeField] RawImage Image;
    [SerializeField] Transform UnlockPanel;

    public void SetData(int id, bool isUnlocked, Action<int> onClick, Texture image )
    {
        UnlockButton.onClick.RemoveAllListeners();
        UnlockButton.onClick.AddListener(() => onClick(id));
        Image.texture = image;
        UpdateUnlocked(isUnlocked);
    }

    internal void UpdateUnlocked(bool isUnlocked)
    {
        UnlockPanel.gameObject.SetActive(!isUnlocked);
    }
}
