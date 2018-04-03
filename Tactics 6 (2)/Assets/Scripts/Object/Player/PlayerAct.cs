using UnityEngine;
using System.Collections;

public class PlayerAct : MonoBehaviour {

    public int X, Y, Z; //타일상 좌표
    public bool bIsMove = false;
    public bool bIsInit = false;
    public bool bIsMoved = false;
    public MapManager MapManager;
    public int iBestCount;
    public GameObject gDirArrow;

    private Tiles.Tiledir[] m_BestArr;
    private Status m_IsStatus;
    private Tiles m_IsOnTiles;
    private Vector3 m_vTemp = new Vector3(0, -1.05f, 0.02f);
    private Animator m_animator;
    private Vector3 m_vEndPos;
    private Vector3 m_vStartPos;
    private Vector3 m_vPlus;
    private Vector2 m_mousePos;
    private float m_fProgress;
    private bool m_bIsRun;
    private int m_iDiff;


    private SpriteRenderer sp;

    private Vector3 vX, vY, vZ;

    void Start() {
        GameData.gNowPlayer = this.gameObject;

        X = 11;
        Y = 7;
        Z = 0;

        vX = new Vector3(GameData.vX.x, GameData.vX.y, GameData.vX.z);
        vY = new Vector3(GameData.vY.x, GameData.vY.y, GameData.vY.z);
        vZ = new Vector3(GameData.vZ.x, GameData.vZ.y, GameData.vZ.z);

        m_bIsRun = false;

        this.transform.position = (new Vector3(0, 0, 0) + (GameData.vX * X) 
            + (GameData.vY * Y) + (GameData.vZ * Z) - m_vTemp);

        m_animator = GetComponent<Animator>();
        sp = GetComponent<SpriteRenderer>();
        m_IsStatus = GetComponent<Status>();

        m_IsOnTiles = MapManager.TileList[MapManager.Getidx(X, Y)].GetComponent<Tiles>();
        m_IsOnTiles.gIsOnPlayer = this.gameObject;
    }

