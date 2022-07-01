using UnityEngine;

namespace Home
{
    public class SummoningController : MonoBehaviour
    {
        [SerializeField] SummoningPresenter _presenter;
        [SerializeField] MonsterList _monsterList;
        [SerializeField] StorageController _storageController;
        [SerializeField] CurrencyManager _currencyManager;

        private void Start()
        {
            // DEBUG
            //ResetToOneScroll();
            // DEBUG

            _presenter.OnSummoningButtonClicked += SummonMonster;
            int numberOfScrolls = _currencyManager.GetNumberOfBasicScrolls();
            _presenter.SetNumberOfScrolls(numberOfScrolls);
        }

        private void SummonMonster()
        {
            var monsterData = _monsterList.GetRandomMonsterSummoningDataUniform();
            _presenter.AnimateSummoning(monsterData.SummoningPrefab);
            _storageController.AddMonsterFromSummoning(monsterData);

            _currencyManager.UseBasicScroll();
            var numberOfScrolls = _currencyManager.GetNumberOfBasicScrolls();
            _presenter.SetNumberOfScrolls(numberOfScrolls);
        }

        // DEBUG DEBUG DEBUG
        // Run this to reset to one scroll
        private void ResetToOneScroll()
        {
            var currencyData = new CurrencyData()
            {
                BasicScrolls = 1
            };

            StateSaver.Instance.SaveCurrency(currencyData);
        }
    }
}