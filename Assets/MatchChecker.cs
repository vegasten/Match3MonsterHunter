using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MatchChecker
{
    public List<Match> FindFiveInARow(TileData[,] tilesMatrix, int numberOfRows, int numberOfColumns)
    {
        var matches = new List<Match>();

        // Horizontal
        for (int j = 0; j < numberOfRows; j++)
        {
            for (int i = 0; i < numberOfColumns - 4; i++)
            {
                int count = 0;
                for (int k = 0; k < 4; k++)
                {
                    var tile = tilesMatrix[i + k, j];
                    var nextTile = tilesMatrix[i + k + 1, j];

                    if (tile.IsUsed || nextTile.IsUsed || tile.Type != nextTile.Type)
                    {
                        break;
                    }
                    count++;
                    if (count == 4)
                    {
                        var match = new Match();
                        match.TileType = tile.Type;
                        match.MatchType = MatchType.FiveInARow;
                        match.Index = new List<Vector2Int>();
                        match.SpecialIndex = new Vector2Int(i + 2, j);

                        for (int l = 0; l < 5; l++)
                        {
                            match.Index.Add(new Vector2Int(i + l, j));
                            tilesMatrix[i + l, j].IsUsed = true;
                        }

                        matches.Add(match);
                    }
                }
            }
        }

        // Vertical
        for (int i = 0; i < numberOfColumns; i++)
        {
            for (int j = 0; j < numberOfRows - 4; j++)
            {
                int count = 0;
                for (int k = 0; k < 4; k++)
                {
                    var tile = tilesMatrix[i, j + k];
                    var nextTile = tilesMatrix[i, j + k + 1];

                    if (tile.IsUsed || nextTile.IsUsed || tile.Type != nextTile.Type)
                    {
                        break;
                    }
                    count++;
                    if (count == 4)
                    {
                        var match = new Match();
                        match.TileType = tile.Type;
                        match.MatchType = MatchType.FiveInARow;
                        match.Index = new List<Vector2Int>();
                        match.SpecialIndex = new Vector2Int(i, j + 2);

                        for (int l = 0; l < 5; l++)
                        {
                            match.Index.Add(new Vector2Int(i, j + l));
                            tilesMatrix[i, j + l].IsUsed = true;
                        }

                        matches.Add(match);
                    }
                }
            }
        }
        return matches;
    }

    public List<Match> FindPlusShapes(TileData[,] tilesMatrix, int numberOfRows, int numberOfColumns)
    {
        var matches = new List<Match>();

        for (int i = 1; i < numberOfColumns - 1; i++)
        {
            for (int j = 1; j < numberOfRows - 1; j++)
            {
                var tileCenter = tilesMatrix[i, j];
                var tileLeft = tilesMatrix[i - 1, j];
                var tileRight = tilesMatrix[i + 1, j];
                var tileUp = tilesMatrix[i, j - 1];
                var tileDown = tilesMatrix[i, j + 1];

                var tiles = new List<TileData>() { tileCenter, tileLeft, tileRight, tileUp, tileDown };

                if (tiles.All(t => !t.IsUsed && t.Type == tileCenter.Type))
                {
                    var match = new Match();
                    match.MatchType = MatchType.PlusShape;
                    match.TileType = tileCenter.Type;
                    match.Index = new List<Vector2Int>();
                    match.SpecialIndex = tileCenter.TileIndex;
                    foreach (var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Index.Add(tile.TileIndex);
                    }
                    matches.Add(match);
                }
            }
        }
        return matches;
    }

    public List<Match> FindTShapes(TileData[,] tilesMatrix, int numberOfRows, int numberOfColumns)
    {
        var matches = new List<Match>();

        for (int j = 1; j < numberOfRows - 1; j++)
        {
            for (int i = 0; i < numberOfColumns - 2; i++)
            {

                var tileFirst = tilesMatrix[i, j];
                var tileMiddle = tilesMatrix[i + 1, j];
                var tileLast = tilesMatrix[i + 2, j];

                var tiles = new List<TileData>() { tileFirst, tileMiddle, tileLast };

                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var tileFirstUp = tilesMatrix[i, j - 1];
                    var tileFirstDown = tilesMatrix[i, j + 1];

                    if (!tileFirstUp.IsUsed && tileFirstUp.Type == tileFirst.Type && !tileFirstDown.IsUsed && tileFirstDown.Type == tileFirst.Type)
                    {
                        tiles.Add(tileFirstUp);
                        tiles.Add(tileFirstDown);

                        var match = new Match();
                        match.MatchType = MatchType.TShape;
                        match.TileType = tileFirst.Type;
                        match.Index = new List<Vector2Int>();
                        match.SpecialIndex = tileFirst.TileIndex;
                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Index.Add(tile.TileIndex);
                        }
                        matches.Add(match);
                        break;
                    }

                    var tileLastUp = tilesMatrix[i + 2, j - 1];
                    var tileLastDown = tilesMatrix[i + 2, j + 1];

                    if (!tileLastUp.IsUsed && tileLastUp.Type == tileFirst.Type && !tileLastDown.IsUsed && tileLastDown.Type == tileFirst.Type)
                    {
                        tiles.Add(tileLastUp);
                        tiles.Add(tileLastDown);

                        var match = new Match();
                        match.MatchType = MatchType.TShape;
                        match.TileType = tileFirst.Type;
                        match.Index = new List<Vector2Int>();
                        match.SpecialIndex = tileLast.TileIndex;
                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Index.Add(tile.TileIndex);
                        }
                        matches.Add(match);
                        break;
                    }
                }

            }
        }

        for (int i = 1; i < numberOfColumns - 1; i++)
        {
            for (int j = 0; j < numberOfRows - 2; j++)
            {

                var tileFirst = tilesMatrix[i, j];
                var tileMiddle = tilesMatrix[i, j + 1];
                var tileLast = tilesMatrix[i, j + 2];

                var tiles = new List<TileData>() { tileFirst, tileMiddle, tileLast };

                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var tileFirstLeft = tilesMatrix[i - 1, j];
                    var tileFirstRight = tilesMatrix[i + 1, j];

                    if (!tileFirstLeft.IsUsed && tileFirstLeft.Type == tileFirst.Type && !tileFirstRight.IsUsed && tileFirstRight.Type == tileFirst.Type)
                    {
                        tiles.Add(tileFirstLeft);
                        tiles.Add(tileFirstRight);

                        var match = new Match();
                        match.MatchType = MatchType.TShape;
                        match.TileType = tileFirst.Type;
                        match.Index = new List<Vector2Int>();
                        match.SpecialIndex = tileFirst.TileIndex;
                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Index.Add(tile.TileIndex);
                        }
                        matches.Add(match);
                        break;
                    }

                    var tileLastLeft = tilesMatrix[i - 1, j + 2];
                    var tileLastRight = tilesMatrix[i + 1, j + 2];

                    if (!tileLastLeft.IsUsed && tileLastLeft.Type == tileFirst.Type && !tileLastRight.IsUsed && tileLastRight.Type == tileFirst.Type)
                    {
                        tiles.Add(tileLastLeft);
                        tiles.Add(tileLastRight);

                        var match = new Match();
                        match.MatchType = MatchType.TShape;
                        match.TileType = tileFirst.Type;
                        match.Index = new List<Vector2Int>();
                        match.SpecialIndex = tileLast.TileIndex;
                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Index.Add(tile.TileIndex);
                        }
                        matches.Add(match);
                        break;
                    }
                }

            }
        }
        return matches;
    }

    public List<Match> FindLShapes(TileData[,] tilesMatrix, int numberOfRows, int numberOfColumns)
    {
        var matches = new List<Match>();

        for (int j = 0; j < numberOfRows - 2; j++)
        {
            for (int i = 0; i < numberOfColumns - 2; i++)
            {
                var tileFirst = tilesMatrix[i, j];
                var tileSecond = tilesMatrix[i + 1, j];
                var tileThird = tilesMatrix[i + 2, j];

                var tiles = new List<TileData>() { tileFirst, tileSecond, tileThird };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var tileFirstDown = tilesMatrix[i, j + 1];
                    var tileFirstDownDown = tilesMatrix[i, j + 2];

                    if (!tileFirstDown.IsUsed && tileFirstDown.Type == tileFirst.Type && !tileFirstDownDown.IsUsed && tileFirstDownDown.Type == tileFirst.Type)
                    {
                        tiles.Add(tileFirstDown);
                        tiles.Add(tileFirstDownDown);

                        var match = new Match();
                        match.MatchType = MatchType.LShape;
                        match.TileType = tileFirst.Type;
                        match.Index = new List<Vector2Int>();
                        match.SpecialIndex = tileFirst.TileIndex;

                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Index.Add(tile.TileIndex);
                        }
                        matches.Add(match);
                        break;
                    }

                    var tileLastDown = tilesMatrix[i + 2, j + 1];
                    var tileLastDownDown = tilesMatrix[i + 2, j + 2];

                    if (!tileLastDown.IsUsed && tileLastDown.Type == tileFirst.Type && !tileLastDownDown.IsUsed && tileLastDownDown.Type == tileFirst.Type)
                    {
                        tiles.Add(tileLastDown);
                        tiles.Add(tileLastDownDown);

                        var match = new Match();
                        match.MatchType = MatchType.LShape;
                        match.TileType = tileFirst.Type;
                        match.Index = new List<Vector2Int>();
                        match.SpecialIndex = tileThird.TileIndex;

                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Index.Add(tile.TileIndex);
                        }
                        matches.Add(match);
                        break;
                    }
                }
            }
        }


        for (int j = 2; j < numberOfRows; j++)
        {
            for (int i = 0; i < numberOfColumns - 2; i++)
            {
                var tileFirst = tilesMatrix[i, j];
                var tileSecond = tilesMatrix[i + 1, j];
                var tileThird = tilesMatrix[i + 2, j];

                var tiles = new List<TileData>() { tileFirst, tileSecond, tileThird };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var tileFirstUp = tilesMatrix[i, j - 1];
                    var tileFirstUpUp = tilesMatrix[i, j - 2];

                    if (!tileFirstUp.IsUsed && tileFirstUp.Type == tileFirst.Type && !tileFirstUpUp.IsUsed && tileFirstUpUp.Type == tileFirst.Type)
                    {
                        tiles.Add(tileFirstUp);
                        tiles.Add(tileFirstUpUp);

                        var match = new Match();
                        match.MatchType = MatchType.LShape;
                        match.TileType = tileFirst.Type;
                        match.Index = new List<Vector2Int>();
                        match.SpecialIndex = tileFirst.TileIndex;

                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Index.Add(tile.TileIndex);
                        }
                        matches.Add(match);
                        break;
                    }

                    var tileLastUp = tilesMatrix[i + 2, j - 1];
                    var tileLastUpUp = tilesMatrix[i + 2, j - 2];

                    if (!tileLastUp.IsUsed && tileLastUp.Type == tileFirst.Type && !tileLastUpUp.IsUsed && tileLastUpUp.Type == tileFirst.Type)
                    {
                        tiles.Add(tileLastUp);
                        tiles.Add(tileLastUpUp);

                        var match = new Match();
                        match.MatchType = MatchType.LShape;
                        match.TileType = tileFirst.Type;
                        match.Index = new List<Vector2Int>();
                        match.SpecialIndex = tileThird.TileIndex;

                        foreach (var tile in tiles)
                        {
                            tile.IsUsed = true;
                            match.Index.Add(tile.TileIndex);
                        }
                        matches.Add(match);
                        break;
                    }
                }
            }
        }
        return matches;
    }

    public List<Match> FindFourInARow(TileData[,] tilesMatrix, int numberOfRows, int numberOfColumns)
    {
        var matches = new List<Match>();

        // Horizontal
        for (int j = 0; j < numberOfRows; j++)
        {
            for (int i = 0; i < numberOfColumns - 3; i++)
            {
                var tileFirst = tilesMatrix[i, j];
                var tileSecond = tilesMatrix[i + 1, j];
                var tileThird = tilesMatrix[i + 2, j];
                var tileFourth = tilesMatrix[i + 3, j];

                var tiles = new List<TileData>() { tileFirst, tileSecond, tileThird, tileFourth };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var match = new Match();
                    match.MatchType = MatchType.FourInARow;
                    match.TileType = tileFirst.Type;
                    match.Index = new List<Vector2Int>();
                    match.SpecialIndex = tileSecond.TileIndex;
                    foreach (var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Index.Add(tile.TileIndex);
                    }
                    matches.Add(match);
                }
            }
        }

        // Vertical
        for (int i = 0; i < numberOfColumns; i++)
        {
            for (int j = 0; j < numberOfRows - 3; j++)
            {
                var tileFirst = tilesMatrix[i, j];
                var tileSecond = tilesMatrix[i, j + 1];
                var tileThird = tilesMatrix[i, j + 2];
                var tileFourth = tilesMatrix[i, j + 3];

                var tiles = new List<TileData>() { tileFirst, tileSecond, tileThird, tileFourth };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var match = new Match();
                    match.MatchType = MatchType.FourInARow;
                    match.TileType = tileFirst.Type;
                    match.Index = new List<Vector2Int>();
                    match.SpecialIndex = tileSecond.TileIndex;

                    foreach (var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Index.Add(tile.TileIndex);
                    }
                    matches.Add(match);
                }
            }
        }
        return matches;
    }

    public List<Match> FindThreeInARow(TileData[,] tilesMatrix, int numberOfRows, int numberOfColumns)
    {
        var matches = new List<Match>();

        // Horizontal
        for (int j = 0; j < numberOfRows; j++)
        {
            for (int i = 0; i < numberOfColumns - 2; i++)
            {
                var tileFirst = tilesMatrix[i, j];
                var tileSecond = tilesMatrix[i + 1, j];
                var tileThird = tilesMatrix[i + 2, j];

                var tiles = new List<TileData>() { tileFirst, tileSecond, tileThird };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var match = new Match();
                    match.MatchType = MatchType.ThreeInARow;
                    match.TileType = tileFirst.Type;
                    match.Index = new List<Vector2Int>();
                    foreach (var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Index.Add(tile.TileIndex);
                    }
                    matches.Add(match);
                }
            }
        }

        // Vertical
        for (int i = 0; i < numberOfColumns; i++)
        {
            for (int j = 0; j < numberOfRows - 2; j++)
            {
                var tileFirst = tilesMatrix[i, j];
                var tileSecond = tilesMatrix[i, j + 1];
                var tileThird = tilesMatrix[i, j + 2];

                var tiles = new List<TileData>() { tileFirst, tileSecond, tileThird };
                if (tiles.All(t => !t.IsUsed && t.Type == tileFirst.Type))
                {
                    var match = new Match();
                    match.MatchType = MatchType.ThreeInARow;
                    match.TileType = tileFirst.Type;
                    match.Index = new List<Vector2Int>();
                    foreach (var tile in tiles)
                    {
                        tile.IsUsed = true;
                        match.Index.Add(tile.TileIndex);
                    }
                    matches.Add(match);
                }
            }
        }
        return matches;
    }
}
