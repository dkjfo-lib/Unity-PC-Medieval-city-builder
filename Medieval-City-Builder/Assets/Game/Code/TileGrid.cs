using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileGrid : MonoBehaviour
{
    public Tile tilePrefab;
    public Vector2Int gridSize = new Vector2Int(20, 20);
    public Vector2 oneTileSize = new Vector2(5, 5);

    private void Start()
    {
        Create();
    }

    public void Create()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var newTile = Instantiate(
                    tilePrefab, 
                    new Vector3(x * oneTileSize.x, 0, y * oneTileSize.y), 
                    Quaternion.identity, 
                    transform);
            }
        }
    }
}
