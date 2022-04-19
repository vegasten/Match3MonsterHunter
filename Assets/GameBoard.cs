using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    public event Action<Tile, Tile> OnTileDragged;

    [SerializeField] private TileFactory _tileFactory;
    [SerializeField] private BoardFactory _boardFactory;

    [Header("Effects")]
    [SerializeField] private ParticleSystem _clearingParticleSystem;

    private TileSlot[,] _board;
    private int _numberOfColumns;
    private int _numberOfRows;

    public void InitializeBoard(int numberOfColums, int numberOfRows)
    {
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
                tile.OnTileDragged += TileDragged;
            }
        }
    }

    public void ClearTile(Vector2Int index)
    {
        _board[index.x, index.y].Clear();
    }

    private void PlayClearingParticleEffect()
    {

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
        rectTransform1.DOAnchorPos(Vector2.zero, 0.2f);
        var rectTransform2 = tile2.GetComponent<RectTransform>();
        rectTransform2.DOAnchorPos(Vector2.zero, 0.2f);
    }

    public void InstantiateTile(GameObject tilePrefab, Vector2Int index)
    {
        var tile = Instantiate(tilePrefab, _board.Get(index).transform).GetComponent<Tile>();
        tile.SetTileIndex(index);
        tile.OnTileDragged += TileDragged;
    }

    private void ClearAllTiles()
    {
        foreach (var slot in _board)
        {
            slot.Clear();
        }
    }

    private void TileDragged(Tile tile, Direction direction)
    {
        var nextTileIndex = new Vector2Int(-1, -1);

        switch (direction)
        {
            case Direction.R:
                nextTileIndex = tile.TileIndex + new Vector2Int(1, 0);
                break;
            case Direction.L:
                nextTileIndex = tile.TileIndex - new Vector2Int(1, 0);
                break;
            case Direction.U:
                nextTileIndex = tile.TileIndex - new Vector2Int(0, 1);
                break;
            case Direction.D:
                nextTileIndex = tile.TileIndex + new Vector2Int(0, 1);
                break;
        }

        if (nextTileIndex.x < 0 || nextTileIndex.x > _numberOfColumns - 1)
        {
            return;
        }

        if (nextTileIndex.y < 0 || nextTileIndex.y > _numberOfRows - 1)
        {
            return;
        }

        var nextTile = _board.Get(nextTileIndex).GetComponentInChildren<Tile>();
        OnTileDragged?.Invoke(tile, nextTile);
    }
}
