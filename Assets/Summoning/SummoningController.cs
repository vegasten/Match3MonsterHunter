using UnityEngine;

namespace Home
{
    public class SummoningController : MonoBehaviour
    {
        [SerializeField] SummoningPresenter _presenter;
        [SerializeField] MonsterList _monsterList;
        [SerializeField] StorageController _storageController;

        private void Start()
        {
            _presenter.OnSummoningButtonClicked += SummonMonster;
        }

        private void SummonMonster()
        {
            var monsterData = _monsterList.GetRandomMonsterSummoningDataUniform();
            _presenter.AnimateSummoning(monsterData.SummoningPrefab);
            _storageController.AddMonsterFromSummoning(monsterData);
        }
    }
}