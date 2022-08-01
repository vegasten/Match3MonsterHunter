using System.Collections;
using UnityEngine;

public class SummoningParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _inProgressEffects;
    [SerializeField] private ParticleSystem[] _doneEffects;

    public void StartInProgressParticles()
    {
        foreach (var effect in _inProgressEffects)
        {
            effect.gameObject.SetActive(true);
            effect.Play();
        }
    }

    public void StopInPrograssParticles()
    {
        foreach (var effect in _inProgressEffects)
        {
            effect.Stop();
            effect.gameObject.SetActive(false);
        }
    }

    public void StartDoneEffects()
    {
        foreach (var effect in _doneEffects)
        {
            effect.gameObject.SetActive(true);
            effect.Play();
        }
    }

    public void StopDoneEffects()
    {
        foreach (var effect in _doneEffects)
        {
            effect.Stop();
            effect.gameObject.SetActive(false);
        }
    }
}
