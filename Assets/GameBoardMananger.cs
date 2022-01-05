using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardMananger : MonoBehaviour
{
    // Tiles is currently 6x6
    [SerializeField] private int _numberOfColumns = 6;
    [SerializeField] private int _numberOfRows = 6;

    [SerializeField] private List<Transform> _columns;

    // Different tiles
    // TODO Should be moved somewhere else
    [Header("Tiles")]
    [SerializeField] private GameObject tile1;
    [SerializeField] private GameObject tile2;
    [SerializeField] private GameObject tile3;

    [Header("Debug")]
    [SerializeField] private Button _shuffleButton;

    private Tile[,] TilesMatrix;

    private List<List<Vector2Int>> _horizontalMatches;
    private List<List<Vector2Int>> _verticalMatches;

    private Tile _selectedTile;

    private void Awake()
    {
        _shuffleButton.onClick.AddListener(Shuffle);
        TilesMatrix = new Tile[_numberOfColumns, _numberOfRows];

        _horizontalMatches = new List<List<Vector2Int>>();
        _verticalMatches = new List<List<Vector2Int>>();
    }

    private void Start()
    {
        ClearAllVisualTiles();
        SpawnRandomTiles();
    }

    private void Shuffle()
    {
        _horizontalMatches.Clear();
        _verticalMatches.Clear();

        ClearAllVisualTiles();
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
                        tile = Instantiate(tile1, _columns[i]).GetComponent<Tile>();
                        tile.SetType(TileType.RedCircle);
                        break;
                    case 1:
                        tile = Instantiate(tile2, _columns[i]).GetComponent<Tile>();
                        tile.SetType(TileType.GreenCircle);
                        break;
                    case 2:
                        tile = Instantiate(tile3, _columns[i]).GetComponent<Tile>();
                        tile.SetType(TileType.BlueCircle);
                        break;
                    default:
                        Debug.LogError("Random index out of range");
                        tile = new Tile(); // Todo Hack
                        break;
                }

                tile.SetTileIndex(new Vector2Int(i, j));
                tile.OnTileClicked += OnTileClicked;
                TilesMatrix[i, j] = tile;
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

                    if (FindHorizontalMatches().Count == 0 && FindVerticalMatches().Count  == 0)
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

            TilesMatrix[index1.x, index1.y] = tile2;
            TilesMatrix[index2.x, index2.y] = tile1;
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

            TilesMatrix[index1.x, index1.y] = tile2;
            TilesMatrix[index2.x, index2.y] = tile1;
        }
    }

    private void ClearAllVisualTiles()
    {
        foreach (var col in _columns)
        {
            foreach (Transform child in col)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }

    // TODO: check for T and L shapes
    private void CheckForMatches()
    {
        FindHorizontalMatches();
        FindVerticalMatches();
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
                    if (TilesMatrix[i + k, j].Type == TilesMatrix[i + k + 1, j].Type)
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
                    Debug.Log($"Got a horizontal match: [{i},{j}] - {count} number of tiles  - {TilesMatrix[i, j].Type}");
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
                    if (TilesMatrix[i, j + k].Type == TilesMatrix[i, j + k + 1].Type)
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
                    Debug.Log($"Got a vertical match: [{i},{j}] - {count} number of tiles  - {TilesMatrix[i, j].Type}");
                }
                j += count;
                count = 1;
            }
        }
        return matches;
    }
}
