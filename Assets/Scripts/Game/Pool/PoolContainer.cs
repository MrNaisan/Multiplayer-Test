using UnityEngine;
using System;

public class PoolContainer : MonoBehaviour
{
    #region Singletone
    public PoolContainer()
    {
        Active = this;
    }
    public static PoolContainer Active { get; private set; }
    #endregion
    
    public Bullet BulletPrefab;
    public Coin CoinPrefab;
    [NonSerialized]
    public Pool<Bullet> bullets;
    [NonSerialized]
    public Pool<Coin> coins;

    private void Awake() 
    {
        bullets = new Pool<Bullet>(BulletPrefab, 50, this.transform);
        coins = new Pool<Coin>(CoinPrefab, 50, this.transform);
    }
}
