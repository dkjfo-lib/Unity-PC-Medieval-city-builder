using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    water,
    land,
}

public interface IMapConstructor
{
    void Construct(TileGrid gridSize);
}

public static class TileCreator
{
    public static void CreateTile(TileGrid grid, int x, int y, System.Func<Tile> getPrefab)
    {
        Vector3 localPosition = new Vector3(x * grid.OneTileSize.x, 0, y * grid.OneTileSize.y);
        var newTile = GameObject.Instantiate(
            getPrefab(),
            localPosition,
            Quaternion.identity,
            grid.transform);
    }
}

[System.Serializable]
public class RandomGridCreator : IMapConstructor
{
    public Tile landPrefab;
    public Tile waterPrefab;

    public int borderSize = 5;

    public string seed = "Mike";
    public bool useRandomSeed = false;

    [Range(0, 100)]
    public int landPercent = 50;

    public virtual void Construct(TileGrid grid)
    {
        TileType[,] tiles = GetRandomGrid(grid);
        CreateTiles(grid, tiles);
    }

    protected void CreateTiles(TileGrid grid, TileType[,] tiles)
    {
        for (int x = 0; x < grid.GridSize.x; x++)
        {
            for (int y = 0; y < grid.GridSize.y; y++)
            {
                var tilePrefab = tiles[x, y] == TileType.land ? landPrefab : waterPrefab;
                TileCreator.CreateTile(grid, x, y, () => tilePrefab);
            }
        }
    }

    protected TileType[,] GetRandomGrid(TileGrid grid)
    {
        TileType[,] tiles = new TileType[grid.GridSize.x, grid.GridSize.y];
        if (useRandomSeed)
        {
            seed = System.DateTime.Now.ToString();
        }
        System.Random rnd = new System.Random(seed.GetHashCode());
        for (int x = 0; x < grid.GridSize.x; x++)
        {
            for (int y = 0; y < grid.GridSize.y; y++)
            {
                if (x < borderSize || y < borderSize || x > grid.GridSize.x - borderSize - 1 || y > grid.GridSize.y - borderSize - 1)
                {
                    tiles[x, y] = TileType.water;
                }
                else
                {
                    tiles[x, y] = rnd.Next(0, 100) < landPercent ? TileType.land : TileType.water;
                }
            }
        }
        return tiles;
    }
}

[System.Serializable]
public class SmoothedRandomGridCreator : RandomGridCreator
{
    public int smoothUpdates = 5;
    [Range(0, 48)]
    public int waterPropagation = 4;
    [Range(1, 3)]
    public int neighbourRadius = 1;

    public override void Construct(TileGrid grid)
    {
        TileType[,] tiles = GetRandomGrid(grid);
        for (int i = 0; i < smoothUpdates; i++)
        {
            tiles = Smooth(grid, tiles);
        }
        CreateTiles(grid, tiles);
    }

    protected TileType[,] Smooth(TileGrid grid, TileType[,] tiles)
    {
        TileType[,] newTiles = new TileType[grid.GridSize.x, grid.GridSize.y];
        for (int x = borderSize; x < grid.GridSize.x - borderSize; x++)
        {
            for (int y = borderSize; y < grid.GridSize.y - borderSize; y++)
            {
                int waterAround = GetSorroundingWater(grid, tiles, x, y);
                if (waterAround > waterPropagation)
                {
                    newTiles[x, y] = TileType.water;
                }
                else if (waterAround < waterPropagation)
                {
                    newTiles[x, y] = TileType.land;
                }
                else
                {
                    newTiles[x, y] = tiles[x, y];
                }
            }
        }
        return newTiles;
    }

    private int GetSorroundingWater(TileGrid grid, TileType[,] tiles, int tx, int ty)
    {
        int count = 0;
        for (int x = tx - neighbourRadius; x < tx + neighbourRadius + 1; x++)
        {
            for (int y = ty - neighbourRadius; y < ty + neighbourRadius + 1; y++)
            {
                if (y == ty && x == tx) continue;
                if (tiles[x, y] == TileType.water)
                {
                    count++;
                }
            }
        }
        return count;
    }
}