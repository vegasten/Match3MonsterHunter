using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public event Action<Tile, Direction> OnTileDragged;
    private Camera _uIcamera;

    private void Start()
    {
        _uIcamera = GameObject.FindGameObjectWithTag("UICamera").GetComponent<Camera>();
    }

    public Vector2Int TileIndex { get; private set; }

    public void SetTileIndex(Vector2Int index)
    {
        TileIndex = index;
    }

    private bool _isClickedActive = false;


    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.IsBoardInputEnabled)
        {
            _isClickedActive = true;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.IsBoardInputEnabled)
        {
            _isClickedActive = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (GameManager.IsBoardInputEnabled)
        {
            if (_isClickedActive)
            {
                _isClickedActive = false;

                var tileScreenPoint = _uIcamera.WorldToScreenPoint(transform.position);
                var direction = DeductDirection(tileScreenPoint, Input.mousePosition);
                OnTileDragged?.Invoke(this, direction);
            }
        }
    }

    private Direction DeductDirection(Vector3 start, Vector3 end)
    {
        var diffHorizontal = end.x - start.x;
        var diffVertival = end.y - start.y;

        if (Math.Abs(diffHorizontal) >= Math.Abs(diffVertival))
        {
            if (diffHorizontal >= 0)
            {
                return Direction.R;
            }
            else
            {
                return Direction.L;
            }
        }
        else
        {
            if (diffVertival >= 0)
            {
                return Direction.U;
            }
            else
            {
                return Direction.D;
            }
        }
    }
}
