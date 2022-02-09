using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardMananger : MonoBehaviour
{
    // Tiles is currently 6x6
    [SerializeField] private int _numberOfColumns = 6;
    [SerializeField] private int _numberOfRows = 6;

    [SerializeField] private List<Transform> _slotColumns;

    // Different tiles
    // TODO Should be moved somewhere else
    [Header("Tiles")]
    [SerializeField] private GameObject tile1;
    [SerializeField] private GameObject tile2;
    [SerializeField] private GameObject tile3;

    [Header("Debug")]
    [SerializeField] private Button _shuffleButton;

    private TileSlot[,] _tileSlotsMatrix;
    private Tile[,] _tilesMatrix;

    private List<List<Vector2Int>> _horizontalMatches;
    private List<List<Vector2Int>> _verticalMatches;
    private List<Match> _matches;


    private Tile _selectedTile;

    private void Awake()
    {

        _shuffleButton.onClick.AddListener(Shuffle);
        //_shuffleButton.onClick.AddListener(ClearMatches);

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
        _horizontalMatches.Clear();
        _verticalMatches.Clear();

        _horizontalMatches = FindHorizontalMatches();
        _verticalMatches = FindVerticalMatches();

        foreach (var match in _horizontalMatches)
        {
            foreach (var tileIndex in match)
            {
                ClearTile(tileIndex.x, tileIndex.y);
            }
        }

        foreach (var match in _verticalMatches)
        {
            foreach (var tileIndex in match)
            {
                ClearTile(tileIndex.x, tileIndex.y);
            }
        }

    }

    private void ClearTile(int row, int col)
    {
        _tilesMatrix[row, col] = null;

        var tileSlot = _tileSlotsMatrix[row, col];
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

                    if (FindHorizontalMatches().Count == 0 && FindVerticalMatches().Count == 0)
                    {
                        SwitchTiles(_selectedTile, tile);
                    }

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
        // Order here determines priority
        //FindFiveInARow();
        FindPlusShapes();
        FindTShapes();
        FindLShapes();
        FindFourInARow();
        FindThreeInARow();


        //FindHorizontalMatches();
        //FindVerticalMatches();
    }



    private void FindFiveInARow()
    {
        // Horizontal
        for (int j = 0; j < _numberOfRows; j++)
        {
            for (int i = 0; i < _numberOfColumns - 4; i++)
            {
                int count = 0;
                for (int k = 0; k < 4; k++)
                {
                    var tile = _tilesMatrix[i + k, j];
                    var nextTile = _tilesMatrix[i + k + 1, j];

                    if (tile.IsUsed || nextTile.IsUsed || tile.Type != nextTile.Type)
                    {
                        break;
                    }
                    count++;
                    if (count == 4)
                    {
                        Debug.Log("Found horizontal 5 in a row");
                        var match = new Match();
                        match.TileType = tile.Type;
                        match.MatchType = MatchType.FiveInARow;
                        match.Coordinates = new List<Vector2Int>();

                        for (int l = 0; l < 5; l++)
                        {
                            match.Coordinates.Add(new Vector2Int(i + l, j));
                            _tilesMatrix[i + l, j].IsUsed = true;
                        }

                        _matches.Add(match);
                    }
                }
            }
        }

        // Vertical
        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows - 4; j++)
            {
                int count = 0;
                for (int k = 0; k < 4; k++)
                {
                    var tile = _tilesMatrix[i, j + k];
                    var nextTile = _tilesMatrix[i, j + k + 1];

                    if (tile.IsUsed || nextTile.IsUsed || tile.Type != nextTile.Type)
                    {
                        break;
                    }
                    count++;
                    if (count == 4)
                    {
                        Debug.Log("Found vertical 5 in a row");

                        var match = new Match();
                        match.TileType = tile.Type;
                        match.MatchType = MatchType.FiveInARow;
                        match.Coordinates = new List<Vector2Int>();

                        for (int l = 0; l < 5; l++)
                        {
                            match.Coordinates.Add(new Vector2Int(i, j + l));
                            _tilesMatrix[i, j + l].IsUsed = true;
                        }

                        _matches.Add(match);
                    }
                }
            }
        }
    }

    private void FindPlusShapes()
    {
        for (int i = 1; i < _numberOfColumns - 1; i++)
        {
            for (int j = 1; j < _numberOfRows - 1; j++)
            {
                var tileCenter = _tilesMatrix[i, j];
                var tileLeft = _tilesMatrix[i - 1, j];
                var tileRight = _tilesMatrix[i + 1, j];
                var tileUp = _tilesMatrix[i, j - 1];
                var tileDown = _tilesMatrix[i, j + 1];

                var tiles = new List<Tile>() { tileCenter, tileLeft, tileRight, tileUp, tileDown };

                if (tiles.All(t => !t.IsUsed && t.Type == tileCenter.Type))
                {
                    Debug.Log("Found a plus shape!");
                    var match = new Match();
                    match.MatchType = MatchType.PlusShape;
                    match.TileType = tileCenter.Type;
                    match.Coordinates = new List<Vector2Int>();
                    foreach(var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Coordinates.Add(tile.TileIndex);
                        Debug.Log(tile.TileIndex.ToString());
                    }
                }
            }
        }
    }

    private void FindTShapes()
    {
    }

    private void FindLShapes()
    {

    }

    private void FindFourInARow()
    {

    }

    private void FindThreeInARow()
    {

    }

    private List<List<Vector2Int>> FindHorizontalMatches()
    {
        var matches = new List<List<Vector2Int>>();

        int minNumForMatch = 3;
        int count = 1;

        for (int j = 0; j < _numberOfRows; j++)
        {
            for (int i = 0; i < _numberOfColumns;)
            {
                for (int k = 0; k < _numberOfColumns - i - 1; k++)
                {
                    if (_tilesMatrix[i + k, j].Type == _tilesMatrix[i + k + 1, j].Type)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (count >= minNumForMatch)
                {
                    List<Vector2Int> currentMatchingTiles = new List<Vector2Int>();
                    for (int n = 0; n < count; n++)
                    {
                        currentMatchingTiles.Add(new Vector2Int(i + n, j));
                    }

                    matches.Add(currentMatchingTiles);
                    Debug.Log($"Got a horizontal match: [{i},{j}] - {count} number of tiles  - {_tilesMatrix[i, j].Type}");
                }
                i += count;
                count = 1;
            }
        }

        return matches;
    }

    private List<List<Vector2Int>> FindVerticalMatches()
    {
        var matches = new List<List<Vector2Int>>();

        int minNumForMatch = 3;
        int count = 1;

        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows;)
            {
                for (int k = 0; k < _numberOfRows - j - 1; k++)
                {
                    if (_tilesMatrix[i, j + k].Type == _tilesMatrix[i, j + k + 1].Type)
                    {
                        count++;
                    }
                    else
                    {
                        break;
                    }
                }
                if (count >= minNumForMatch)
                {
                    List<Vector2Int> currentMathingTiles = new List<Vector2Int>();
                    for (int n = 0; n < count; n++)
                    {
                        currentMathingTiles.Add(new Vector2Int(i, j + n));
                    }
                    matches.Add(currentMathingTiles);
                    Debug.Log($"Got a vertical match: [{i},{j}] - {count} number of tiles  - {_tilesMatrix[i, j].Type}");
                }
                j += count;
                count = 1;
            }
        }
        return matches;
    }

}
