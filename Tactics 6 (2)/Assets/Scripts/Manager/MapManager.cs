using System.Collections.Generic;
using UnityEngine;


public class MapManager : MonoBehaviour {

    public GameObject gOnTileObj;

    //길찾기
    public List<GameObject> TileList = new List<GameObject>();
    public List<Tiles.Tiledir> BestList = new List<Tiles.Tiledir>();
    private List<int> OpenList = new List<int>();
    private List<int> CloseList = new List<int>();

    private Tiles m_tStart = GameData.tStartTile;
    private Tiles m_tFinal = GameData.tFinalTile;
    private GameObject m_gPlayer;
    private PlayerAct m_pPlayerMove;

    private bool m_bIsGoal;

    //이동범위
    private List<int> m_MoveOpen = new List<int>();
    private List<int> m_MoveClose = new List<int>();

    private int m_iMaxMoveRange;
    private int m_iPlayerJump;
    private int m_iNowZ;
    private bool m_bChInit;
    private bool m_bIsFindRange;


    void Start()
    {
        m_bChInit = true;

        MakeMap();
    }

    void Update()
    {
        GetDirections();
        SetMoveRange();
    }

    //맵을 만듦
    void MakeMap()
    {
        for (int y = 0, yCnt = GameData.MaxTile[GameData.iNowMap, 1]; y < yCnt; ++y)
        {
            for (int x = 0, xCnt = GameData.MaxTile[GameData.iNowMap, 0]; x < xCnt; ++x)
            {
                GameObject temp = Instantiate(gOnTileObj);
                TileList.Add(temp);
                temp.transform.SetParent(this.transform);
                        
                temp.GetComponent<Tiles>().X = x;
                temp.GetComponent<Tiles>().Y = y;
                temp.GetComponent<Tiles>().Z = GameData.Map[GameData.iNowMap, y, x];
            }
        }
    }

    //이동범위를 보여줌
    void SetMoveRange()
    {
        if (GameData.iNowState == 4)
        {
            if (!m_bChInit)
                return;

            //초기화
            m_gPlayer = GameData.gNowPlayer;
            m_pPlayerMove = m_gPlayer.GetComponent<PlayerAct>();
            m_iMaxMoveRange = m_gPlayer.GetComponent<Status>().iMovingRange;
            m_iPlayerJump = GameData.gNowPlayer.GetComponent<Status>().iJump;
            m_MoveOpen.Clear();
            m_MoveClose.Clear();
            GameData.tStartTile.X = m_pPlayerMove.X;
            GameData.tStartTile.Y = m_pPlayerMove.Y;
            GameData.tStartTile.Z = m_pPlayerMove.Z;
            m_tStart = GameData.tStartTile;
            m_iNowZ = m_pPlayerMove.Z;
            m_MoveOpen.Clear();
            int idx = Getidx(m_tStart);
            m_MoveOpen.Add(idx);
            m_bIsFindRange = true;
            for (int i = TileList.Count; i > 0; --i)
            {
                TileList[i - 1].GetComponent<Tiles>().bIsOpen = false;
                TileList[i - 1].GetComponent<Tiles>().bIsClose = false;
                TileList[i - 1].GetComponent<Tiles>().g = 0;
                TileList[i - 1].GetComponent<Tiles>().gIsOnPlayer = null;
            }

            while (true)
            {
                if (m_MoveOpen.Count == 0)
                    break;
                int min = int.MaxValue;
                for (int i = 0; i < m_MoveOpen.Count; i++)
                {
                    Tiles tTiles = TileList[m_MoveOpen[i]].GetComponent<Tiles>();
                    if (min > tTiles.g)
                    {
                        min = TileList[m_MoveOpen[i]].GetComponent<Tiles>().g;
                        idx = i;
                    }
                }

                idx = m_MoveOpen[idx];

                int Left = FindgTile(Tiles.Tiledir.left, idx);
                int Right = FindgTile(Tiles.Tiledir.right, idx);
                int Up = FindgTile(Tiles.Tiledir.up, idx);
                int Down = FindgTile(Tiles.Tiledir.down, idx);

                TileInit(idx, Left, Tiles.Tiledir.left);
                TileInit(idx, Right,Tiles.Tiledir.right);
                TileInit(idx, Up,   Tiles.Tiledir.up);
                TileInit(idx, Down, Tiles.Tiledir.down);

                Addm_MoveClose(idx);
            }

            m_bIsFindRange = false;
            m_bChInit = false;
            m_bIsFindRange = false;
        }

        else
        {
            for (int i = 0; i < TileList.Count; i++)
            {
                TileList[i].GetComponent<Tiles>().SetMoveRange(false);
            }
            m_bChInit = true;
        }
    }

