using System;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public event Action<Tile> TileClicked;

    [SerializeField] private TileFactory _tileFactory;
    [SerializeField] private BoardFactory _boardFactory;

    private TileSlot[,] _board;
    private int _numberOfColumns;
    private int _numberOfRows;

    public void InitializeBoard(int numberOfColums, int numberOfRows)
    {
        //_board = new TileSlot[numberOfColums, numberOfRows];
        _numberOfColumns = numberOfColums;
        _numberOfRows = numberOfRows;

        _board = _boardFactory.CreateBoard(_numberOfColumns, _numberOfRows);
    }

    public void VisualizeTileData(TileData[,] tileDataMatrix)
    {
        ClearAllTiles();

        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows; j++)
            {
                var tileType = tileDataMatrix[i, j].Type;
                var tile = Instantiate(_tileFactory.GetTile(tileType), _board[i, j].transform).GetComponent<Tile>(); // TODO where should factory be used
                tile.SetTileIndex(new Vector2Int(i, j));
                tile.OnTileClicked += OnTileClicked;
            }
        }
    }

    public void ClearTile(Vector2Int index)
    {
        _board[index.x, index.y].Clear();
    }

    public void MoveTile(Vector2Int startIndex, Vector2Int targetIndex)
    {
        var tileTransform = _board.Get(startIndex).transform.GetChild(0);
        tileTransform.SetParent(_board.Get(targetIndex).transform);
        tileTransform.GetComponent<Tile>().SetTileIndex(targetIndex);

        var rectTransform = tileTransform.GetComponent<RectTransform>(); // TODO animated by using this?
        rectTransform.anchoredPosition = Vector2.zero;
    }

    public void SwapTiles(Tile tile1, Tile tile2)
    {
        var index1 = tile1.TileIndex;
        var index2 = tile2.TileIndex;

        var parent1 = tile1.transform.parent;
        var parent2 = tile2.transform.parent;

        var siblingIndex1 = tile1.transform.GetSiblingIndex();
        var siblingIndex2 = tile2.transform.GetSiblingIndex();

        tile1.transform.SetParent(parent2);
        tile2.transform.SetParent(parent1);

        tile1.transform.SetSiblingIndex(siblingIndex2);
        tile2.transform.SetSiblingIndex(siblingIndex1);

        tile1.SetTileIndex(index2);
        tile2.SetTileIndex(index1);

        var rectTransform1 = tile1.GetComponent<RectTransform>();
        rectTransform1.anchoredPosition = Vector2.zero;

        var rectTransform2 = tile2.GetComponent<RectTransform>();
        rectTransform2.anchoredPosition = Vector2.zero;
    }

    public void InstantiateTile(GameObject tilePrefab, Vector2Int index)
    {
        var tile = Instantiate(tilePrefab, _board.Get(index).transform).GetComponent<Tile>();
        tile.SetTileIndex(index);
    }

    private void OnTileClicked(Tile tile)
    {
        TileClicked?.Invoke(tile);
    }

    private void ClearAllTiles()
    {
        foreach (var slot in _board)
        {
            slot.Clear();
        }
    }
}
