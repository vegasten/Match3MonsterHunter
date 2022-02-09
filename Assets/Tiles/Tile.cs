using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public event Action<Tile> OnTileClicked;

    public Vector2Int TileIndex { get; private set; }
    public TileType Type { get; private set; }
    public bool IsUsed = false;

    private Image _image;

    [SerializeField] private Color _unselectedColor;
    [SerializeField] private Color _selectedColor;

    public void SetTileIndex(Vector2Int index)
    {
        TileIndex = index;
    }

    public void SetType(TileType type)
    {
        Type = type;
    }

    private void Awake()
    {
        _image = GetComponent<Image>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnTileClicked?.Invoke(this);
    }

    public void SetAsSelected(bool IsSelected)
    {
        if (IsSelected)
        {
            _image.color = _selectedColor;
        }
        else
        {
            _image.color = _unselectedColor;
        }
    }
}
