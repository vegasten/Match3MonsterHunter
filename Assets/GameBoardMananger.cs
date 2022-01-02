using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardMananger : MonoBehaviour
{
    // Tiles is currently 6x6
    [SerializeField] private int numberOfColumns = 6;
    [SerializeField] private int numberOfRows = 6;

    [SerializeField] private List<Transform> columns;

    // Different tiles
    // TODO Should be moved somewhere else
    [Header("Tiles")]
    [SerializeField] private Image tile1;
    [SerializeField] private Image tile2;
    [SerializeField] private Image tile3;

    [Header("Debug")]
    [SerializeField] private Button shuffleButton;

    private TileHolder[,] TilesMatrix;

    private void Awake()
    {
        shuffleButton.onClick.AddListener(Shuffle);
        TilesMatrix = new TileHolder[numberOfColumns, numberOfRows];
    }
    private void Start()
    {
        ClearAllVisualTiles();
        SpawnRandomTiles();
        UpdateVisuals();
    }

    private void Shuffle()
    {
        ClearAllVisualTiles();
        SpawnRandomTiles();

        UpdateVisuals();

        CheckForMatches();
    }

    private void SpawnRandomTiles()
    {
        for (int i = 0; i < numberOfColumns; i++)
        {
            for (int j = 0; j < numberOfRows; j++)
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
        for (int i = 0; i < numberOfColumns; i++)
        {
            for (int j = 0; j < numberOfRows; j++)
            {
                switch (TilesMatrix[i, j].Type)
                {
                    case TileType.RedCircle:
                        Instantiate(tile1, columns[i]);
                        break;
                    case TileType.GreenCircle:
                        Instantiate(tile2, columns[i]);
                        break;
                    case TileType.BlueCircle:
                        Instantiate(tile3, columns[i]);
                        break;
                    default:
                        Debug.LogError("Something happened with the TileType enum check in UpdateVisuals");
                        break;
                }
            }
        }
    }

    private void ClearAllVisualTiles()
    {
        foreach (var col in columns)
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

        for (int j = 0; j < numberOfRows; j++)
        {
            for (int i = 0; i < numberOfColumns;)
            {
                for (int k = 0; k < numberOfColumns - i - 1; k++)
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
                    Debug.Log($"Got a horizontal match: [{i},{j}] - {count} number of tiles  - {TilesMatrix[i, j].Type}");
                i += count;
                count = 1;
            }
        }
    }

    private void FindVerticalMatches()
    {
        int minNumForMatch = 3;
        int count = 1;

        for (int i = 0; i < numberOfColumns; i++)
        {
            for (int j = 0; j < numberOfRows;)
            {
                for (int k = 0; k < numberOfRows - j - 1; k++)
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
                    Debug.Log($"Got a vertical match: [{i},{j}] - {count} number of tiles  - {TilesMatrix[i, j].Type}");
                j += count;
                count = 1;
            }
        }
    }
}
