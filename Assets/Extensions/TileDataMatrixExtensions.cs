using UnityEngine;

namespace Battle
{
    public static class TileDataMatrixExtensions
    {
        public static void Swap(this TileData[,] tileMatrix, Vector2Int index1, Vector2Int index2)
        {
            var tileData1 = tileMatrix[index1.x, index1.y];
            var tileData2 = tileMatrix[index2.x, index2.y];

            tileData1.TileIndex = index2;
            tileData2.TileIndex = index1;

            tileMatrix.Set(index1, tileData2);
            tileMatrix.Set(index2, tileData1);

        }

        public static TileData Get(this TileData[,] tileMatrix, Vector2Int index)
        {
            return tileMatrix[index.x, index.y];
        }

        public static void Set(this TileData[,] tileMatrix, Vector2Int index, TileData tileData)
        {
            tileMatrix[index.x, index.y] = tileData;
        }

        public static void Clear(this TileData[,] tileMatrix, Vector2Int index)
        {
            tileMatrix[index.x, index.y] = null;
        }
    }
}