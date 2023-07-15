using UnityEngine;
using Photon.Pun;
using System;

public class WeaponLogic : MonoBehaviourPun
{
    public GameObject ProjectilePrefab;
    float projectileSpeed => GameData.Default.Weapon.ProjectileSpeed;
    float fireDelay => GameData.Default.Weapon.FireRate;
    private float lastFireTime;
    [NonSerialized]
    public int playerID;

    [PunRPC]
    public void FireRPC(Vector3 position, Quaternion rotation, int id)
    {
        if (Time.time - lastFireTime >= fireDelay)
        {
            Bullet projectile;
            PoolContainer.Active.bullets.TryInstantiate(out projectile, position, rotation);
            projectile.Enable(id);
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            projectileRb.velocity = transform.up * projectileSpeed;
            lastFireTime = Time.time;
        }
    }

    public void Fire()
    {
        if (!photonView.IsMine) return;

        photonView.RPC("FireRPC", RpcTarget.All, transform.position, transform.parent.rotation, playerID);
    }
}
