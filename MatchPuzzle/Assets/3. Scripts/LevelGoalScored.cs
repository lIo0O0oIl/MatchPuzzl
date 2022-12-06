using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGoalScored : LevelGoal
{
    public override bool IsGameOver()
    {
        return movesLeft == 0;
    }

    public override bool IsWinner()
    {
        if (ScoreManager.instance != null)
        {
            return ScoreManager.instance.CurrentScore >= scoreGoals[0];
        }
        return false;
    }
}
