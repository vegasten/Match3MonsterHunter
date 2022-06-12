using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class Match
    {
        public MatchType MatchType;
        public TileType TileType;
        public List<Vector2Int> Index;
        public Vector2Int SpecialIndex;
    }
}