using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    empty,
    water,
    land1,
    land2,
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
    public Tile landPrefab1;
    public Tile landPrefab2;
    public Tile waterPrefab;

    [Range(3, 10)]
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
                var tilePrefab =
                    tiles[x, y] == TileType.land1 ? landPrefab1 :
                    tiles[x, y] == TileType.land2 ? landPrefab2 :
                    tiles[x, y] == TileType.water ? waterPrefab :
                    null;
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
                if (IsBorderTile(grid, x, y))
                {
                    tiles[x, y] = TileType.water;
                }
                else
                {
                    tiles[x, y] = rnd.Next(0, 100) < landPercent ?
                        rnd.Next(0, 2) < 1 ? TileType.land1 : TileType.land2 :
                        TileType.water;
                }
            }
        }
        return tiles;
    }

    protected bool IsBorderTile(TileGrid grid, int x, int y)
    {
        return
            x < borderSize || x > grid.GridSize.x - borderSize - 1 ||
            y < borderSize || y > grid.GridSize.y - borderSize - 1;
    }
}

[System.Serializable]
public class SmoothedRandomGridCreator : RandomGridCreator
{
    public int smoothUpdates = 5;
    [Range(0, 1)]
    public float tilePropagationPercent = .33f;
    [Range(1, 3)]
    public int neighbourRadius = 1;

    private int[] neighbourCount = { 8, 24, 38 };

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
        for (int x = 0; x < grid.GridSize.x; x++)
        {
            for (int y = 0; y < grid.GridSize.y; y++)
            {
                if (IsBorderTile(grid, x, y))
                {
                    newTiles[x, y] = TileType.water;
                }
                else
                {
                    TileType dominantType = GetDominantTileType(tiles, x, y);
                    if (dominantType == TileType.empty)
                    {
                        newTiles[x, y] = tiles[x, y];
                    }
                    else
                    {
                        newTiles[x, y] = dominantType;
                    }
                }
            }
        }
        return newTiles;
    }

    private TileType GetDominantTileType(TileType[,] tiles, int tx, int ty)
    {
        float currectPercent = 0;
        TileType dominantType = TileType.empty;
        BaseEnumMap<TileType, float> tileCounter = new BaseEnumMap<TileType, float>();
        for (int x = tx - neighbourRadius; x < tx + neighbourRadius + 1; x++)
        {
            for (int y = ty - neighbourRadius; y < ty + neighbourRadius + 1; y++)
            {
                if (y == ty && x == tx) continue;
                tileCounter.Set(tiles[x, y], tileCounter.Get(tiles[x, y]) + 1);
            }
        }
        foreach (TileType tileType in System.Enum.GetValues(typeof(TileType)))
        {
            tileCounter.Set(tileType, tileCounter.Get(tileType) / neighbourCount[neighbourRadius - 1]);
            if (tileCounter.Get(tileType) > tilePropagationPercent &&
                tileCounter.Get(tileType) > currectPercent)
            {
                currectPercent = tileCounter.Get(tileType);
                dominantType = tileType;
            }
        }
        return dominantType;
    }
}