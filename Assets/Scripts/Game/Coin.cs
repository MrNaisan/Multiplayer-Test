using UnityEngine;
using System;

public class Coin : PooledItem
{
    [NonSerialized]
    public SpawnCoins spawner;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.TryGetComponent<PlayerController>(out PlayerController player))
        {
            ReturnToPool();
            spawner.coinsCount--;
            player.AddCoin();
        }    
    }
}
