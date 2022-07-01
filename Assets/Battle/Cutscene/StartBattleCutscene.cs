using Cinemachine;
using DG.Tweening;
using System;
using UnityEngine;

public class StartBattleCutscene : MonoBehaviour
{
    public event Action OnCutSceneComplete;

    [SerializeField] private CinemachineVirtualCamera _virtualCam;

    public void StartCutScene()
    {
        var dollyCam = _virtualCam.GetCinemachineComponent<CinemachineTrackedDolly>();

        float pathPosition = 0f;
        DOTween.To(() => pathPosition, x => pathPosition = x, 1.0f, 4.0f)
            .SetEase(Ease.InOutSine)
            .OnUpdate(() => { dollyCam.m_PathPosition = pathPosition; })
            .OnComplete(() => OnCutSceneComplete?.Invoke());            
    }
}
