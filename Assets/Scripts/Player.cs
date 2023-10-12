using System.Collections;
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

    public Tilemap buildTiles;
    public GameObject HighlightSprite;
    public GameObject tower;
    public MouseMode mouseMode;

    private void Update()
    {
        switch (mouseMode)
        {
            case MouseMode.Select:
                // 
                break;
            case MouseMode.Check:
                var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                var cellPos = buildTiles.WorldToCell(mousePos);
                var cen = buildTiles.GetCellCenterWorld(cellPos);
                if (buildTiles.GetColliderType(cellPos) == Tile.ColliderType.Sprite)
                {
                    HighlightSprite.transform.position = cen;
                    if (Input.GetMouseButtonUp(0))
                    {
                        buildTiles.SetColliderType(cellPos, Tile.ColliderType.None);
                        Instantiate(tower, cen, Quaternion.identity);
                    }
                }
                else
                {
                    HighlightSprite.transform.position = Vector3.one * 1111;
                }
                break;
            default:
                break;
        }
        
    }
}
