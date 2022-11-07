using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : Singleton<ScoreManager>
{
    int m_currentScore = 0;
    int m_counterValue = 0;
    int m_increament = 5;

    public TextMeshProUGUI scoreText;
    public float countTime = 1f;

    private void Start()
    {
        UpdateScoreText(m_currentScore);
    }

    public void UpdateScoreText(int scoreValue)
    {
        if (scoreText != null)
        {
            scoreText.text = scoreValue.ToString();
        }
    }

    public void AddScore(int value)
    {
        m_currentScore += value;
        StartCoroutine(CountScoreRoutine());
    }

    IEnumerator CountScoreRoutine()
    {
        int iterations = 0;

        while (m_counterValue < m_currentScore && iterations < 100000)
        {
            m_counterValue += m_increament;
            UpdateScoreText(m_counterValue);
            iterations++;
            yield return new WaitForSeconds(0.15f);
        }

        m_counterValue = m_currentScore;
        UpdateScoreText(m_currentScore);
    }


}
