using UnityEngine;

public class Status : MonoBehaviour {

    public int iHp;
    public int iMp;
    public int iOffense;
    public int iDefense;
    public int iSpeed;
    public int iJump;
    public int iMovingRange;
    public int iAttackRange;
    public Tiles.Tiledir nowDir;

    public bool bIsMoved = false;
    public bool bIsAction = false;
    public bool bIsEnd = false;


    void Start () {
        //기본값
        iHp = 100;
        iMp = 0;
        iOffense = 10;
        iDefense = 0;
        iSpeed = 3;
        iJump = 1;
        iMovingRange = 5;
        iAttackRange = 1;
    }
	
	void Update () {
		if(bIsAction && bIsMoved)
        {
            bIsEnd = true;
        }
        else
        {
            bIsEnd = false;
        }
	}
}
