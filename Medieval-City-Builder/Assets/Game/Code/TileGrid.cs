using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TileGrid : MonoBehaviour
{
    [System.Obsolete("Use Methods!", false)]
    public Vector2Int gridSize = new Vector2Int(20, 20);
    [System.Obsolete("Use Methods!", false)]
    public Vector2 oneTileSize = new Vector2(5, 5);
    [System.Obsolete("Use Methods!", false)]
    public SmoothedRandomGridCreator gridCreator;

    public bool constructed = false;

    public Vector2Int GridSize => gridSize;
    public Vector2 OneTileSize => oneTileSize;
    public IMapConstructor GridCreator => gridCreator;

    private void Update()
    {
        if (constructed == false)
        {
            constructed = true;
            DeleteOldMap();
            GridCreator.Construct(this);
        }
    }

    private void DeleteOldMap()
    {
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Vector3 size = new Vector3(OneTileSize.x * GridSize.x, 0, GridSize.y * OneTileSize.y);
        Gizmos.DrawWireCube(
            transform.position + size / 2 - new Vector3(OneTileSize.x, 0, OneTileSize.y) / 2,
            size
            );
    }
}