    //길찾기 과정
    void GetDirections()
    {
        if (GameData.bIsClick == true)   //타일 입력이 확인되면 길찾기 시작
        {
            for (int i = TileList.Count; i > 0; --i)
            {
                TileList[i - 1].GetComponent<Tiles>().bIsOpen = false;
                TileList[i - 1].GetComponent<Tiles>().bIsClose = false;
                TileList[i - 1].GetComponent<Tiles>().g = 0;
                TileList[i - 1].GetComponent<Tiles>().f = 0;
                TileList[i - 1].GetComponent<Tiles>().h = 0;
            }

            m_pPlayerMove = GameData.gNowPlayer.GetComponent<PlayerAct>();

            GameData.tStartTile.X = m_pPlayerMove.X;
            GameData.tStartTile.Y = m_pPlayerMove.Y;
            GameData.tStartTile.Z = m_pPlayerMove.Z;

            //초기화
            m_bIsGoal = false;
            BestList.Clear();
            OpenList.Clear();
            CloseList.Clear();
            m_tFinal = GameData.tFinalTile;
            int startIdx = Getidx(m_tStart);
            m_bIsFindRange = false;

            AddOpenList(startIdx);
            Loop();
            Result();
        }
    }

    void Loop()
    {   
        int idx = -1;
        while (true)
        {
            //f값이 가장 작은 타일을 찾음
            int min = int.MaxValue;
            for (int i = 0; i < OpenList.Count; i++)
            {
                Tiles tTiles = TileList[OpenList[i]].GetComponent<Tiles>();
                if (min > tTiles.f)
                {
                    min = TileList[OpenList[i]].GetComponent<Tiles>().f;
                    idx = i;
                }
            }

            idx = OpenList[idx];
            Tiles tInSearch = TileList[idx].GetComponent<Tiles>();

            //주변타일을 탐색, 오픈리스트에 넣음
            int Left    = FindgTile(Tiles.Tiledir.left, idx);
            int Right   = FindgTile(Tiles.Tiledir.right, idx);
            int Up      = FindgTile(Tiles.Tiledir.up, idx);
            int Down    = FindgTile(Tiles.Tiledir.down, idx);

            TileInit(idx, Left,  Tiles.Tiledir.left);
            TileInit(idx, Right, Tiles.Tiledir.right);
            TileInit(idx, Up,	  Tiles.Tiledir.up);
            TileInit(idx, Down,  Tiles.Tiledir.down );

            AddCloseList(idx);

            //오픈리스트가 비어있다면 갈 수 없음
            if (OpenList.Count == 0)
                return;

            Tiles tCmpTiles;

            if (Left != -1)
            {
                tCmpTiles = TileList[Left].GetComponent<Tiles>();
                if (tCmpTiles.X == m_tFinal.X && tCmpTiles.Y == m_tFinal.Y && 
                    Mathf.Abs(tInSearch.Z - tCmpTiles.Z) <= m_iPlayerJump)
                {
                    m_bIsGoal = true;
                    return;
                }
            }

            if (Right != -1)
            {
                tCmpTiles = TileList[Right].GetComponent<Tiles>();
                if (tCmpTiles.X == m_tFinal.X && tCmpTiles.Y == m_tFinal.Y &&
                    Mathf.Abs(tInSearch.Z - tCmpTiles.Z) <= m_iPlayerJump)
                {
                    m_bIsGoal = true;
                    return;
                }
            }

            if (Up != -1)
            {
                tCmpTiles = TileList[Up].GetComponent<Tiles>();
                if (tCmpTiles.X == m_tFinal.X && tCmpTiles.Y == m_tFinal.Y &&
                    Mathf.Abs(tInSearch.Z - tCmpTiles.Z) <= m_iPlayerJump)
                {
                    m_bIsGoal = true;
                    return;
                }
            }

            if (Down != -1)
            {
                tCmpTiles = TileList[Down].GetComponent<Tiles>();
                if (tCmpTiles.X == m_tFinal.X && tCmpTiles.Y == m_tFinal.Y &&
                    Mathf.Abs(tInSearch.Z - tCmpTiles.Z) <= m_iPlayerJump)
                {
                    m_bIsGoal = true;
                    return;
                }
            }
        }
    }

    void Result()
    {
        if (!m_bIsGoal)
            return;
        int idx = Getidx(m_tFinal.X, m_tFinal.Y);
        m_pPlayerMove.bIsMove = true;
        while (true)
        {
            if (idx == Getidx(m_tStart.X, m_tStart.Y))
            {
                m_pPlayerMove.iBestCount = BestList.Count;
                m_pPlayerMove.bIsInit = true;
                return;
            }

            Tiles.Tiledir dir = TileList[idx].GetComponent<Tiles>().dir;
            BestList.Add(dir);

            switch(dir)
            {
                case Tiles.Tiledir.left:
                    idx += 1;
                    break;
                case Tiles.Tiledir.right:
                    idx -= 1;
                    break;
                case Tiles.Tiledir.up:
                    idx += GameData.MaxTile[GameData.iNowMap, 0];
                    break;
                case Tiles.Tiledir.down:
                    idx -= GameData.MaxTile[GameData.iNowMap, 0];
                    break;
            }
        }
    }

