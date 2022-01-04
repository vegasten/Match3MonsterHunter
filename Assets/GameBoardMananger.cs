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

    private TileHolder[,] TilesMatrix;

    private List<List<Vector2Int>> _horizontalMatches;
    private List<List<Vector2Int>> _verticalMatches;

    private Tile _selectedTile;

    private void Awake()
    {
        _shuffleButton.onClick.AddListener(Shuffle);
        TilesMatrix = new TileHolder[_numberOfColumns, _numberOfRows];

        _horizontalMatches = new List<List<Vector2Int>>();
        _verticalMatches = new List<List<Vector2Int>>();
    }

    private void Start()
    {
        ClearAllVisualTiles();
        SpawnRandomTiles();
        UpdateVisuals();
    }

    private void Shuffle()
    {
        _horizontalMatches.Clear();
        _verticalMatches.Clear();

        ClearAllVisualTiles();
        SpawnRandomTiles();

        UpdateVisuals();

        CheckForMatches();
    }

    private void SpawnRandomTiles()
    {
        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows; j++)
            {
                int tileTypeIndex = Random.Range(0, 3); // TODO Connect this and enum in some way

                var tileHolder = new TileHolder();

                switch (tileTypeIndex)
                {
                    case 0:
                        tileHolder.Type = TileType.RedCircle;
                        break;
                    case 1:
                        tileHolder.Type = TileType.GreenCircle;
                        break;
                    case 2:
                        tileHolder.Type = TileType.BlueCircle;
                        break;
                    default:
                        Debug.LogError("Random index out of range");
                        break;
                }

                TilesMatrix[i, j] = tileHolder;
            }
        }
    }

    private void UpdateVisuals()
    {
        for (int i = 0; i < _numberOfColumns; i++)
        {
            for (int j = 0; j < _numberOfRows; j++)
            {

                Tile tile;
                switch (TilesMatrix[i, j].Type)
                {
                    case TileType.RedCircle:
                        tile = Instantiate(tile1, _columns[i]).GetComponent<Tile>();
                        break;
                    case TileType.GreenCircle:
                        tile = Instantiate(tile2, _columns[i]).GetComponent<Tile>();
                        break;
                    case TileType.BlueCircle:
                        tile = Instantiate(tile3, _columns[i]).GetComponent<Tile>();
                        break;
                    default:
                        Debug.LogError("Something happened with the TileType enum check in UpdateVisuals");
                        tile = new Tile(); // Todo Hack
                        break;
                }
                tile.SetTileIndex(new Vector2Int(i, j));
                tile.OnTileClicked += OnTileClicked;
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

        Debug.Log($"Clicked tile: {tileIndex.ToString()}");
    }

    private bool CanSwitch(Tile tile1, Tile tile2)
    {
        var index1 = tile1.TileIndex;
        var index2 = tile2.TileIndex;

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

        if (index1.x == index2.x)
        {
            tile1.transform.SetSiblingIndex(index2.y);
            tile2.transform.SetSiblingIndex(index1.y);

            tile1.SetTileIndex(new Vector2Int(index1.x, index2.y));
            tile2.SetTileIndex(new Vector2Int(index2.x, index1.y));
        }
        else
        {
            var col1 = tile1.transform.parent;
            var col2 = tile2.transform.parent;

            tile1.transform.SetParent(col2);
            tile1.transform.SetSiblingIndex(index2.y);
            tile2.transform.SetParent(col1);
            tile2.transform.SetSiblingIndex(index1.y);

            tile1.SetTileIndex(new Vector2Int(index2.x, index1.y));
            tile2.SetTileIndex(new Vector2Int(index1.x, index2.y));
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

    private void FindHorizontalMatches()
    {
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

                    _horizontalMatches.Add(currentMatchingTiles);
                    Debug.Log($"Got a horizontal match: [{i},{j}] - {count} number of tiles  - {TilesMatrix[i, j].Type}");
                }
                i += count;
                count = 1;
            }
        }
    }

    private void FindVerticalMatches()
    {
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

                    Debug.Log($"Got a vertical match: [{i},{j}] - {count} number of tiles  - {TilesMatrix[i, j].Type}");
                }
                j += count;
                count = 1;
            }
        }
    }
}
