using UnityEngine;

public class TileSlot : MonoBehaviour
{
    public Tile InstantiateTile(GameObject tile)
    {
        return Instantiate(tile, transform).GetComponent<Tile>();
    }

    public void Clear()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}



