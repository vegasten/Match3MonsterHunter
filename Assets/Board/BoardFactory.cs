using System.Collections.Generic;
using UnityEngine;

public class BoardFactory : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform _columnsParent;
    [SerializeField] private GameObject _columnPrefab;
    [SerializeField] private GameObject _TileSlotPrefab;

    public TileSlot[,] CreateBoard(int numberOfColumns, int numberOfRows)
    {
        var columns = new List<Transform>();
        for (int k = 0; k < numberOfColumns; k++)
        {
            columns.Add(Instantiate(_columnPrefab, _columnsParent).transform);
        }

        var tileSlotMatrix = new TileSlot[numberOfColumns, numberOfRows];
        for (int i = 0; i < columns.Count; i++)
        {
            for (int j = 0; j < numberOfRows; j++)
            {
                tileSlotMatrix[i, j] = Instantiate(_TileSlotPrefab, columns[i]).GetComponent<TileSlot>();
            }
        }

        return tileSlotMatrix;
    }
}
