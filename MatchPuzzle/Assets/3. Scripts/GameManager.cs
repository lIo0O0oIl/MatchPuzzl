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

    public bool IsGameOver { get => m_isGameOver; set => m_isGameOver = value; }

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

        //마지막 교환하고 다시 보드 채울 때까지 기다리기
        yield return StartCoroutine("WaitForBoardRoutine", 1f);

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
            if (ScoreManager.instance != null)
            {
                if (ScoreManager.instance.CurrentScore >= scoreGoal)
                {
                    m_isGameOver = true;
                    m_isWinner = true;
                }
            }

            if (movesLeft == 0)
            {
                m_isGameOver = true;
                m_isWinner = false;
            }

            yield return null;
        }
    }

    IEnumerator WaitForBoardRoutine(float delay = 0f)
    {
        if (m_board != null)
        {
            while (!m_board.isRefilling)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(delay);
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

            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayWinSound();
            }
        }
        else
        {
            if (messageWindow != null)
            {
                messageWindow.GetComponent<RectXformMover>().MoveOn();
                messageWindow.ShowMessage(loseIcon, "You lose..", "ok");
            }

            if (SoundManager.instance != null)
            {
                SoundManager.instance.PlayloseSound();
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
