using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoardMananger : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private GameBoard _gameBoard;

    [Header("Game board settings")]
    [SerializeField] private int _numberOfColumns = 6;
    [SerializeField] private int _numberOfRows = 6;

    [Header("Factories")]
    [SerializeField] private TileFactory _tileFactory;

    [Header("Debug")]

    private TileData[,] _tilesMatrix;
    private List<Match> _matches;
    private MatchChecker _matchChecker;

    private void Awake()
    {
        _tilesMatrix = new TileData[_numberOfColumns, _numberOfRows];
        _matches = new List<Match>();
        _matchChecker = new MatchChecker();
    }

    private void Start()
    {
        _gameBoard.InitializeBoard(_numberOfColumns, _numberOfRows);
        SpawnRandomTilesWithNoMatch();
        _gameBoard.OnTileDragged += OnTileDragged;
    }

    private void SpawnRandomTilesWithNoMatch()
    {
        bool hasMatches = true;

        CreateRandomTilesData();
        while (hasMatches)
        {
            var matches = _matchChecker.FindThreeInARow(_tilesMatrix, _numberOfRows, _numberOfColumns);

            if (matches.Count == 0)
            {
                hasMatches = false;
            }
            else
            {
                // TODO Just change single tiles instead of spawning everything new
                // Should be enough to change the center of a three-in-a-row

                CreateRandomTilesData();

            }
            ClearAllIsUsed();
            matches.Clear();
        }

        _gameBoard.VisualizeTileData(_tilesMatrix);
    }

    private void CreateRandomTilesData()
    {
        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows; j++)
            {
                int tileTypeIndex = Random.Range(0, 5); // TODO Connect this and enum in some way                
                TileData tileData = new TileData(i, j, (TileType)tileTypeIndex);
                _tilesMatrix[i, j] = tileData;
            }
        }
    }

    private void CheckForMatches()
    {
        // Order here determines priority
        _matches.AddRange(_matchChecker.FindFiveInARow(_tilesMatrix, _numberOfRows, _numberOfColumns));
        _matches.AddRange(_matchChecker.FindPlusShapes(_tilesMatrix, _numberOfRows, _numberOfColumns));
        _matches.AddRange(_matchChecker.FindTShapes(_tilesMatrix, _numberOfRows, _numberOfColumns));
        _matches.AddRange(_matchChecker.FindLShapes(_tilesMatrix, _numberOfRows, _numberOfColumns));
        _matches.AddRange(_matchChecker.FindFourInARow(_tilesMatrix, _numberOfRows, _numberOfColumns));
        _matches.AddRange(_matchChecker.FindThreeInARow(_tilesMatrix, _numberOfRows, _numberOfColumns));

        ClearAllIsUsed();
    }

    private void ClearAllIsUsed()
    {
        foreach (var tileData in _tilesMatrix)
        {
            tileData.IsUsed = false;
        }
    }

    private void ClearMatches()
    {
        foreach (var match in _matches)
        {
            foreach (var index in match.Index)
            {
                _tilesMatrix.Clear(index);
                _gameBoard.ClearTile(index);
            }
        }
        _matches.Clear();
    }

    private IEnumerator ClearingLoop()
    {
        bool matchExisted = true;

        while (matchExisted)
        {
            CheckForMatches();
            if (_matches.Count == 0)
            {
                matchExisted = false;
                continue;
            }

            ClearMatches();

            yield return new WaitForSeconds(1);

            MakeTilesFallDown();

            yield return new WaitForSeconds(1);

            SpawnNewTiles();

            yield return new WaitForSeconds(1);
        }

        GameManager.IsBoardInputEnabled = true;
        Debug.Log("Finished loop");
    }

    private void MakeTilesFallDown()
    {
        bool tileFell = true;

        while (tileFell)
        {
            tileFell = false;
            for (int j = _numberOfRows - 2; j >= 0; j--)
            {
                for (int i = 0; i < _numberOfColumns; i++)
                {
                    var tileUnder = _tilesMatrix[i, j + 1];
                    var currentTile = _tilesMatrix[i, j];

                    if (tileUnder == null && currentTile != null)
                    {
                        _tilesMatrix[i, j] = null;
                        currentTile.TileIndex = new Vector2Int(i, j + 1);
                        _tilesMatrix[i, j + 1] = currentTile;

                        _gameBoard.MoveTile(new Vector2Int(i, j), new Vector2Int(i, j + 1));

                        tileFell = true;
                    }
                }
            }
        }
    }

    private void SpawnNewTiles()
    {
        for (int i = 0; i < _numberOfColumns; i++)
        {
            int count = 0;
            for (int j = 0; j < _numberOfRows; j++)
            {
                if (_tilesMatrix[i, j] == null)
                {
                    count++;
                }
            }

            for (int k = 0; k < count; k++)
            {
                var tileType = (TileType)(Random.Range(0, 5)); // TODO Connect this and enum in some way                

                var tileData = new TileData(i, k, tileType);
                _tilesMatrix[i, k] = tileData;
                tileData.IsUsed = false;

                var tilePrefab = _tileFactory.GetTile(tileType);
                _gameBoard.InstantiateTile(tilePrefab, new Vector2Int(i, k));
            }
        }
    }

    private void OnTileDragged(Tile tile, Tile nextTile)
    {
        if (CanSwitch(tile, nextTile))
        {
            SwitchTiles(tile, nextTile);
            GameManager.IsBoardInputEnabled = false;           
            StartCoroutine(ClearingLoop());
        }
    }

    private bool CanSwitch(Tile tile1, Tile tile2)
    {
        var index1 = tile1.TileIndex;
        var index2 = tile2.TileIndex;

        if (IsAdjacent(index1, index2))
        {
            var tileData1 = _tilesMatrix[index1.x, index1.y];
            var tileData2 = _tilesMatrix[index2.x, index2.y];

            if (WillMakeAMatchOnSwitch(tileData1, tileData2))
            {
                return true;
            }
            else if (WillMakeAMatchOnSwitch(tileData2, tileData1))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsAdjacent(Vector2Int index1, Vector2Int index2)
    {
        if (index1.x == index2.x && Mathf.Abs(index1.y - index2.y) <= 1)
            return true;
        else if (index1.y == index2.y && Mathf.Abs(index1.x - index2.x) <= 1)
            return true;
        else
            return false;
    }

    private bool WillMakeAMatchOnSwitch(TileData tileData1, TileData tileData2)
    {
        bool makesAMatch = false;

        _tilesMatrix[tileData1.TileIndex.x, tileData1.TileIndex.y] = tileData2;
        _tilesMatrix[tileData2.TileIndex.x, tileData2.TileIndex.y] = tileData1;

        CheckForMatches();
        if (_matches.Count >= 1)
        {
            makesAMatch = true;
        }

        _matches.Clear();

        _tilesMatrix[tileData1.TileIndex.x, tileData1.TileIndex.y] = tileData1;
        _tilesMatrix[tileData2.TileIndex.x, tileData2.TileIndex.y] = tileData2;


        return makesAMatch;
    }

    private void SwitchTiles(Tile tile1, Tile tile2)
    {
        _gameBoard.SwapTiles(tile1, tile2);
        _tilesMatrix.Swap(tile1.TileIndex, tile2.TileIndex);
    }
}