    void Update()
    {
        Moveing();

        if (GameData.iNowState == 8)
        {
            gDirArrow.SetActive(true);
            if (Input.GetMouseButton(0))
            {
                Vector2 PlayerPos = transform.position;
                m_mousePos = Camera.main.ScreenToWorldPoint(
                new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));

                if (PlayerPos.x < m_mousePos.x && PlayerPos.y > m_mousePos.y)
                {
                    m_IsStatus.nowDir = Tiles.Tiledir.right;
                    m_animator.Play("Lower Idle");
                    sp.flipX = true;
                    gDirArrow.SetActive(false);

                    GameData.iNowState = 0;
                }

                else if (PlayerPos.x > m_mousePos.x && PlayerPos.y < m_mousePos.y)
                {
                    m_IsStatus.nowDir = Tiles.Tiledir.left;
                    m_animator.Play("Upper Idle");
                    sp.flipX = false;
                    gDirArrow.SetActive(false);

                    GameData.iNowState = 0;
                }

                else if (PlayerPos.x > m_mousePos.x && PlayerPos.y > m_mousePos.y)
                {
                    m_IsStatus.nowDir = Tiles.Tiledir.down;
                    m_animator.Play("Lower Idle");
                    sp.flipX = false;
                    gDirArrow.SetActive(false);

                    GameData.iNowState = 0;
                }

                else if (PlayerPos.x < m_mousePos.x && PlayerPos.y < m_mousePos.y)
                {
                    m_IsStatus.nowDir = Tiles.Tiledir.up;
                    m_animator.Play("Upper Idle");
                    sp.flipX = true;
                    gDirArrow.SetActive(false);

                    GameData.iNowState = 0;
                }
            }
        }
    }

    void Moveing()
    {
        if (bIsMove)
        {
            if (iBestCount > 0 && !m_bIsRun)
            {
                if(bIsInit)
                {
                    m_IsOnTiles.gOnTile.GetComponent<Tile>().bChClick = true;
                    m_BestArr = new Tiles.Tiledir[iBestCount];
                    for (int i = 0; i < iBestCount; i++)
                    {
                        m_BestArr[i] = MapManager.BestList[i];
                    }
                }

                GameData.iNowState = 2;

                switch (m_BestArr[iBestCount - 1])
                {
                    case Tiles.Tiledir.right:
                        StartCoroutine(RightCase());
                        break;
                    case Tiles.Tiledir.left:
                        StartCoroutine(LeftCase());
                        break;
                    case Tiles.Tiledir.up:
                        StartCoroutine(UpCase());
                        break;
                    case Tiles.Tiledir.down:
                        StartCoroutine(DownCase());
                        break;
                }

                GameData.bIsClick = false;
            }

            if (iBestCount == 0)
            {
                bIsMove = false;
                bIsMoved = true;
                MapManager.TileList[MapManager.Getidx(X, Y)].GetComponent<Tiles>().gIsOnPlayer = this.gameObject;
                MapManager.BestList.Clear();

                m_IsOnTiles = MapManager.TileList[MapManager.Getidx(X, Y)].GetComponent<Tiles>();
                m_IsOnTiles.gOnTile.GetComponent<Tile>().bChClick = false;
                m_IsOnTiles.gIsOnPlayer = this.gameObject;

                if (m_IsStatus.bIsAction)
                    GameData.iNowState = 8;
                else
                    GameData.iNowState = 6;
                return;
            }
        }
        else
        {            
            this.transform.position = (new Vector3(0, 0, 0) + (vX * X) + 
                (GameData.vY * Y) + (GameData.vZ * Z) - m_vTemp);  
        }
    }   

    IEnumerator RightCase()
    {
        //만약 움직이는 상태가 아니라면
        //초기화
        if (!m_bIsRun)
        {
            X++;
            m_vStartPos = this.transform.position;
            m_vEndPos = m_vStartPos + vX;
            m_bIsRun = true;
            m_iDiff = 0;
            m_vPlus = new Vector3(0, 0, 0);
            m_fProgress = 0;
            GameData.iNowState = 2;
            sp.flipX = true;

            //높이가 지금 타일과 다르다면
            //높이관련 변수 초기화
            if (GameData.Map[GameData.iNowMap, Y, X] != GameData.Map[GameData.iNowMap, Y, X - 1])
            {
                m_iDiff = GameData.Map[GameData.iNowMap, Y, X] - GameData.Map[GameData.iNowMap, Y, X - 1];
                Z += m_iDiff;
                m_vEndPos += vZ * m_iDiff;
                m_animator.Play("Lower Jump");
            }
            else
            {
                m_animator.Play("Lower Move");
            }
            
        }
        
        //이동과정    
        while (true)
        {
            m_fProgress = Mathf.Min(1, m_fProgress + Time.deltaTime * 3);

            Vector3 lerpPos = transform.position = Vector3.Lerp(m_vStartPos, m_vEndPos, m_fProgress);
            lerpPos.z = m_vEndPos.z;
            transform.position = lerpPos;
            //도착했다면
            if (m_fProgress >= 1)
            {
                m_bIsRun = false;
                iBestCount--;
                StopCoroutine(RightCase());
                if(iBestCount == 0)
                    m_animator.Play("Lower Idle");
                break;
            }
            yield return null;
        }
    }

    IEnumerator LeftCase()
    {
        if (!m_bIsRun)
        {
            X--;
            m_vStartPos = this.transform.position;
            m_vEndPos = m_vStartPos - vX;
            m_bIsRun = true;
            m_iDiff = 0;
            m_vPlus = new Vector3(0, 0, 0);
            m_fProgress = 0;
            GameData.iNowState = 2;
            sp.flipX = false;
            m_IsStatus.nowDir = Tiles.Tiledir.left;

            if (GameData.Map[GameData.iNowMap, Y, X] != GameData.Map[GameData.iNowMap, Y, X + 1])
            {
                m_iDiff = GameData.Map[GameData.iNowMap, Y, X] - GameData.Map[GameData.iNowMap, Y, X + 1];
                Z += m_iDiff;
                m_vEndPos += vZ * m_iDiff;
                m_animator.Play("Upper Jump");
            }
            else
            {
                m_animator.Play("Upper Move");
            }
        }
        while (true)
        {
            m_fProgress = Mathf.Min(1, m_fProgress + Time.deltaTime * 3);

            Vector3 lerpPos = Vector3.Lerp(m_vStartPos, m_vEndPos, m_fProgress);
            lerpPos.z = m_vStartPos.z;
            transform.position = lerpPos;
            //도착했다면
            if (m_fProgress >= 1)
            {
                m_bIsRun = false;
                iBestCount--;
                StopCoroutine(LeftCase());
                if (iBestCount == 0)
                    m_animator.Play("Upper Idle");
                break;
            }
            yield return null;
        }
    }

    IEnumerator UpCase()
    {
        if (!m_bIsRun)
        {
            Y--;
            m_vStartPos = this.transform.position;
            m_vEndPos = m_vStartPos - vY;
            m_bIsRun = true;
            m_iDiff = 0;
            m_vPlus = new Vector3(0, 0, 0);
            m_fProgress = 0;
            GameData.iNowState = 2;
            sp.flipX = true;
            m_IsStatus.nowDir = Tiles.Tiledir.up;

            if (GameData.Map[GameData.iNowMap, Y, X] != GameData.Map[GameData.iNowMap, Y + 1, X])
            {
                m_iDiff = GameData.Map[GameData.iNowMap, Y, X] - GameData.Map[GameData.iNowMap, Y + 1, X];
                Z += m_iDiff;
                m_vEndPos += vZ * m_iDiff;
                m_animator.Play("Upper Jump");
            }
            else
            {
                m_animator.Play("Upper Move");
            }
        }
        while (true)
        {
            m_fProgress = Mathf.Min(1, m_fProgress + Time.deltaTime * 3);

            Vector3 lerpPos = Vector3.Lerp(m_vStartPos, m_vEndPos, m_fProgress);
            lerpPos.z = m_vStartPos.z;
            transform.position = lerpPos;
            //도착했다면
            if (m_fProgress >= 1)
            {
                m_bIsRun = false;
                iBestCount--;
                if (iBestCount == 0)
                    m_animator.Play("Upper Idle");
                break;
            }
            yield return null;

        }
    }

    IEnumerator DownCase()
    {
        if (!m_bIsRun)
        {
            Y++;
            m_vStartPos = this.transform.position;
            m_vEndPos = m_vStartPos + vY;
            m_bIsRun = true;
            m_iDiff = 0;
            m_vPlus = new Vector3(0, 0, 0);
            m_fProgress = 0;
            GameData.iNowState = 2;
            sp.flipX = false;
            m_IsStatus.nowDir = Tiles.Tiledir.down;
            //높이가 지금 타일과 다르다면
            //높이관련 변수 초기화
            if (GameData.Map[GameData.iNowMap, Y, X] != GameData.Map[GameData.iNowMap, Y - 1, X])
            {
                m_iDiff = GameData.Map[GameData.iNowMap, Y, X] - GameData.Map[GameData.iNowMap, Y - 1, X];
                Z += m_iDiff;
                m_vEndPos += vZ * m_iDiff;
                m_animator.Play("Lower Jump");
            }
            else
            {
                m_animator.Play("Lower Move");
            }
        }
        while (true)
        {
            m_fProgress = Mathf.Min(1, m_fProgress + Time.deltaTime * 3);

            //도착했다면
            Vector3 lerpPos = transform.position = Vector3.Lerp(m_vStartPos, m_vEndPos, m_fProgress);
            lerpPos.z = m_vEndPos.z;
            transform.position = lerpPos;
            if (m_fProgress >= 1)
            {
                m_bIsRun = false;
                iBestCount--;
                StopCoroutine(DownCase());
                if (iBestCount == 0)
                    m_animator.Play("Lower Idle");
                break;
            }
            yield return null;
        }
    }
}