using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BoardDeadlook : MonoBehaviour
{
    List<GamePiece> GetRowOrColumnList(GamePiece[,] allPieces, int x, int y, int listLength = 3, bool checkRow = true)      //�ο�� Į���� �����ִ� ��.
    {
        int width = allPieces.GetLength(0);  //������ �迭, ù��° ��
        int height = allPieces.GetLength(1);    //2���� �迭 �ι�° ��

        List<GamePiece> piecesList = new List<GamePiece>(); //3���� ����

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

        var groups = gamePieces.GroupBy(n => n.matchValue);     //���� ��ġ���� ����...?

        foreach(var grp in groups)
        {
            if (grp.Count() >= minForMatch && grp.Key != MatchValue.None)   //key�� ��������....?
            {
                matches = grp.ToList();
            } 
        }

        return matches;
    }

    List<GamePiece> GetNeighbors(GamePiece[,] allPieces, int x, int y)      //3���� ���� �� ����� �� �Ʒ��� �������� ����Ʈ
    {
        int width = allPieces.GetLength(0);  //������ �迭, ù��° ��
        int height = allPieces.GetLength(1);    //2���� �迭 �ι�° ��

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
            if (x + (int)dir.x >= 0 && x + (int)dir.y < width && y + (int)dir.y >= 0 && y + (int)dir.y < height)    //���̶� ��¼�� ����ǰ�
            {
                if (allPieces[x + (int)dir.x, y + (int)dir.y] != null)
                {
                    if (!neighbors.Contains(allPieces[x + (int)dir.x, y + (int)dir.y]))     //���ԵǾ� ���� ���� ����
                    {
                        neighbors.Add(allPieces[x + (int)dir.x, y + (int)dir.y]);
                    }
                }
            }
        }

        return neighbors;
    }


}
