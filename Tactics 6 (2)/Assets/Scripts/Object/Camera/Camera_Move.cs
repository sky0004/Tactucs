using UnityEngine;

public class Camera_Move : MonoBehaviour {

    private GameObject player;

    void Start()
    {

    }

    void Update()
    {
        player = GameData.gNowPlayer;
        transform.position = new Vector3(player.transform.position.x, 
            player.transform.position.y, player.transform.position.z - 10);
    }

    void NextTurn()
    {
        player = GameData.gNowPlayer;
    }
}
