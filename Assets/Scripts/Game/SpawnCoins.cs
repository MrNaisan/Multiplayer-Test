using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;

public class SpawnCoins : MonoBehaviour
{
    int maxCount => GameData.Default.MaxCoinsCount;
    float time => GameData.Default.CoinsSpawnRate;
    PhotonView photonView;
    public float MaxX, MinX, MaxY, MinY;
    [NonSerialized]
    public int coinsCount;

    public void Start() 
    {
        photonView = GetComponent<PhotonView>();
        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnCour());
        }
    }

    IEnumerator SpawnCour()
    {
        while(true)
        {
            if(coinsCount < maxCount)
            {
                Vector2 randomPos = new Vector2(UnityEngine.Random.Range(MinX, MaxX), UnityEngine.Random.Range(MinY, MaxY));
                photonView.RPC("Spawn", RpcTarget.AllBuffered, randomPos);
            }
            yield return new WaitForSeconds(time);
        }
    }

    [PunRPC]
    public void Spawn(Vector2 pos)
    {
        Coin coin;
        PoolContainer.Active.coins.TryInstantiate(out coin, pos, Quaternion.identity);
        coinsCount++;
        coin.spawner = this;
    }
}
