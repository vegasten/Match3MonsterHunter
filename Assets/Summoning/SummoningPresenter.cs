using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Home
{
    public class SummoningPresenter : MonoBehaviour
    {
        public event Action OnSummoningButtonClicked;

        [SerializeField] Transform _summoningSpot;
        [SerializeField] GameObject _summoningScreen;
        [SerializeField] ParticleSystem _summoningParticleSystem;

        [Header("Summoning UI")]
        [SerializeField] Button _summoningButton;
        [SerializeField] GameObject _goBackButton;
        [SerializeField] TMP_Text _numberOfScrollsText;

        [Header("Spinning")]
        [SerializeField] private float _rotationSpeed = 75f;

        private Transform _monsterToRotate;

        private void Start()
        {
            _summoningButton.onClick.AddListener(SummoningButtonClicked);
        }

        private void Update()
        {
            if (_monsterToRotate != null)
            {
                _monsterToRotate.Rotate(0, _rotationSpeed * Time.deltaTime, 0);
            }
        }

        private void SummoningButtonClicked()
        {
            OnSummoningButtonClicked?.Invoke();
        }

        public void AnimateSummoning(GameObject monsterPrefab)
        {
            _summoningScreen.SetActive(false);
            _goBackButton.SetActive(false);
            StartCoroutine(AnimateSummoningCoroutine(monsterPrefab));
        }

        private IEnumerator AnimateSummoningCoroutine(GameObject monsterPrefab)
        {
            _summoningParticleSystem.Play();
            yield return new WaitForSeconds(3.0f);
            _summoningParticleSystem.Stop();
            var monster = Instantiate(monsterPrefab, _summoningSpot);
            _monsterToRotate = monster.transform;
            yield return new WaitForSeconds(3.0f);
            _monsterToRotate = null;
            Destroy(monster);
            _summoningScreen.SetActive(true);
            _goBackButton.SetActive(true);
        }

        public void SetNumberOfScrolls(int numberOfScrolls)
        {
            _summoningButton.enabled = numberOfScrolls > 0;
            _numberOfScrollsText.text = numberOfScrolls.ToString();
        }
    }
}