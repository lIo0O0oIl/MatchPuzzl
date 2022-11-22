using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardDeadlook : MonoBehaviour
{
    List<GamePiece> GetRowOrColumnList(GamePiece[,] allPieces, int x, int y, int listLength = 3, bool checkRow = true)      //로우랑 칼럼을 묶어주는 것.
    {
        int width = allPieces.GetLength(0);  //일차원 배열, 첫번째 열
        int height = allPieces.GetLength(1);    //2차원 배열 두번째 열

        List<GamePiece> piecesList = new List<GamePiece>(); //3개씩 묶기

        for (int i = 0; i < listLength; i++)
        {
            if (checkRow)
            {
                if (x + i < width && y < height)
                {
                    piecesList.Add(allPieces[x + i, y]);
                }
            }
            else
            {
                if (x < width && y < height)
                {
                    piecesList.Add(allPieces[x, y + i]);
                }
            }
        }

        return piecesList;
    }

    List<GamePiece> GetMinimumMatches(List<GamePiece> gamePieces, int minForMatch = 2)
    {
        List<GamePiece> matches = new List<GamePiece>();

        var groups = gamePieces.GroupBy(n => n.matchValue);     //같은 매치끼리 묵음...?

        foreach(var grp in groups)
        {
            if (grp.Count() >= minForMatch && grp.Key != MatchValue.None)   //key가 묶어진거....?
            {
                matches = grp.ToList();
            } 
        }

        return matches;
    }

    List<GamePiece> GetNeighbors(GamePiece[,] allPieces, int x, int y)      //3개씩 묶고 그 가운데의 위 아래를 가져오는 리스트
    {
        int width = allPieces.GetLength(0);  //일차원 배열, 첫번째 열
        int height = allPieces.GetLength(1);    //2차원 배열 두번째 열

        List<GamePiece> neighbors = new List<GamePiece>();

        Vector2[] searchDirections = new Vector2[4]
        {
            new Vector2(-1f, 0f),
            new Vector2(1f, 0f),
            new Vector2(0f, -1f),
            new Vector2(0f, 1f),
        };

        foreach (Vector2 dir in searchDirections)
        {
            if (x + (int)dir.x >= 0 && x + (int)dir.y < width && y + (int)dir.y >= 0 && y + (int)dir.y < height)    //높이랑 어쩌고 제약건것
            {
                if (allPieces[x + (int)dir.x, y + (int)dir.y] != null)
                {
                    if (!neighbors.Contains(allPieces[x + (int)dir.x, y + (int)dir.y]))     //포함되어 있지 않을 때만
                    {
                        neighbors.Add(allPieces[x + (int)dir.x, y + (int)dir.y]);
                    }
                }
            }
        }

        return neighbors;
    }


}
