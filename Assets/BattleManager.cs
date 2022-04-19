using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public void RegisterCombo(int numberOfCombos, int numberOfTiles)
    {
        Debug.Log($"Registered: {numberOfCombos} combos consisting of {numberOfTiles} tiles!");
    }
}
