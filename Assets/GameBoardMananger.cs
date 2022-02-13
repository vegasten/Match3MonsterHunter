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
        // Order here determines priority
        FindFiveInARow();
        FindPlusShapes();
        FindTShapes();
        FindLShapes();
        FindFourInARow();
        FindThreeInARow();
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
                        match.SpecialCoordinate = new Vector2Int(i + 2, j);

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
                        match.SpecialCoordinate = new Vector2Int(i, j + 2);

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
                    match.SpecialCoordinate = tileCenter.TileIndex;
                    foreach (var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Coordinates.Add(tile.TileIndex);
                    }
                    _matches.Add(match);
                }
            }
        }
    }

    private void FindTShapes()
    {
        for (int j = 1; j < _numberOfRows - 1; j++)
        {
            for (int i = 0; i < _numberOfColumns - 2; i++)
            {

                var tileFirst = _tilesMatrix[i, j];
                var tileMiddle = _tilesMatrix[i + 1, j];
                var tileLast = _tilesMatrix[i + 2, j];

                var tiles = new List<Tile>() { tileFirst, tileMiddle, tileLast };

                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var tileFirstUp = _tilesMatrix[i, j - 1];
                    var tileFirstDown = _tilesMatrix[i, j + 1];

                    if (!tileFirstUp.IsUsed && tileFirstUp.Type == tileFirst.Type && !tileFirstDown.IsUsed && tileFirstDown.Type == tileFirst.Type)
                    {
                        tiles.Add(tileFirstUp);
                        tiles.Add(tileFirstDown);

                        var match = new Match();
                        match.MatchType = MatchType.TShape;
                        match.TileType = tileFirst.Type;
                        match.Coordinates = new List<Vector2Int>();
                        match.SpecialCoordinate = tileFirst.TileIndex;
                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Coordinates.Add(tile.TileIndex);
                        }
                        _matches.Add(match);
                        break;
                    }

                    var tileLastUp = _tilesMatrix[i + 2, j - 1];
                    var tileLastDown = _tilesMatrix[i + 2, j + 1];

                    if (!tileLastUp.IsUsed && tileLastUp.Type == tileFirst.Type && !tileLastDown.IsUsed && tileLastDown.Type == tileFirst.Type)
                    {
                        Debug.Log("Found a T shape");

                        tiles.Add(tileLastUp);
                        tiles.Add(tileLastDown);

                        var match = new Match();
                        match.MatchType = MatchType.TShape;
                        match.TileType = tileFirst.Type;
                        match.Coordinates = new List<Vector2Int>();
                        match.SpecialCoordinate = tileLast.TileIndex;
                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Coordinates.Add(tile.TileIndex);
                        }
                        _matches.Add(match);
                        break;
                    }
                }

            }
        }

        for (int i = 1; i < _numberOfColumns - 1; i++)
        {
            for (int j = 0; j < _numberOfRows - 2; j++)
            {

                var tileFirst = _tilesMatrix[i, j];
                var tileMiddle = _tilesMatrix[i, j + 1];
                var tileLast = _tilesMatrix[i, j + 2];

                var tiles = new List<Tile>() { tileFirst, tileMiddle, tileLast };

                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var tileFirstLeft = _tilesMatrix[i - 1, j];
                    var tileFirstRight = _tilesMatrix[i + 1, j];

                    if (!tileFirstLeft.IsUsed && tileFirstLeft.Type == tileFirst.Type && !tileFirstRight.IsUsed && tileFirstRight.Type == tileFirst.Type)
                    {
                        Debug.Log("Found a T shape");

                        tiles.Add(tileFirstLeft);
                        tiles.Add(tileFirstRight);

                        var match = new Match();
                        match.MatchType = MatchType.TShape;
                        match.TileType = tileFirst.Type;
                        match.Coordinates = new List<Vector2Int>();
                        match.SpecialCoordinate = tileFirst.TileIndex;
                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Coordinates.Add(tile.TileIndex);
                        }
                        _matches.Add(match);
                        break;
                    }

                    var tileLastLeft = _tilesMatrix[i - 1, j + 2];
                    var tileLastRight = _tilesMatrix[i + 1, j + 2];

                    if (!tileLastLeft.IsUsed && tileLastLeft.Type == tileFirst.Type && !tileLastRight.IsUsed && tileLastRight.Type == tileFirst.Type)
                    {
                        Debug.Log("Found a T shape");

                        tiles.Add(tileLastLeft);
                        tiles.Add(tileLastRight);

                        var match = new Match();
                        match.MatchType = MatchType.TShape;
                        match.TileType = tileFirst.Type;
                        match.Coordinates = new List<Vector2Int>();
                        match.SpecialCoordinate = tileLast.TileIndex;
                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Coordinates.Add(tile.TileIndex);
                        }
                        _matches.Add(match);
                        break;
                    }
                }

            }
        }
    }

    private void FindLShapes()
    {
        for (int j = 0; j < _numberOfRows - 2; j++)
        {
            for (int i = 0; i < _numberOfColumns - 2; i++)
            {
                var tileFirst = _tilesMatrix[i, j];
                var tileSecond = _tilesMatrix[i + 1, j];
                var tileThird = _tilesMatrix[i + 2, j];

                var tiles = new List<Tile>() { tileFirst, tileSecond, tileThird };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var tileFirstDown = _tilesMatrix[i, j + 1];
                    var tileFirstDownDown = _tilesMatrix[i, j + 2];

                    if (!tileFirstDown.IsUsed && tileFirstDown.Type == tileFirst.Type && !tileFirstDownDown.IsUsed && tileFirstDownDown.Type == tileFirst.Type)
                    {
                        Debug.Log("Found L shape");

                        tiles.Add(tileFirstDown);
                        tiles.Add(tileFirstDownDown);

                        var match = new Match();
                        match.MatchType = MatchType.LShape;
                        match.TileType = tileFirst.Type;
                        match.Coordinates = new List<Vector2Int>();
                        match.SpecialCoordinate = tileFirst.TileIndex;

                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Coordinates.Add(tile.TileIndex);
                        }
                        _matches.Add(match);
                        break;
                    }

                    var tileLastDown = _tilesMatrix[i + 2, j + 1];
                    var tileLastDownDown = _tilesMatrix[i + 2, j + 2];

                    if (!tileLastDown.IsUsed && tileLastDown.Type == tileFirst.Type && !tileLastDownDown.IsUsed && tileLastDownDown.Type == tileFirst.Type)
                    {
                        Debug.Log("Found L shape");

                        tiles.Add(tileLastDown);
                        tiles.Add(tileLastDownDown);

                        var match = new Match();
                        match.MatchType = MatchType.LShape;
                        match.TileType = tileFirst.Type;
                        match.Coordinates = new List<Vector2Int>();
                        match.SpecialCoordinate = tileThird.TileIndex;

                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Coordinates.Add(tile.TileIndex);
                        }
                        _matches.Add(match);
                        break;
                    }
                }
            }
        }


        for (int j = 2; j < _numberOfRows; j++)
        {
            for (int i = 0; i < _numberOfColumns - 2; i++)
            {
                var tileFirst = _tilesMatrix[i, j];
                var tileSecond = _tilesMatrix[i + 1, j];
                var tileThird = _tilesMatrix[i + 2, j];

                var tiles = new List<Tile>() { tileFirst, tileSecond, tileThird };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var tileFirstUp = _tilesMatrix[i, j - 1];
                    var tileFirstUpUp = _tilesMatrix[i, j - 2];

                    if (!tileFirstUp.IsUsed && tileFirstUp.Type == tileFirst.Type && !tileFirstUpUp.IsUsed && tileFirstUpUp.Type == tileFirst.Type)
                    {
                        Debug.Log("Found L shape");

                        tiles.Add(tileFirstUp);
                        tiles.Add(tileFirstUpUp);

                        var match = new Match();
                        match.MatchType = MatchType.LShape;
                        match.TileType = tileFirst.Type;
                        match.Coordinates = new List<Vector2Int>();
                        match.SpecialCoordinate = tileFirst.TileIndex;

                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Coordinates.Add(tile.TileIndex);
                        }
                        _matches.Add(match);
                        break;
                    }

                    var tileLastUp = _tilesMatrix[i + 2, j - 1];
                    var tileLastUpUp = _tilesMatrix[i + 2, j - 2];

                    if (!tileLastUp.IsUsed && tileLastUp.Type == tileFirst.Type && !tileLastUpUp.IsUsed && tileLastUpUp.Type == tileFirst.Type)
                    {
                        Debug.Log("Found L shape");

                        tiles.Add(tileLastUp);
                        tiles.Add(tileLastUpUp);

                        var match = new Match();
                        match.MatchType = MatchType.LShape;
                        match.TileType = tileFirst.Type;
                        match.Coordinates = new List<Vector2Int>();
                        match.SpecialCoordinate = tileThird.TileIndex;

                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Coordinates.Add(tile.TileIndex);
                        }
                        _matches.Add(match);
                        break;
                    }
                }
            }
        }
    }

    private void FindFourInARow()
    {
        // Horizontal
        for (int j = 0; j < _numberOfRows; j++)
        {
            for (int i = 0; i < _numberOfColumns - 3; i++)
            {
                var tileFirst = _tilesMatrix[i, j];
                var tileSecond = _tilesMatrix[i + 1, j];
                var tileThird = _tilesMatrix[i + 2, j];
                var tileFourth = _tilesMatrix[i + 3, j];

                var tiles = new List<Tile>() { tileFirst, tileSecond, tileThird, tileFourth };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var match = new Match();
                    match.MatchType = MatchType.FourInARow;
                    match.TileType = tileFirst.Type;
                    match.Coordinates = new List<Vector2Int>();
                    match.SpecialCoordinate = tileSecond.TileIndex;
                    foreach (var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Coordinates.Add(tile.TileIndex);
                    }
                    _matches.Add(match);
                }
            }
        }

        // Vertical
        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows - 3; j++)
            {
                var tileFirst = _tilesMatrix[i, j];
                var tileSecond = _tilesMatrix[i, j + 1];
                var tileThird = _tilesMatrix[i, j + 2];
                var tileFourth = _tilesMatrix[i, j + 3];

                var tiles = new List<Tile>() { tileFirst, tileSecond, tileThird, tileFourth };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var match = new Match();
                    match.MatchType = MatchType.FourInARow;
                    match.TileType = tileFirst.Type;
                    match.Coordinates = new List<Vector2Int>();
                    match.SpecialCoordinate = tileSecond.TileIndex;

                    foreach (var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Coordinates.Add(tile.TileIndex);
                    }
                    _matches.Add(match);
                }
            }
        }
    }

    private void FindThreeInARow()
    {
        // Horizontal
        for (int j = 0; j < _numberOfRows; j++)
        {
            for (int i = 0; i < _numberOfColumns - 2; i++)
            {
                var tileFirst = _tilesMatrix[i, j];
                var tileSecond = _tilesMatrix[i + 1, j];
                var tileThird = _tilesMatrix[i + 2, j];

                var tiles = new List<Tile>() { tileFirst, tileSecond, tileThird };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var match = new Match();
                    match.MatchType = MatchType.ThreeInARow;
                    match.TileType = tileFirst.Type;
                    match.Coordinates = new List<Vector2Int>();
                    foreach (var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Coordinates.Add(tile.TileIndex);
                    }
                    _matches.Add(match);
                }
            }
        }

        // Vertical
        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows - 2; j++)
            {
                var tileFirst = _tilesMatrix[i, j];
                var tileSecond = _tilesMatrix[i, j + 1];
                var tileThird = _tilesMatrix[i, j + 2];

                var tiles = new List<Tile>() { tileFirst, tileSecond, tileThird };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var match = new Match();
                    match.MatchType = MatchType.ThreeInARow;
                    match.TileType = tileFirst.Type;
                    match.Coordinates = new List<Vector2Int>();
                    foreach (var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Coordinates.Add(tile.TileIndex);
                    }
                    _matches.Add(match);
                }
            }
        }
    }
}
