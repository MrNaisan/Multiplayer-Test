using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public float MaxX, MinX, MaxY, MinY;

    private void Start() 
    {
        Vector2 pos = new Vector2(Random.Range(MinX, MaxX), Random.Range(MinY, MaxY));  
        PhotonNetwork.Instantiate(PlayerPrefab.name, pos, Quaternion.identity);  
    }
}
