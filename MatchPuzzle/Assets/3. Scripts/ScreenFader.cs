using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(MaskableGraphic))]
public class ScreenFader : MonoBehaviour
{
    public float solidAlpha = 1f;

    public float clearAlpha = 0f;

    public float delay = 0f;

    public float timeToFad = 1f;

    MaskableGraphic m_Graphic;

    private void Start()
    {
        m_Graphic = GetComponent<MaskableGraphic>();
        //FadeOff();
    }

    IEnumerator FadeRoutine(float alpha)
    {
        yield return new WaitForSeconds(delay);

        m_Graphic.CrossFadeAlpha(alpha, timeToFad, true);
    }

    public void FadeOn()
    {
        StartCoroutine(FadeRoutine(solidAlpha));
    }

    public void FadeOff()
    {
        StartCoroutine(FadeRoutine(clearAlpha));
    }
}
