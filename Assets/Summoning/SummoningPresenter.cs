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

        [SerializeField] private Transform _summoningSpot;
        [SerializeField] private GameObject _summoningScreen;
        [SerializeField] private SummoningParticles _summoningParticles;
        [SerializeField] private Image _coveringImage;

        [Header("Summoning UI")]
        [SerializeField] private Button _summoningButton;
        [SerializeField] private GameObject _goBackButton;
        [SerializeField] private TMP_Text _numberOfScrollsText;

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
            _summoningParticles.StartInProgressParticles();
            yield return new WaitForSeconds(4.0f);

            _coveringImage.DOFade(1f, 1f);

            yield return new WaitForSeconds(1.0f);

            _summoningParticles.StopInPrograssParticles();
            _coveringImage.DOFade(0f, 0.5f);
            var monster = Instantiate(monsterPrefab, _summoningSpot);
            _monsterToRotate = monster.transform;

            _summoningParticles.StartDoneEffects();
            yield return new WaitForSeconds(3f);
            _summoningParticles.StopDoneEffects();
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