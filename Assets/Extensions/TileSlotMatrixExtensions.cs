using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TileSlotMatrixExtensions
{
    public static TileSlot Get(this TileSlot[,] slotMatrix, Vector2Int index)
    {
        return slotMatrix[index.x, index.y];
    }

}
