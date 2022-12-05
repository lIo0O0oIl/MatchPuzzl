using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardShuffler : MonoBehaviour
{
    public List<GamePiece> RemoveNormalPieces(GamePiece[,] allGamePieces)   //bomb, collectable을 제외한 normalpieces 모아서 리턴
    {
        List<GamePiece> normalPieces = new List<GamePiece>();

        int width = allGamePieces.GetLength(0);
        int height = allGamePieces.GetLength(1);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGamePieces[i, j] != null)
                {
                    Bomb bomb = allGamePieces[i, j].GetComponent<Bomb>();
                    Collectable collectable = allGamePieces[i, j].GetComponent<Collectable>();

                    if (bomb == null && collectable == null)
                    {
                        normalPieces.Add(allGamePieces[i, j]);
                        allGamePieces[i, j] = null;
                    }
                }
            }
        }
        return normalPieces;
    }

    public void ShuffleList(List<GamePiece> piecesToShuffle)
    {
        int maxCount = piecesToShuffle.Count;

        for (int i = 0; i < maxCount - 1; i++)
        {
            int r = Random.Range(i, maxCount);
            if (i == r) continue;

            GamePiece tmp = piecesToShuffle[r];
            piecesToShuffle[r] = piecesToShuffle[i];
            piecesToShuffle[i] = tmp;
        }
    }

    public void MovePieces(GamePiece[,] allGamePieces, float swapTime = 0.5f)
    {
        int width = allGamePieces.GetLength(0);
        int height = allGamePieces.GetLength(1);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allGamePieces[i, j] != null)
                {
                    allGamePieces[i, j].Move(i, j, swapTime);
                }
            }
        }
    }
}
