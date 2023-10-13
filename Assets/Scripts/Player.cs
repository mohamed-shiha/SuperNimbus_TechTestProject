using UnityEngine;
using UnityEngine.Tilemaps;


public class Player : MonoBehaviour
{
    public enum MouseMode
    {
        Select,
        Check,
    }

    public Tilemap buildTiles;
    public GameObject HighlightSprite;
    public GameObject tower;
    public MouseMode mouseMode;
    int selectedTowerID;

    private void Awake()
    {
        GameManager.Instance.OnTowerSelected += TowerSelected;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnTowerSelected -= TowerSelected;
    }

    private void Update()
    {
        switch (mouseMode)
        {
            case MouseMode.Check:
                if (Input.GetMouseButtonUp(1))
                {
                    mouseMode = MouseMode.Select;
                    HighlightSprite.transform.position = Vector3.one * 1111;
                    break;
                }
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var cellPos = buildTiles.WorldToCell(mousePos);
                var cellCenter = buildTiles.GetCellCenterWorld(cellPos);
                if (buildTiles.GetColliderType(cellPos) == Tile.ColliderType.Sprite)
                {
                    HighlightSprite.transform.position = cellCenter;
                    if (Input.GetMouseButtonUp(0))
                    {
                        buildTiles.SetColliderType(cellPos, Tile.ColliderType.None);
                        GameManager.Instance.SpawnTower(selectedTowerID, cellCenter);
                    }
                }
                else
                {
                    HighlightSprite.transform.position = Vector3.one * 1111;
                }
                break;
        }
        
    }

    private void TowerSelected(int newID)
    {
        selectedTowerID = newID;
        mouseMode = MouseMode.Check;
    }
}
