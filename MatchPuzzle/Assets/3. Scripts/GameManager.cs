using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public int movesLeft = 30;
    public int scoreGoal = 10000;

    public ScreenFader screenFader;
    public TextMeshProUGUI levelNameText;
    public TextMeshProUGUI movesLeftText;

    bool m_isReadyToBegin = false;
    bool m_isGameOver = false;
    bool m_isWinner = false;
    bool m_isReadToToload = false;

    Board m_board;

    public MesageWindow messageWindow;

    public Sprite loseIcon;
    public Sprite winIcon;
    public Sprite goalIcon;

    private void Start()
    {
        m_board = FindObjectOfType<Board>();

        Scene scene = SceneManager.GetActiveScene();

        if (levelNameText != null)
        {
            levelNameText.text = scene.name;
        }

        UpdateMoves();

        StartCoroutine(ExecuteGameLoop());
    }

    public void UpdateMoves()
    {
        if (movesLeftText != null) movesLeftText.text = movesLeft.ToString();
    }

    IEnumerator ExecuteGameLoop()
    {
        yield return StartCoroutine("StartGameRoutine");
        yield return StartCoroutine("PlayGameRoutine");
        yield return StartCoroutine("EndGameRoutine");
    }

    public void BeginGame()
    {
        m_isReadyToBegin = true;
    }

    IEnumerator StartGameRoutine()
    {
        if (messageWindow != null)
        {
            messageWindow.GetComponent<RectXformMover>().MoveOn();
            messageWindow.ShowMessage(goalIcon, "Score goal\n" + scoreGoal.ToString(), "Start");
        }

        while (!m_isReadyToBegin)
        {
            yield return null;
            //yield return new WaitForSeconds(2f);
            //m_isReadyToBegin = true;
        }

        screenFader?.FadeOff();

        yield return new WaitForSeconds(0.5f);

        if (m_board != null) m_board?.SetUpBorad();
    }

    IEnumerator PlayGameRoutine()
    {
        while (!m_isGameOver)
        {
            if (movesLeft == 0)
            {
                m_isGameOver = true;
                m_isWinner = false;
            }

            yield return null;
        }
    }

    IEnumerator EndGameRoutine()
    {
        m_isReadToToload = false;

        if (m_isWinner)
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(winIcon, "You win!!", "ok");
            }
        }
        else
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(loseIcon, "You lose...", "ok");
            }
        }

        if (screenFader != null) screenFader.FadeOn();

        while (!m_isReadToToload)
        {
            yield return null;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ReloadScene()
    {
        m_isReadToToload = true;
    }
}
