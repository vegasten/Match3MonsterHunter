using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBoardMananger : MonoBehaviour
{
    // Tiles is currently 6x6
    [SerializeField] private int numberOfColums = 6;
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
        TilesMatrix = new TileHolder[numberOfColums, numberOfRows];
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
        for (int i = 0; i < numberOfColums; i++)
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
        for (int i = 0; i < numberOfColums; i++)
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

    // TODO: Should it support more than 5 in a row???
    // TODO: check for T and L shapes
    private void CheckForMatches()
    {
        FindHorizontalMatches();
        FindVerticalMatches();
        
    }

    private void FindHorizontalMatches()
    {
        for (int j = 0; j < numberOfRows; j++)
        {
            for (int i = 0; i < numberOfColums - 2; i++)
            {
                if (TilesMatrix[i, j].Type == TilesMatrix[i + 1, j].Type)
                {
                    if (TilesMatrix[i + 1, j].Type == TilesMatrix[i + 2, j].Type)
                    {
                        Debug.Log($"Got a horizontal match: [{i},{j}] - {TilesMatrix[i, j].Type}");

                        if (i + 3 < numberOfColums)
                        {
                            if (TilesMatrix[i + 2, j].Type == TilesMatrix[i + 3, j].Type)
                            {
                                Debug.Log("It is actually 4 in a row");

                                if (i + 4 < numberOfColums)
                                {
                                    if (TilesMatrix[i + 3, j].Type == TilesMatrix[i + 4, j].Type)
                                    {
                                        Debug.Log("No wait, 5 in a row!");

                                    }
                                    i++;
                                }
                                i++;
                            }
                        }
                        i++; // No need to check the tiles that are in this current match
                        i++;
                    }
                }
            }
        }
    }

    private void FindVerticalMatches()
    {
        for (int i = 0; i < numberOfColums; i++)
        {
            for (int j = 0; j < numberOfRows - 2; j++)
            {
                if (TilesMatrix[i, j].Type == TilesMatrix[i, j + 1].Type)
                {
                    if (TilesMatrix[i, j + 1].Type == TilesMatrix[i, j + 2].Type)
                    {
                        Debug.Log($"Got a vertical match: [{i},{j}] - {TilesMatrix[i, j].Type}");



                        if (i + 3 < numberOfRows)
                        {
                            if (TilesMatrix[i, j + 2].Type == TilesMatrix[i, j + 3].Type)
                            {
                                Debug.Log("It is actually 4 in a row");

                                if (i + 4 < numberOfRows)
                                {
                                    if (TilesMatrix[i, j + 3].Type == TilesMatrix[i, j + 4].Type)
                                    {
                                        Debug.Log("No wait, 5 in a row!");

                                    }
                                    j++;
                                }
                                j++;
                            }
                        }
                        j++; // No need to check the tiles that are in this current match
                        j++;
                    }
                }
            }
        }
    }
}
