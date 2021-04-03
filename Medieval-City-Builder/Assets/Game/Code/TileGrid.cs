using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TileGrid : MonoBehaviour
{
    public Tile tilePrefab;
    public Vector2Int gridSize = new Vector2Int(20, 20);
    public Vector2 oneTileSize = new Vector2(5, 5);

    public bool constructed = false;

    private void Update()
    {
        if (constructed == false)
            Create();
    }

    public void Create()
    {
        constructed = true;
        for (int i = this.transform.childCount; i > 0; --i)
            DestroyImmediate(this.transform.GetChild(0).gameObject);
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector3 localPosition = new Vector3(x * oneTileSize.x, 0, y * oneTileSize.y);
                var newTile = Instantiate(
                    tilePrefab,
                    localPosition,
                    Quaternion.identity,
                    transform);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Vector3 size = new Vector3(oneTileSize.x * gridSize.x, 0, gridSize.y * oneTileSize.y);
        Gizmos.DrawWireCube(
            transform.position + size / 2 - new Vector3(oneTileSize.x, 0, oneTileSize.y) / 2,
            size
            );
    }
}
