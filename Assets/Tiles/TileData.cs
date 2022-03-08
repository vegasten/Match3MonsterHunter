using UnityEngine;

public class TileData
{
    public TileData(Vector2Int tileIndex, TileType type)
    {
        TileIndex = tileIndex;
        Type = type;
    }

    public TileData(int x, int y, TileType type)
    {
        TileIndex = new Vector2Int(x, y);
        Type = type;
    }

    public Vector2Int TileIndex;
    public TileType Type;
    public bool IsUsed;
}
