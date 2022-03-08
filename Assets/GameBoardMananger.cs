using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardMananger : MonoBehaviour
{
    [Header("Game board settings")]
    [SerializeField] private int _numberOfColumns = 6;
    [SerializeField] private int _numberOfRows = 6;

    [Header("Factories")]
    [SerializeField] private BoardFactory _boardFactory;
    [SerializeField] private TileFactory _tileFactory;

    [Header("Debug")]
    [SerializeField] private Button _shuffleButton;    

    private TileSlot[,] _tileSlotMatrix;
    private TileData[,] _tilesMatrix;

    private List<Match> _matches;

    private Tile _selectedTile;
    private MatchChecker _matchChecker;

    private void Awake()
    {
        _tilesMatrix = new TileData[_numberOfColumns, _numberOfRows];
        _matches = new List<Match>();
        _matchChecker = new MatchChecker();
    }

    private void Start()
    {
        _tileSlotMatrix = _boardFactory.CreateBoard(_numberOfColumns, _numberOfRows);

        SpawnRandomTilesWithNoMatch();
    }

    private void ClearAllTiles()
    {
        foreach (var tileSlot in _tileSlotMatrix)
        {
            tileSlot.Clear();
        }
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

        VisualizeTiles();

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

    private void VisualizeTiles()
    {
        ClearAllTiles();

        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows; j++)
            {
                var tileType = _tilesMatrix[i, j].Type;
                var tile = Instantiate(_tileFactory.GetTile(tileType), _tileSlotMatrix[i, j].transform).GetComponent<Tile>();
                tile.SetTileIndex(new Vector2Int(i, j));
                tile.OnTileClicked += OnTileClicked;
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
            foreach (var coordinate in match.Coordinates)
            {
                ClearTile(coordinate);
            }
        }
        _matches.Clear();
    }

    private void ClearTile(Vector2Int coordinate)
    {
        _tilesMatrix[coordinate.x, coordinate.y] = null;

        var tileSlot = _tileSlotMatrix[coordinate.x, coordinate.y];
        tileSlot.Clear();
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

                        var boardTile = _tileSlotMatrix[i, j].transform.GetChild(0);
                        boardTile.SetParent(_tileSlotMatrix[i, j + 1].transform);
                        boardTile.GetComponent<Tile>().SetTileIndex(new Vector2Int(i, j + 1));
                        var rectTransform = boardTile.GetComponent<RectTransform>();
                        rectTransform.anchoredPosition = Vector2.zero;

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
                var tile = _tileSlotMatrix[i, k].InstantiateTile(tilePrefab);

                tile.SetTileIndex(new Vector2Int(i, k));
                tile.OnTileClicked += OnTileClicked;
            }
        }
    }

    private void OnTileClicked(Tile tile)
    {
        if (_selectedTile == null)
        {
            tile.SetAsSelected(true);
            _selectedTile = tile;
        }
        else
        {
            if (_selectedTile == tile)
            {
                _selectedTile.SetAsSelected(false);
                _selectedTile = null;
            }
            else
            {
                if (CanSwitch(_selectedTile, tile))
                {
                    SwitchTiles(_selectedTile, tile);
                    _selectedTile.SetAsSelected(false);
                    _selectedTile = null;
                    StartCoroutine(ClearingLoop());
                }
                else
                {
                    _selectedTile.SetAsSelected(false);
                    _selectedTile = null;
                }

            }
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

        _tilesMatrix.Swap(index1, index2);
    }
}
