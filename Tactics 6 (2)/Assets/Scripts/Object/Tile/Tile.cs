using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    public int X, Y, Z; //타일상 좌표
    public bool bChClick;  //이 타일로 이동이 가능한지 체크
    public GameObject MovingRangeFx;
    public bool bMoveRangeOn;

    void Awake () {
        bMoveRangeOn = false;
    }
	
	void Update () {
        MovingRangeFx.SetActive(bMoveRangeOn);
        CastRay();
    }

    void CastRay() // 유닛 히트처리 부분.  레이를 쏴서 처리합니다. 
    {
        RaycastHit2D temp = new RaycastHit2D();

            //이동 가능한 상태이고, 클릭이 되었다면
        if (Input.GetMouseButtonUp(0) && bChClick && bMoveRangeOn && GameData.fClickDelay == 0)      
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero, 0f);

            if (hit.collider != null && temp != hit)
            {
                if (hit.collider.gameObject == this.gameObject)     //자신의 타일상 좌표를 반환
                {
                    if(GameData.iNowState == 0)
                    {

                    }

                    else if (GameData.iNowState == 4)
                    {
                        GameData.tFinalTile.X = X;
                        GameData.tFinalTile.Y = Y;
                        GameData.tFinalTile.Z = Z;
                        GameData.bIsClick = true;
                    }

                };
            }
        }
    }

    public bool GetMoveRangeOn()
    {
        return bMoveRangeOn;
    }

    public void SetMoveRangeOn(bool b)
    {
        bMoveRangeOn = b;
    }
}