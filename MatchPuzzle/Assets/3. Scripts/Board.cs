using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Board : MonoBehaviour
{
    public int width;
    public int height;

    public int borderSize = 2;

    public GameObject tileNormalPrefabs;
    public GameObject tileObstaclePrefab;
    public GameObject[] gamePiecePerfabs;

    public GameObject adjacentBombPrefab;
    public GameObject rowBombPrefab;
    public GameObject columnBombPrefab;
    public GameObject colorBombPerfab;

    GameObject m_clickedTileBomb;
    GameObject m_targetTileBomb;

    public float swapTime = 0.5f;

    Tile[,] m_allTiles; //2���� �迭 ����
    GamePiece[,] m_allGamePiece;

    Tile m_clickedTile;
    Tile m_targetTile;

    bool m_playerInputEnabled = true;

    public StartingObject[] startingTiles;
    public StartingObject[] startingGamePiece;

    ParticleManager m_particleManager;

    [System.Serializable]
    public class StartingObject
    {
        public GameObject Perfab;
        public int x;
        public int y;
        public int z;
    }

    void Start()
    {
        m_allTiles = new Tile[width, height]; //������ �迭 �ȿ� ũ�� ����
        m_allGamePiece = new GamePiece[width, height]; //�迭 �ʱ�ȭ
        SetupTiles();
        SetUpGamePieces();
        SetupCamera();
        FilBorad(10, 0.5f);

        m_particleManager = FindObjectOfType<ParticleManager>();
    }

    void SetupTiles() //Ÿ�� �����ϴ� �Լ�
    {
        foreach (StartingObject sTile in startingTiles)
        {
            if (sTile != null)
            {
                MakeTile(sTile.Perfab, sTile.x, sTile.y, sTile.z);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (m_allTiles[i, j] == null)
                {
                    MakeTile(tileNormalPrefabs, i, j);
                }
            }
        }
    }

    void SetUpGamePieces()
    {
        foreach (StartingObject sPiece in startingGamePiece)
        {
            if (sPiece != null)
            {
                GameObject piece = Instantiate(sPiece.Perfab, new Vector3(sPiece.x, sPiece.y, 0), Quaternion.identity);
                MakeGamePiece(piece, sPiece.x, sPiece.y, 10, 0.1f);
            }
        }
    }

    private void MakeTile(GameObject prefab, int x, int y, int z = 0)
    {
        if (prefab != null)
        {
            GameObject tile = Instantiate(prefab, new Vector3(x, y, z), Quaternion.identity); //ȸ�� 0, ȸ�� ����
            tile.name = "Tile(" + x + "," + y + ")";
            m_allTiles[x, y] = tile.GetComponent<Tile>();
            tile.transform.parent = transform;
            m_allTiles[x, y].Init(x, y, this);
        }
    }

    void SetupCamera()
    { //�̰�
        Camera.main.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);
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

    void FilRandom()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                FillRandomAt(i, j);
            }
        }
    }

    void FilBorad(int falseYOffset = 0, float moveTime = 0.1f)
    {
        int maxInterations = 100;
        int iterations = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (m_allGamePiece[i, j] == null && m_allTiles[i, j].tileType != TileType.Obstacle)
                {
                    GamePiece Piece = FillRandomAt(i, j, falseYOffset, moveTime);
                    while (HasMatchOnFill(i, j))
                    {
                        ClearPieceAt(i, j);
                        Piece = FillRandomAt(i, j, falseYOffset, moveTime);
                        iterations++;
                        if (iterations >= maxInterations)
                        {
                            Debug.Log("break=====================================");
                            break;
                        }
                    }
                }
            }
        }
    }

    private bool HasMatchOnFill(int x, int y, int minLength = 3)
    {
        List<GamePiece> leftMatches = FindMatches(x, y, new Vector2(-1, 0), minLength);
        List<GamePiece> downMatches = FindMatches(x, y, new Vector2(0, -1), minLength);
        List<GamePiece> rightMatches = FindMatches(x, y, new Vector2(1, 0), minLength);
        List<GamePiece> upMatchse = FindMatches(x, y, new Vector2(0, 1), minLength);

        if (leftMatches == null) { leftMatches = new List<GamePiece>(); }
        if (downMatches == null) { downMatches = new List<GamePiece>(); }
        if (rightMatches == null) { rightMatches = new List<GamePiece>(); }
        if (upMatchse == null) { upMatchse = new List<GamePiece>(); }

        return (leftMatches.Count > 0 || downMatches.Count > 0 || rightMatches.Count > 0 || upMatchse.Count > 0);
    }

    private GamePiece FillRandomAt(int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (IsWithinBounds(x, y))
        {
            GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity);
            MakeGamePiece(randomPiece, x, y, falseYOffset, moveTime);
            return randomPiece.GetComponent<GamePiece>();
        }
        return null;
    }


    GameObject MakeBomb(GameObject prefab, int x, int y)
    {
        if (prefab != null && IsWithinBounds(x, y))
        {
            GameObject bomb = Instantiate(prefab, new Vector3(x, y, 0), Quaternion.identity);
            bomb.GetComponent<Bomb>().Init(this);
            bomb.GetComponent<Bomb>().SetCoord(x, y);
            bomb.transform.parent = transform;
            return bomb;
        }
        return null;
    }

    private void MakeGamePiece(GameObject perfab, int x, int y, int falseYOffset = 0, float moveTime = 0.1f)
    {
        if (perfab != null)
        {
            perfab.GetComponent<GamePiece>().Init(this);
            PlaceGamePiece(perfab.GetComponent<GamePiece>(), x, y);

            if (falseYOffset != 0)
            {
                perfab.transform.position = new Vector3(x, y + falseYOffset, 0);
                perfab.GetComponent<GamePiece>().Move(x, y, moveTime);
            }

            perfab.transform.parent = transform;
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
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
    }

    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        //if (m_playerInputEnabled)
        {
            //�� �� gamepiece�� ��ü
            GamePiece clickedPiece = m_allGamePiece[clickedTile.xIndex, clickedTile.yIndex];
            GamePiece targetPiece = m_allGamePiece[targetTile.xIndex, targetTile.yIndex];

            if (clickedPiece != null && targetPiece != null)
            {

                clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
                targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);


                yield return new WaitForSeconds(swapTime);

                List<GamePiece> clickedPieceMathes = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
                List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.xIndex, targetTile.yIndex);
                List<GamePiece> colorMatches = new List<GamePiece>();

                if (isColorBomb(clickedPiece) && !isColorBomb(targetPiece))
                {
                    clickedPiece.matchValue = targetPiece.matchValue;
                    colorMatches = FindAllMatchValue(clickedPiece.matchValue);
                }
                else if (!isColorBomb(clickedPiece) && isColorBomb(targetPiece))
                {
                    targetPiece.matchValue = clickedPiece.matchValue;
                    colorMatches = FindAllMatchValue(targetPiece.matchValue);
                }
                else if (isColorBomb(clickedPiece) && isColorBomb(targetPiece))
                {
                    foreach (GamePiece piece in m_allGamePiece)
                    {
                        if (!colorMatches.Contains(piece))
                        {
                            colorMatches.Add(piece);
                        }
                    }
                }

                if (clickedPieceMathes.Count == 0 && targetPieceMatches.Count == 0 && colorMatches.Count == 0)
                {
                    clickedPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
                    targetPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);

                    yield return new WaitForSeconds(swapTime);
                }
                else
                {
                    Vector2 swapDirection = new Vector2(targetTile.xIndex - clickedTile.xIndex, targetTile.yIndex - clickedTile.yIndex);
                    m_clickedTileBomb = DropBomb(clickedTile.xIndex, clickedTile.yIndex, swapDirection, clickedPieceMathes);
                    m_targetTileBomb = DropBomb(targetTile.xIndex, targetTile.yIndex, swapDirection, targetPieceMatches);

                    if (m_clickedTileBomb != null && targetPiece != null)
                    {
                        GamePiece clickedBombPiece = m_clickedTileBomb.GetComponent<GamePiece>();
                        if (!isColorBomb(clickedBombPiece))
                        {
                            clickedBombPiece.ChangeColor(targetPiece);
                        }
                    }

                    if (m_targetTileBomb != null && clickedPiece != null)
                    {
                        GamePiece targetBombPiece = m_targetTileBomb.GetComponent<GamePiece>();
                        if (!isColorBomb(targetBombPiece))
                        {
                            targetBombPiece.ChangeColor(clickedPiece);
                        }
                    }

                    ClearAndRefillBoard(clickedPieceMathes.Union(targetPieceMatches).ToList().Union(colorMatches).ToList());
                }

            }
        }
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

            if (nextPiece == null)
            {
                break;
            }
            else
            {
                if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))       //���� ���Ƶα�
                {
                    matches.Add(nextPiece);
                }
                else
                {
                    break;
                }
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

        foreach (GamePiece piece in downwardMatches)
        {
            if (!upwardMatches.Contains(piece))
            {
                upwardMatches.Add(piece);
            }
        }

        return (upwardMatches.Count >= minLength) ? upwardMatches : null;

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

    List<GamePiece> FindAllMatches()
    {
        List<GamePiece> combinedMatches = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                List<GamePiece> matchs = FindMatchesAt(i, j);
                combinedMatches = combinedMatches.Union(matchs).ToList();
            }
        }
        return combinedMatches;
    }


    void HighlightTileOff(int x, int y)
    {
        if (m_allTiles[x, y].tileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        }
    }

    void HighlightTileOn(int x, int y, Color color)
    {
        if (m_allTiles[x, y].tileType != TileType.Breakable)
        {
            SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
            spriteRenderer.color = color;
        }
    }

    void HighlightMatchse()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                HightlightMatchesAt(i, j);
            }
        }
    }

    void HightlightMatchesAt(int i, int j)
    {
        HighlightTileOff(i, j);
        List<GamePiece> combineMatches = FindMatchesAt(i, j);

        if (combineMatches.Count > 0)
        {
            foreach (GamePiece piece in combineMatches)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    void HighlightPieces(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                HighlightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }


    private List<GamePiece> FindMatchesAt(int i, int j, int minLength = 3)
    {
        List<GamePiece> horizMatches = FindHorizontalMatches(i, j, minLength);
        List<GamePiece> vertMatches = FindVerticalMatches(i, j, minLength);

        if (horizMatches == null)
        {
            horizMatches = new List<GamePiece>();
        }

        if (vertMatches == null)
        {
            vertMatches = new List<GamePiece>();
        }

        var combineMatches = horizMatches.Union(vertMatches).ToList();
        return combineMatches;
    }

    private List<GamePiece> FindMatchesAt(List<GamePiece> gamePieces, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();

        foreach (GamePiece piece in gamePieces)
        {
            matches = matches.Union(FindMatchesAt(piece.xIndex, piece.yIndex, minLength)).ToList();
        }

        return matches;
    }


    void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = m_allGamePiece[x, y];

        if (pieceToClear != null)
        {
            m_allGamePiece[x, y] = null;
            Destroy(pieceToClear.gameObject);
        }

        //HighlightTileOff(x, y);
    }

    void ClearPieceAt(List<GamePiece> gamePieces, List<GamePiece> bombedPieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                ClearPieceAt(piece.xIndex, piece.yIndex);

                if (m_particleManager != null)
                {
                    if (bombedPieces.Contains(piece))
                    {
                        m_particleManager.bombFXAt(piece.xIndex, piece.yIndex);
                    }
                    m_particleManager.ClearPieceFXAt(piece.xIndex, piece.yIndex);
                }
            }

        }
    }
    
    private void BreakTileAt(int x, int y)
    {
        Tile tileToBreak = m_allTiles[x, y];

        if (tileToBreak != null && tileToBreak.tileType == TileType.Breakable)
        {
            if (m_particleManager != null)
            {
                m_particleManager.BreakTileFXAt(tileToBreak.breakableValue, x, y);
            }

            tileToBreak.BreakTile();
        }
    }

    void BreakTileAt(List<GamePiece> gamePIeces)
    {
        foreach (GamePiece piece in gamePIeces)
        {
            if (piece != null)
            {
                BreakTileAt(piece.xIndex, piece.yIndex);
            }
        }
    }

    void ClearBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPieceAt(i, j);
            }
        }
    }


    List<GamePiece> CollapseColumn(int column, float collapseTime = 0.1f)
    {
        List<GamePiece> movingPiece = new List<GamePiece>();

        for (int i = 0; i < height; i++)
        {
            if (m_allGamePiece[column, i] == null && m_allTiles[column, i].tileType != TileType.Obstacle)   // Ÿ�� Ÿ���� ����� ���� �̵��ϰ�
            {
                for (int j = i + 1; j < height; j++)    // j�� ���� ���������� �ǽ���
                {
                    if (m_allGamePiece[column, j] != null)
                    {
                        m_allGamePiece[column, j].Move(column, i, collapseTime * (j - i));
                        m_allGamePiece[column, i] = m_allGamePiece[column, j];
                        m_allGamePiece[column, i].SetCoord(column, i);

                        if (!movingPiece.Contains(m_allGamePiece[column, i]))
                        {
                            movingPiece.Add(m_allGamePiece[column, i]);
                        }

                        m_allGamePiece[column, j] = null;
                        break;
                    }
                }
            }
        }

        return movingPiece;
    }

    List<GamePiece> CollapseColumn(List<GamePiece> gamePiece)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<int> columnsToCollapse = GetColumns(gamePiece);

        foreach (int column in columnsToCollapse)
        {
            movingPieces = movingPieces.Union(CollapseColumn(column)).ToList();
        }

        return movingPieces;
    }

    List<int> GetColumns(List<GamePiece> gamePiece)
    {
        List<int> columns = new List<int>();

        foreach (GamePiece piece in gamePiece)
        {
            if (!columns.Contains(piece.xIndex))
            {
                columns.Add(piece.xIndex);
            }
        }

        return columns;
    }


    void ClearAndRefillBoard(List<GamePiece> gamePieces)
    {
        StartCoroutine(ClearAndRefillBoardRountine(gamePieces));
    }

    IEnumerator ClearAndRefillBoardRountine(List<GamePiece> gamePieces)
    {
        m_playerInputEnabled = false;
        List<GamePiece> matches = gamePieces;

        do
        {
            //clear and collapse
            yield return StartCoroutine(ClearAndCollapseRountine(matches));
            yield return null;

            //refill
            yield return StartCoroutine(RefillRoutine());
            matches = FindAllMatches();

            yield return new WaitForSeconds(0.5f);

        } while (matches.Count != 0);

        m_playerInputEnabled = true;
    }

    IEnumerator ClearAndCollapseRountine(List<GamePiece> gamePieces)
    {
        List<GamePiece> movingPieces = new List<GamePiece>();
        List<GamePiece> matchse = new List<GamePiece>();

        //HighlightPieces(gamePieces);
        yield return new WaitForSeconds(0.2f);
        bool isFinished = false;

        while (!isFinished)
        {
            //bomb piece�̸� gamePieces �߰�
            List<GamePiece> bombedPieces = GetBombedPieces(gamePieces);
            gamePieces = gamePieces.Union(bombedPieces).ToList();

            bombedPieces = GetBombedPieces(gamePieces);
            gamePieces = gamePieces.Union(bombedPieces).ToList();
            
            ClearPieceAt(gamePieces, bombedPieces);
            BreakTileAt(gamePieces);

            if (m_clickedTileBomb != null)
            {
                ActiveBomb(m_clickedTileBomb);
                m_clickedTileBomb = null;
            }

            if (m_targetTileBomb != null)
            {
                ActiveBomb(m_targetTileBomb);
                m_targetTileBomb = null;
            }

            yield return new WaitForSeconds(0.25f);

            movingPieces = CollapseColumn(gamePieces);

            while (!IsCollapsed(movingPieces))
            {
                yield return null;
            }

            yield return new WaitForSeconds(0.2f);

            matchse = FindMatchesAt(movingPieces);

            if (matchse.Count == 0)
            {
                isFinished = true;
                break;
            }
            else
            {
                yield return StartCoroutine(ClearAndCollapseRountine(matchse));  //�����Լ�
                isFinished = true;
            }
        }

        yield return null;
    }

    IEnumerator RefillRoutine()
    {
        FilBorad(10, 0.5f);
        yield return null;
    }

    bool IsCollapsed(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (piece.transform.position.y - (float)piece.yIndex > 0.01f)
                {
                    return false;
                }
            }
        }
        return true;
    }


    List<GamePiece> GetRowPieces(int row)  //rowbomb clear
    {
        List<GamePiece> gamePIeces = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            if (m_allGamePiece[i, row] != null)
            {
                gamePIeces.Add(m_allGamePiece[i, row]);
            }
        }

        return gamePIeces;
    }

    List<GamePiece> GetColumnPieces(int column)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = 0; i < height; i++)
        {
            if (m_allGamePiece[column, i] != null)
            {
                gamePieces.Add(m_allGamePiece[column, i]);
            }
        }

        return gamePieces;
    }

    List<GamePiece> GetAbjacentPieces(int x, int y, int offset = 1)
    {
        List<GamePiece> gamePieces = new List<GamePiece>();

        for (int i = x - offset; i <= x + offset; i++)
        {
            for (int j = y - offset; j <= y + offset; j++)
            {
                if (IsWithinBounds(i, j))
                {
                    gamePieces.Add(m_allGamePiece[i, j]);
                }
            }
        }

        return gamePieces;
    }

    List<GamePiece> GetBombedPieces(List<GamePiece> gamePieces)
    {
        List<GamePiece> allPiecedToClear = new List<GamePiece>();

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                List<GamePiece> piecesToClear = new List<GamePiece>();

                Bomb bomb = piece.GetComponent<Bomb>();

                if (bomb != null)
                {
                    switch (bomb.bombType)
                    {
                        case BombType.Column:
                            piecesToClear = GetColumnPieces(bomb.xIndex);
                            break;
                        case BombType.Row:
                            piecesToClear = GetRowPieces(bomb.yIndex);
                            break;
                        case BombType.Adjacent:
                            piecesToClear = GetAbjacentPieces(bomb.xIndex, bomb.yIndex);
                            break;
                        case BombType.Color:
                            break;
                    }
                    allPiecedToClear = allPiecedToClear.Union(piecesToClear).ToList();
                }
            }
        }
        return allPiecedToClear;
    }


    bool IsCornerMatch(List<GamePiece> gamePieces)
    {
        bool vertical = false;
        bool horizontal = false;
        int xStart = -1;
        int yStart = -1;

        foreach (GamePiece piece in gamePieces)
        {
            if (piece != null)
            {
                if (xStart == -1 || yStart == -1)
                {
                    xStart = piece.xIndex;
                    yStart = piece.yIndex;
                    continue;
                }

                if (piece.xIndex != xStart && piece.yIndex == yStart)
                {
                    horizontal = true;
                }

                if (piece.xIndex == xStart && piece.yIndex != yStart)
                {
                    vertical = true;
                }
            }
        }

        return (horizontal && vertical);
    }

    GameObject DropBomb(int x, int y, Vector2 swapDirection, List<GamePiece> gamePieces)
    {
        GameObject bomb = null;

        if (gamePieces.Count >= 4)
        {
            if (IsCornerMatch(gamePieces))
            {
                //adjacentBomb
                if (adjacentBombPrefab != null)
                {
                    bomb = MakeBomb(adjacentBombPrefab, x, y);
                }
            }
            else
            {
                if (gamePieces.Count >= 5)
                {
                    if (colorBombPerfab != null)
                    {
                        bomb = MakeBomb(colorBombPerfab, x, y);
                    }
                }

                Debug.Log(swapDirection);
                //rowbomb
                if (swapDirection.x != 0)
                {
                    if (rowBombPrefab != null)
                    {
                        bomb = MakeBomb(rowBombPrefab, x, y);
                    }
                }
                else
                {
                    if (columnBombPrefab != null)
                    {
                        bomb = MakeBomb(columnBombPrefab, x, y);
                    }
                }
                //columBomb
            }            
        }
        return bomb;
    }

    void ActiveBomb(GameObject bomb)
    {
        int x = (int)bomb.transform.position.x;
        int y = (int)bomb.transform.position.y;

        if (IsWithinBounds(x, y))
        {
            m_allGamePiece[x, y] = bomb.GetComponent<GamePiece>();
        }
    }


    List<GamePiece> FindAllMatchValue(MatchValue mValue)
    {
        List<GamePiece> foundPieces = new List<GamePiece>();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (m_allGamePiece[i, j] != null)
                {
                    if (m_allGamePiece[i, j].matchValue == mValue)
                    {
                        foundPieces.Add(m_allGamePiece[i, j]);
                    }
                }
            }
        }

        return foundPieces;
    }

    bool isColorBomb(GamePiece gamePiece)
    {
        Bomb bomb = gamePiece.GetComponent<Bomb>();
        if (bomb != null)
        {
            return (bomb.bombType == BombType.Color);
        }
        return false;
    }







}
