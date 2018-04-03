using UnityEngine;

public static class GameData
{
    public static int iNowMap = 0;  //현재의 맵
    //인게임의 경우 0 턴 돌아온시점, 1 캐릭터 선택시, 2 이동중, 3 행동중, 4 이동 선택, 5 행동 선택, 6 이동 후, 7 행동 후, 8 방향 선택, 9 적 턴
    public static int iNowState = 0;
    public static float fClickDelay = 0;


    public static GameObject gNowPlayer = null;
    public static Tiles tStartTile = new Tiles();  //시작지점
    public static Tiles tFinalTile = new Tiles();  //최종 목적지
    public static bool bIsClick = false;           //타일이 정상적으로 클릭되었는가
    
    //public static Vector3 vX = new Vector3(0.6363636363f, 0.3636363636f, 0.5f);
    //public static Vector3 vY = new Vector3(-0.6363636363f, 0.3636363636f, 0.5f);
    //public static Vector3 vZ = new Vector3(0, 0.7272727272f, 0);

    //공식에 쓰이는 좌표
    public static Vector3 vX = new Vector3(0.7f, -0.4f, -0.5f);   
    public static Vector3 vY = new Vector3(-0.7f, -0.4f, -0.5f);
    public static Vector3 vZ = new Vector3(0, 0.8f, -0.5f);


    //몇번째 맵, [X 최대갯수, Y최대갯수]
    public static int[,] MaxTile =
    {
        { 12, 8 },
    };


    //몇번째 맵,Y, X에 쌓이는 타일 갯수
    public static int[,,] Map =
    {
        //첫번째 맵
        {
            {   6,  5,  5,  7,  2,  1,  1,  1,  1,  1,  1,  1   },
            {   5,  4,  4,  3,  1,  1,  1,  1,  1,  1,  1,  1   },
            {   4,  3,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1   },
            {   3,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1   },
            {   2,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1   },
            {   1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1   },
            {   1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1   },
            {   1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1,  1   },
        },

        //두번째 맵
    };
}