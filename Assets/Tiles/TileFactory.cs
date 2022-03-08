using UnityEngine;

public class TileFactory : MonoBehaviour
{
    [SerializeField] private GameObject _redCircle;
    [SerializeField] private GameObject _greenCircle;
    [SerializeField] private GameObject _blueCircle;
    [SerializeField] private GameObject _yellowCircle;
    [SerializeField] private GameObject _purpleCircle;
    [SerializeField] private GameObject _errorTile;

    public GameObject GetTile(TileType type)
    {
        switch (type)
        {
            case TileType.RedCircle:
                return _redCircle;
            case TileType.GreenCircle:
                return _greenCircle;
            case TileType.BlueCircle:
                return _blueCircle;
            case TileType.YellowCircle:
                return _yellowCircle;
            case TileType.PurpleCircle:
                return _purpleCircle;
        }

        return _errorTile;
    }
}
