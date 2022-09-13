using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public int borderSize = 2;
    public GameObject tilePrefabs;
    public GameObject[] gamePiecePerfabs;

    public float swapTime = 0.5f;

    Tile[,] m_allTiles; //2���� �迭 ����
    GamePiece[,] m_allGamePiece;

    Tile m_clickedTile;
    Tile m_targetTile;

    void Start()
    {
        m_allTiles = new Tile[width, height]; //������ �迭 �ȿ� ũ�� ����
        m_allGamePiece = new GamePiece[width, height]; //�迭 �ʱ�ȭ
        SetupTiles();
        SetupCamera();
        FillRandom();
        HighlightMatchse();
    }

    void SetupTiles() //Ÿ�� �����ϴ� �Լ�
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate(tilePrefabs, new Vector3(i, j, 0), Quaternion.identity); //ȸ�� 0, ȸ�� ����
                tile.name = "Tile(" + i + "," + j + ")";
                m_allTiles[i,j] = tile.GetComponent<Tile>();
                tile.transform.parent = transform;

                m_allTiles[i,j].Init(i, j, this);
            }
        }
    }

    void SetupCamera()
    { //�̰�
        Camera.main.transform.position = new Vector3((float)(width-1) / 2f, (float)(height-1) / 2f, -10f);
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        float verticalSize = (float)height / 2f + (float)borderSize;
        float horizontalSize = (float)width / 2f + (float)borderSize / aspectRatio;
        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;

    }

    GameObject GetRandomGamePiece()
    {
        int randomIndex = Random.Range(0, gamePiecePerfabs.Length);

        if (gamePiecePerfabs[randomIndex] == null)
        {
            Debug.Log("�ε������� ���� ��");
        }

        return gamePiecePerfabs[randomIndex];
    }

    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    { // �̰�
        if (gamePiece == null)
        {
            Debug.LogWarning("Board : Invalid GamePicec!");
            return;
        }
        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;

        if (IsWithinBounds(x, y))
        {
            m_allGamePiece[x, y] = gamePiece;
        }

        gamePiece.SetCoord(x, y);
    }

    bool IsWithinBounds(int x, int y)
    {
        return (x >= 0 && y >= 0 && x < width && y < height);
    }

    void FillRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity);

                if (randomPiece != null)
                {
                    randomPiece.GetComponent<GamePiece>().Init(this);
                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j  );
                    randomPiece.transform.parent = transform;
                }


            }
        }
    }


    public void ClickTile(Tile tile)
    {
        if (m_clickedTile == null)
        {
            m_clickedTile = tile;
            //Debug.Log("clicked tile" + tile.name);
        }
    }

    public void DragToTile(Tile tile)
    {
        if (m_clickedTile != null && InNextTo(m_clickedTile, tile))
        {
            m_targetTile = tile;
            //Debug.Log("target tile" + tile.name);
        }
    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTiles(m_clickedTile, m_targetTile);
        }

        m_clickedTile = null;
        m_targetTile = null;
    }

    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        //�� �� gamepiece�� ��ü
        GamePiece clickedPiece = m_allGamePiece[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPiece = m_allGamePiece[targetTile.xIndex, targetTile.yIndex];

        clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
        targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);

    }


    bool InNextTo(Tile start, Tile end)
    {
        if (start.xIndex == end.xIndex && Mathf.Abs(end.yIndex - start.yIndex) == 1) return true;
        if (Mathf.Abs(end.xIndex - start.xIndex) == 1 && start.yIndex == end.yIndex) return true;       //���ͷ� �Ÿ�����ؼ� ��Ÿ�� ���� ����.
        return false;
    }

    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLenth = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();        //��ŸƮ �ǽ��� ���ϱ�

        GamePiece startPiece = null;

        if (IsWithinBounds(startX, startY))     //���� ���� �ȿ� �ִ���
        {
            startPiece = m_allGamePiece[startX, startY];
        }

        if (startPiece != null)     //���� �ִ��� ������ Ȯ��
        {
            matches.Add(startPiece);
        }
        else
        {
            return null;
        }

        int nextX;
        int nextY;

        int maxValue = (width > height) ? width : height;

        for (int i = 1; i < maxValue - 1; i++)
        {
            nextX = startX + (int)Mathf.Clamp(searchDirection.x, -1, 1) * i;        //-1, 0, 1 ���·θ� �޴°�
            nextY = startY + (int)Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))       //�迭�ȿ� �ִ���
            {
                break;
            }

            GamePiece nextPiece = m_allGamePiece[nextX, nextY];

            if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))       //���� ���Ƶα�
            {
                matches.Add(nextPiece);
            }
            else
            {
                break;
            }

        }

        if (matches.Count >= minLenth)      //�� �� �̻��� �� ��������.
        {
            return matches;
        }

        return null;

    }

    List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -1), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }
        if (downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        //var combineMatches = upwardMatches.Union(downwardMatches).ToList();

        foreach(GamePiece piece in downwardMatches)
        {
            if (!upwardMatches.Contains(piece))
            {
                upwardMatches.Add(piece);
            }
        }

        return (upwardMatches.Count >= minLength)? upwardMatches : null;

    }

    List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightwardMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftwardMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightwardMatches == null)
        {
            rightwardMatches = new List<GamePiece>();
        }
        if (leftwardMatches == null)
        {
            leftwardMatches = new List<GamePiece>();
        }

        //var combineMatches = upwardMatches.Union(downwardMatches).ToList();

        foreach (GamePiece piece in leftwardMatches)
        {
            if (!rightwardMatches.Contains(piece))
            {
                rightwardMatches.Add(piece);
            }
        }

        return (rightwardMatches.Count >= minLength) ? rightwardMatches : null;

    }

    void HighlightMatchse()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                SpriteRenderer spriteRenderer = m_allTiles[i, j].GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

                List<GamePiece> horizMatches = FindHorizontalMatches(i, j, 3);
                List<GamePiece> vertMatches = FindVerticalMatches(i, j, 3);

                if (horizMatches == null)
                {
                    horizMatches = new List<GamePiece>();
                }

                if (vertMatches == null)
                {
                    vertMatches = new List<GamePiece>();
                }
                var combineMatches = horizMatches.Union(vertMatches).ToList();
                Debug.Log(combineMatches.Count);

                if (combineMatches.Count > 0)
                {
                    Debug.Log("123");
                    foreach (GamePiece piece in combineMatches)
                    {
                        spriteRenderer = m_allTiles[piece.xIndex, piece.yIndex].GetComponent<SpriteRenderer>();
                        spriteRenderer.color = piece.GetComponent<SpriteRenderer>().color;
                    }
                }
            }
        }
    }





}
