using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class Player : MonoBehaviour
{
    public enum MouseMode
    {
        Select,
        Check,
    }

    [SerializeField] Tilemap buildTiles;
    [SerializeField] GameObject HighlightSprite;

    MouseMode mouseMode;
    int selectedTowerID;
    //private List<SpawnData> unlokedTowers;
    List<Vector3Int> SpawnPositions;

    private void Awake()
    {
        GameManager.Instance.OnTowerSelected += OnTowerSelected;
    }

    private void OnDestroy()
    {
        GameManager.Instance.OnTowerSelected -= OnTowerSelected;
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
                        if (SpawnPositions == null)
                        {
                            SpawnPositions = new();
                        }
                        SpawnPositions.Add(cellPos);
                        mouseMode = MouseMode.Select;
                        HighlightSprite.transform.position = Vector3.one * 1111;
                    }
                }
                else
                {
                    HighlightSprite.transform.position = Vector3.one * 1111;
                }
                break;
        }

    }

    private void OnTowerSelected(int newID)
    {
        selectedTowerID = newID;
        mouseMode = MouseMode.Check;
    }

    internal void OnRestart()
    {
        mouseMode = MouseMode.Select;
        HighlightSprite.transform.position = Vector3.one * 1111;
        if (SpawnPositions != null && SpawnPositions.Count > 0)
        {
            foreach (var cellPos in SpawnPositions)
            {
                buildTiles.SetColliderType(cellPos, Tile.ColliderType.Sprite);
            }
            SpawnPositions.Clear();
        }
    }

}
