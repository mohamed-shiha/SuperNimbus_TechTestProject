using System;
using UnityEngine;
using UnityEngine.UI;

public class TowerButton : MonoBehaviour
{
    [SerializeField] Image TowerIcon;
    [SerializeField] int ID;
    int Cost;
    Button Button;

    private void Start()
    {
        Button = GetComponentInChildren<Button>();
        Button.onClick.AddListener(() => OnSelected());
    }

    public void SetData(int id, int cost, Sprite icon)
    {
        ID = id;
        Cost = cost;
        TowerIcon.sprite = icon;
    }

    public void OnSelected()
    {
        GameManager.Instance.OnTowerSelected(ID);
    }

    public void UpdateIntractable(int totalPoints)
    {
        Button.interactable = totalPoints >= Cost;
    }
}
