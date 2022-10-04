using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePlayer : MonoBehaviour
{
    public ParticleSystem[] allparticles;
    public float lifeTime = 1f;

    void Start()
    {
        allparticles = GetComponentsInChildren<ParticleSystem>();
        Destroy(gameObject, lifeTime);
    }

    public void Play()
    {
        foreach (ParticleSystem p in allparticles)
        {
            p.Stop();
            p.Play();
        }
    }
}
