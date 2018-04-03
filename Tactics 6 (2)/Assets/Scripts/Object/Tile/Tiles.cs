using UnityEngine;

public class Tiles : MonoBehaviour
{
    public enum Tiledir { left, up, right, down };
    public GameObject gOnTile;
    public GameObject gIsOnPlayer;
    public int X, Y, Z; //타일상 좌표
    public Tiledir dir;
    public int g, h, f;
    public bool bIsOpen;
    public bool bIsClose;

    [SerializeField]
    private GameObject m_gTile;

    // Use this for initialization
    void Start()
    {
        bIsOpen = false;
        bIsClose = false;
        TileCreate();
        this.transform.position = (new Vector3(0, 0, 0) + (GameData.vX * X) + (GameData.vY * Y));
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = (new Vector3(0, 0, 0) + (GameData.vX * X) + (GameData.vY * Y));

        if (GameData.iNowState == 0 && gIsOnPlayer != null)
            SetMoveRange(true);
        
    }

    void TileCreate()
    {
        //타일 생성
        if (Z != 0)
        {
            for (int i = 0; i < Z; i++)
            {
                GameObject temp = Instantiate(m_gTile);

                if (i != 0) //만약 자기 위에 타일이 있다면
                {
                    gOnTile.GetComponent<Tile>().bChClick = false; //타일을 올라갈 수 없는 상태로 만든다
                }
                gOnTile = temp; //지금 게임오브젝트를 넣어준다

                //올라갈 수 있는 상태로 만들어준다
                temp.GetComponent<Tile>().bChClick = true;
                //만든 타일에 타일상 좌표값을 전해준다
                temp.GetComponent<Tile>().X = X;
                temp.GetComponent<Tile>().Y = Y;
                temp.GetComponent<Tile>().Z = i;
                //부모를 정해준다
                temp.transform.SetParent(this.transform);
                //높이값을 준다
                temp.transform.position = (GameData.vZ * i);      
            }
        }
    }

    public void SetMoveRange(bool b)
    {
        gOnTile.GetComponent<Tile>().SetMoveRangeOn(b);
    }

    public bool GetMoveRangeOn()
    {
        return gOnTile.GetComponent<Tile>().GetMoveRangeOn();
    }
}