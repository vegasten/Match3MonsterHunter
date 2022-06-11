using UnityEngine;

[CreateAssetMenu(fileName = "Monster", menuName = "ScriptableObjects/Monsters", order = 1)]
public class MonsterScriptableObject : ScriptableObject
{
    public int Health;
    public int AttackPower;    

    public GameObject BattleGameObject;
    public Sprite StorageImage;    
}
