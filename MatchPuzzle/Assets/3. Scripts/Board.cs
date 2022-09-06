using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public int borderSize = 2;
    public GameObject tilePrefabs;
    public GameObject[] gamePiecePerfabs;

    Tile[,] m_allTiles; //2���� �迭 ����
    GamePiece[,] m_allGamePiecePrefabs;

    Tile m_clickedTile;
    Tile m_targetTile;

    void Start()
    {
        m_allTiles = new Tile[width, height]; //������ �迭 �ȿ� ũ�� ����
        m_allGamePiecePrefabs = new GamePiece[width, height]; //�迭 �ʱ�ȭ
        SetupTiles();
        SetupCamera();
        FillRandom();
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

    void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    { // �̰�
        if (gamePiece == null)
        {
            Debug.LogWarning("Board : Invalid GamePicec!");
            return;
        }
        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;
        gamePiece.SetCoord(x, y);
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
                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j  );
                }


            }
        }
    }


    public void ClickTile(Tile tile)
    {
        if (m_clickedTile == null)
        {
            m_clickedTile = tile;
            Debug.Log("clicked tile" + tile.name);
        }
    }

    public void DragToTile(Tile tile)
    {
        if (m_clickedTile != null)
        {
            m_targetTile = tile;
            Debug.Log("target tile" + tile.name);
        }
    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTiles(m_clickedTile, m_targetTile);
        }
    }

    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        //�� �� gamepiece�� ��ü

        m_clickedTile = null;
        m_targetTile = null;
    }



}
