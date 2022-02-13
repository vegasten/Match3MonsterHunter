using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardMananger : MonoBehaviour
{
    [SerializeField] private int _numberOfColumns = 6;
    [SerializeField] private int _numberOfRows = 6;
    [SerializeField] private List<Transform> _slotColumns;

    [Header("Tiles")]
    [SerializeField] private GameObject tile1;
    [SerializeField] private GameObject tile2;
    [SerializeField] private GameObject tile3;

    [Header("Debug")]
    [SerializeField] private Button _shuffleButton;
    [SerializeField] private Button _clearButton;
    [SerializeField] private Button _fallDownButton;
    [SerializeField] private Button _spawnNewButton;

    private TileSlot[,] _tileSlotsMatrix;
    private Tile[,] _tilesMatrix;

    private List<List<Vector2Int>> _horizontalMatches;
    private List<List<Vector2Int>> _verticalMatches;
    private List<Match> _matches;

    private Tile _selectedTile;

    private void Awake()
    {

        _shuffleButton.onClick.AddListener(Shuffle);
        _clearButton.onClick.AddListener(ClearMatches);
        _fallDownButton.onClick.AddListener(MakeTilesFallDown);
        _spawnNewButton.onClick.AddListener(SpawnNewTiles);

        _tileSlotsMatrix = new TileSlot[_numberOfColumns, _numberOfRows];
        _tilesMatrix = new Tile[_numberOfColumns, _numberOfRows];

        _horizontalMatches = new List<List<Vector2Int>>();
        _verticalMatches = new List<List<Vector2Int>>();
        _matches = new List<Match>();


        InitializeSlotMatrix();
    }

    private void InitializeSlotMatrix()
    {
        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows; j++)
            {
                var tileSlot = _slotColumns[i].GetChild(j).GetComponent<TileSlot>();
                _tileSlotsMatrix[i, j] = tileSlot;
            }
        }
    }


    private void Start()
    {
        ClearAllTiles();
        SpawnRandomTiles();
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
                        currentTile.SetTileIndex(new Vector2Int(i, j + 1));
                        _tilesMatrix[i, j + 1] = currentTile;

                        var boardTile = _tileSlotsMatrix[i, j].transform.GetChild(0);
                        boardTile.SetParent(_tileSlotsMatrix[i, j + 1].transform);
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
                int tileTypeIndex = Random.Range(0, 3); // TODO Connect this and enum in some way                
                Tile tile;
                switch (tileTypeIndex)
                {
                    case 0:
                        tile = Instantiate(tile1, _tileSlotsMatrix[i, k].transform).GetComponent<Tile>();
                        tile.SetType(TileType.RedCircle);
                        break;
                    case 1:
                        tile = Instantiate(tile2, _tileSlotsMatrix[i, k].transform).GetComponent<Tile>();
                        tile.SetType(TileType.GreenCircle);
                        break;
                    case 2:
                        tile = Instantiate(tile3, _tileSlotsMatrix[i, k].transform).GetComponent<Tile>();
                        tile.SetType(TileType.BlueCircle);
                        break;
                    default:
                        Debug.LogError("Random index out of range");
                        tile = new Tile(); // Todo Hack
                        break;
                }

                tile.SetTileIndex(new Vector2Int(i, k));
                tile.OnTileClicked += OnTileClicked;
                _tilesMatrix[i, k] = tile;
            }
        }

        CheckForMatches();
    }

    private void ClearTile(Vector2Int coordinate)
    {
        _tilesMatrix[coordinate.x, coordinate.y] = null;

        var tileSlot = _tileSlotsMatrix[coordinate.x, coordinate.y];
        if (tileSlot.transform.childCount >= 1)
        {
            Destroy(tileSlot.transform.GetChild(0).gameObject);
        }
    }

    private void Shuffle()
    {
        _horizontalMatches.Clear();
        _verticalMatches.Clear();

        ClearAllTiles();
        SpawnRandomTiles();

        CheckForMatches();
    }

    private void SpawnRandomTiles()
    {
        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows; j++)
            {
                int tileTypeIndex = Random.Range(0, 3); // TODO Connect this and enum in some way                
                Tile tile;
                switch (tileTypeIndex)
                {
                    case 0:
                        tile = Instantiate(tile1, _tileSlotsMatrix[i, j].transform).GetComponent<Tile>();
                        tile.SetType(TileType.RedCircle);
                        break;
                    case 1:
                        tile = Instantiate(tile2, _tileSlotsMatrix[i, j].transform).GetComponent<Tile>();
                        tile.SetType(TileType.GreenCircle);
                        break;
                    case 2:
                        tile = Instantiate(tile3, _tileSlotsMatrix[i, j].transform).GetComponent<Tile>();
                        tile.SetType(TileType.BlueCircle);
                        break;
                    default:
                        Debug.LogError("Random index out of range");
                        tile = new Tile(); // Todo Hack
                        break;
                }

                tile.SetTileIndex(new Vector2Int(i, j));
                tile.OnTileClicked += OnTileClicked;
                _tilesMatrix[i, j] = tile;
            }
        }
    }

    private void OnTileClicked(Tile tile)
    {
        var tileIndex = tile.TileIndex;

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

                    //if (FindHorizontalMatches().Count == 0 && FindVerticalMatches().Count == 0)
                    //{
                    //    SwitchTiles(_selectedTile, tile);
                    //}

                    _selectedTile.SetAsSelected(false);
                    _selectedTile = null;
                }
                else
                {
                    _selectedTile.SetAsSelected(false);
                    tile.SetAsSelected(true);
                    _selectedTile = tile;
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
            return true;
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

    private void SwitchTiles(Tile tile1, Tile tile2)
    {
        var index1 = tile1.TileIndex;
        var index2 = tile2.TileIndex;

        if (index1.x == index2.x) // Vertical switch
        {
            tile1.transform.SetSiblingIndex(index2.y);
            tile2.transform.SetSiblingIndex(index1.y);

            tile1.SetTileIndex(new Vector2Int(index1.x, index2.y));
            tile2.SetTileIndex(new Vector2Int(index2.x, index1.y));

            _tilesMatrix[index1.x, index1.y] = tile2;
            _tilesMatrix[index2.x, index2.y] = tile1;
        }
        else // Horizontal switch
        {
            var col1 = tile1.transform.parent;
            var col2 = tile2.transform.parent;

            tile1.transform.SetParent(col2);
            tile1.transform.SetSiblingIndex(index2.y);
            tile2.transform.SetParent(col1);
            tile2.transform.SetSiblingIndex(index1.y);

            tile1.SetTileIndex(new Vector2Int(index2.x, index1.y));
            tile2.SetTileIndex(new Vector2Int(index1.x, index2.y));

            _tilesMatrix[index1.x, index1.y] = tile2;
            _tilesMatrix[index2.x, index2.y] = tile1;
        }
    }

    private void ClearAllTiles()
    {
        foreach (var column in _slotColumns)
        {
            foreach (Transform tileSlot in column)
            {
                if (tileSlot.transform.childCount >= 1)
                {
                    GameObject.Destroy(tileSlot.GetChild(0).gameObject);
                }

            }
        }
    }

    // TODO: check for T and L shapes
    private void CheckForMatches()
    {

        var matchChecker = new MatchChecker();
        // Order here determines priority
        _matches.AddRange(matchChecker.FindFiveInARow(_tilesMatrix, _numberOfRows, _numberOfColumns));
        _matches.AddRange(matchChecker.FindPlusShapes(_tilesMatrix, _numberOfRows, _numberOfColumns));
        _matches.AddRange(matchChecker.FindTShapes(_tilesMatrix, _numberOfRows, _numberOfColumns));
        _matches.AddRange(matchChecker.FindLShapes(_tilesMatrix, _numberOfRows, _numberOfColumns));
        _matches.AddRange(matchChecker.FindFourInARow(_tilesMatrix, _numberOfRows, _numberOfColumns));
        _matches.AddRange(matchChecker.FindThreeInARow(_tilesMatrix, _numberOfRows, _numberOfColumns));
    }
}
