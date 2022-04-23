using UnityEngine;

public class BattleMonster : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void TriggerAttack()
    {
        _animator.SetTrigger("AttackTrigger");
    }

    public void TriggerBigAttack()
    {
        _animator.SetTrigger("BigAttackTrigger");
    }

    public void TriggerHit()
    {
        _animator.SetTrigger("HitTrigger");
    }
}
