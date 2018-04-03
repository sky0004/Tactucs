using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour {

    [SerializeField]
    private List<GameObject> m_PlayerList = new List<GameObject>();
    private Status[] m_Turn;
    private int m_iMin;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < transform.childCount; i++)
        {
            m_PlayerList.Add(transform.GetChild(i).gameObject);
        }

        int Max = int.MinValue;

        for (int i = 0; i < m_PlayerList.Count; i++)
        {
            Status sPlayer = m_PlayerList[i].GetComponent<Status>();
            if (Max < sPlayer.iSpeed)
            {
                Max = sPlayer.iSpeed;

            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(GameData.iNowState == 0)
        {
            for(int i = 0; i < m_PlayerList.Count; i++)
            {
                m_PlayerList[i].GetComponent<PlayerAct>().bIsMoved = false;
            }
        }
	}
}
