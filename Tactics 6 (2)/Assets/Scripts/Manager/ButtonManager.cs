using UnityEngine;

public class ButtonManager : MonoBehaviour {

    public GameObject move;
    public GameObject action;
    public GameObject wait;
    public GameObject end;

    private Status m_IsStatus;

    // Update is called once per frame
    void Update () {
        if (GameData.fClickDelay > 0)
            GameData.fClickDelay -= Time.deltaTime;
        else
            GameData.fClickDelay = 0;

        if (GameData.iNowState == 1)
        {
            m_IsStatus = GameData.gNowPlayer.GetComponent<Status>();
            m_IsStatus.bIsMoved = false;
            m_IsStatus.bIsAction = false;
            move.SetActive(true);
            action.SetActive(true);
            wait.SetActive(true);
        }

        if (GameData.iNowState == 0 || GameData.iNowState == 2 || GameData.iNowState == 3 || 
            GameData.iNowState == 4 || GameData.iNowState == 5 || GameData.iNowState == 8)
        {
            move.SetActive(false);
            action.SetActive(false);
            wait.SetActive(false);

            if (GameData.iNowState == 0)
            {
                end.SetActive(true);
            }
        }

        if(GameData.iNowState == 6)
        {
            move.SetActive(false);
            action.SetActive(true);
            wait.SetActive(true);
        }

        if(GameData.iNowState == 7)
        {
            move.SetActive(true);
            action.SetActive(false);
            wait.SetActive(true);
        }
	}
    

    public void moveBt()
    {
        if (GameData.fClickDelay == 0)
        {
            GameData.gNowPlayer.GetComponent<Status>().bIsMoved = true;
            GameData.iNowState = 4;
            GameData.fClickDelay = 0.1f;
        }
    }

    public void actionBt()
    {
        if (GameData.fClickDelay == 0)
        {
            GameData.gNowPlayer.GetComponent<Status>().bIsAction = true;
            GameData.fClickDelay = 0.1f;
        }
    }

    public void waitBt()
    {
        if (GameData.fClickDelay == 0)
        {
            GameData.iNowState = 8;
            GameData.fClickDelay = 0.1f;
        }
    }

    public void endBt()
    {
        if(GameData.fClickDelay == 0)
        {
            GameData.iNowState = 9;
            GameData.fClickDelay = 0.1f;
        }
    }
}