    public int Getidx(int X, int Y)
    {
        return Y * GameData.MaxTile[GameData.iNowMap, 0] + X;
    }

    int Getidx(Tiles gTile)
    {
        return Getidx(gTile.X, gTile.Y);
    }

    void AddOpenList(int idx)
    {
        OpenList.Add(idx);
        TileList[idx].GetComponent<Tiles>().bIsOpen = true;
    }

    void AddCloseList(int idx)
    {
        CloseList.Add(idx);
        TileList[idx].GetComponent<Tiles>().bIsOpen = false;
        TileList[idx].GetComponent<Tiles>().bIsClose = true;
        OpenList.Remove(idx);
    }

    void Addm_MoveOpen(int idx)
    {
        m_MoveOpen.Add(idx);
        TileList[idx].GetComponent<Tiles>().bIsOpen = true;
    }

    void Addm_MoveClose(int idx)
    {
        m_MoveClose.Add(idx);
        TileList[idx].GetComponent<Tiles>().bIsOpen = false;
        TileList[idx].GetComponent<Tiles>().bIsClose = true;
        m_MoveOpen.Remove(idx);
    }

    int FindgTile(Tiles.Tiledir dir, int idx)
    {
        int result = idx;
        switch (dir)
        {
            case Tiles.Tiledir.left:
                if (idx % GameData.MaxTile[GameData.iNowMap, 0] != 0)
                    result -= 1;
                else
                    result = -1;
                break;
            case Tiles.Tiledir.up:
                if (idx >= GameData.MaxTile[GameData.iNowMap, 0])
                    result -= GameData.MaxTile[GameData.iNowMap, 0];
                else
                    result = -1;
                break;
            case Tiles.Tiledir.right:
                if (idx % GameData.MaxTile[GameData.iNowMap, 0] != GameData.MaxTile[GameData.iNowMap, 0] - 1)
                    result += 1;
                else
                    result = -1;
                break;
            case Tiles.Tiledir.down:
                if (idx < GameData.MaxTile[GameData.iNowMap, 0] * (GameData.MaxTile[GameData.iNowMap, 1] - 1))
                    result += GameData.MaxTile[GameData.iNowMap, 0];
                else
                    result = -1;
                break;
        }
        return result;
    }

    void TileInit(int now, int move, Tiles.Tiledir dir)
    {
        if (move == -1)
            return;

        Tiles movegTile = TileList[move].GetComponent<Tiles>();
        Tiles nowgTile = TileList[now].GetComponent<Tiles>();

        if (movegTile.bIsClose == true)
            return;

        if (!m_bIsFindRange)
        {
            if (movegTile.Z != 0)
            {
                if (Mathf.Abs(nowgTile.Z - movegTile.Z) <= m_iPlayerJump)
                {
                    if (movegTile.bIsOpen)
                    {
                        if (movegTile.g > nowgTile.g + 1)
                        {
                            movegTile.g = nowgTile.g + 1;
                            movegTile.f = movegTile.g + movegTile.h;
                            movegTile.dir = dir;
                        }
                    }
                    else
                    {
                        movegTile.g = nowgTile.g + 1;
                        movegTile.h = Mathf.Abs(m_tFinal.X - movegTile.X) + Mathf.Abs(m_tFinal.Y - movegTile.Y);
                        movegTile.f = movegTile.g + movegTile.h;
                        movegTile.dir = dir;
                        AddOpenList(move);
                        //Debug.Log("X " + move % 12 + '\t' + "Y " + (move / 12) + '\t' + "G" + movegTile.g);
                    }
                }
            }
        }
        else
        {
            if (movegTile.Z != 0 )
            {
                if (Mathf.Abs(nowgTile.Z - movegTile.Z) <= m_iPlayerJump)
                {
                    if (nowgTile.g == GameData.gNowPlayer.GetComponent<Status>().iMovingRange)
                        return;
                    if (movegTile.bIsOpen)
                    {
                        if (movegTile.g > nowgTile.g + 1)
                        {
                            movegTile.g = nowgTile.g + 1;
                        }
                    }
                    else
                    {
                        movegTile.g = nowgTile.g + 1;
                        Addm_MoveOpen(move);
                        TileList[move].GetComponent<Tiles>().SetMoveRange(true);
//                        Debug.Log("X " + move % 12 + '\t' + "Y " + (move / 12) + '\t' + "G" + movegTile.g);
                    }
                }
            }
        }
    }
}
