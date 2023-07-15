using System.Collections;
using UnityEngine;
using System;

public class Bullet : PooledItem
{
    public GameObject PlayerBullet;
    public GameObject EnemyBullet;
    [NonSerialized]
    public int playerID;
    private int mainPlayerID => FireController.Active.mainPlayerID;

    public void Enable(int id)
    {
        playerID = id;

        if (mainPlayerID == playerID)
            PlayerBullet.SetActive(true);
        else
            EnemyBullet.SetActive(true);
        StartCoroutine(DisableCour());
    }

    IEnumerator DisableCour()
    {
        yield return new WaitForSeconds(3f);
        Disable();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (playerID != 0)
        {
            if (other.TryGetComponent<PlayerController>(out PlayerController player))
            {
                if (player.playerID != playerID)
                {
                    if(player.photonView.IsMine)
                        player.photonView.RPC("DamageRPC", Photon.Pun.RpcTarget.AllBuffered, player.photonView.ViewID);
                    Disable();
                }
            }
            else if(other.TryGetComponent<Wall>(out _))
                Disable();
        }
        else
            Disable();
    }

    void Disable()
    {
        StopAllCoroutines();
        playerID = 0;
        ReturnToPool();
        PlayerBullet.SetActive(false);
        EnemyBullet.SetActive(false);
    }
}
