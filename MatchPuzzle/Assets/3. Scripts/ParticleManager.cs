using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject clearFXPrefab;
    public GameObject breakFXPrefab;
    public GameObject doubleBreakFXPrefab;

    public void ClearPieceFXAt(int x, int y, int z = 0)
    {
        if (clearFXPrefab != null)
        {
            GameObject clearFx = Instantiate(clearFXPrefab, new Vector3(x, y, z), Quaternion.identity);

            ParticlePlayer particlePlayer = clearFx.GetComponent<ParticlePlayer>();

            if (particlePlayer != null)
            {
                particlePlayer.Play();
            }
        }
    }

    public void BreakTileFXAt(int breakValue, int x, int y, int z = 0)
    {
        GameObject breakFX = null;
        ParticlePlayer particlePlayer = null;

        if (breakValue > 1)
        {
            if (doubleBreakFXPrefab != null)
            {
                breakFX = Instantiate(doubleBreakFXPrefab, new Vector3(x, y, z), Quaternion.identity);
            }
        }
        else
        {
            if (breakFXPrefab != null)
            {
                breakFX = Instantiate(breakFXPrefab, new Vector3(x, y, z), Quaternion.identity);
            }
        }

        if (breakFX)
        {
            particlePlayer = breakFX.GetComponent<ParticlePlayer>();

            if (particlePlayer != null)
            {
                particlePlayer.Play();
                //particlePlayer?.Play();   // 이 세줄을 한번에 C# 물음표 널 연산자임.
            }
        }
    }





}
