using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;
using TMPro;

public class PlayerController : MonoBehaviour
{
    Animator anim;
    SpriteRenderer playerSprite;
    SpriteRenderer weaponSprite;
    Rigidbody2D rb;
    Joystick joystick;
    WeaponLogic weapon;
    [NonSerialized]
    public PhotonView photonView;
    bool isFacingRight = true;
    int animState = 1;
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public Transform weaponTransform;
    float speed => GameData.Default.Player.Speed;
    public Image Healthbar;
    float maxHp => GameData.Default.Player.HealthPoint;
    float hp;
    [NonSerialized]
    public int playerID;
    [NonSerialized]
    public int coins;
    bool isDead = false;
    public TextMeshPro nickNameText;
    [NonSerialized]
    public string playerName;

    void Start()
    {
        hp = maxHp;

        rb = GetComponent<Rigidbody2D>();
        photonView = GetComponent<PhotonView>();
        weapon = GetComponentInChildren<WeaponLogic>();
        playerID = weapon.playerID = photonView.ViewID;

        if(photonView.IsMine)
        {
            PlayerPrefab.SetActive(true);
            anim = PlayerPrefab.GetComponent<Animator>();
            joystick = FindObjectOfType<Joystick>();
            FireController.Active.OnFire += weapon.Fire;
            FireController.Active.mainPlayerID = playerID;
            playerName = PhotonNetwork.NickName;
            GameStateController.Active.playerController = this;
        }
        else
        {
            EnemyPrefab.SetActive(true);
            anim = EnemyPrefab.GetComponent<Animator>();
            nickNameText.color = Color.red;
        }

        playerSprite = anim.GetComponent<SpriteRenderer>();
        weaponSprite = weaponTransform.GetComponentInChildren<SpriteRenderer>();
        photonView.RPC("UpdatePlayerName", RpcTarget.AllBuffered, playerName);
    }

    void Update()
    {
        if(photonView.IsMine && !isDead)
        {
            Movement();
        }
    }

    private void Movement()
    {
        float moveX = joystick.Horizontal;
        float moveY = joystick.Vertical;

        Vector2 moveDirection = new Vector2(moveX, moveY);

        moveDirection.Normalize();

        rb.velocity = moveDirection * speed;

        SetWeaponDirection(moveDirection);

        CheckFlip(moveX);

        CheckAnim();
    }

    private void CheckFlip(float direction)
    {
        if(direction < 0f)
        {
            if(isFacingRight)
                photonView.RPC("FlipSprite", RpcTarget.AllBuffered, !isFacingRight);
        }
        else if(direction > 0f)
        {
            if(!isFacingRight)
                photonView.RPC("FlipSprite", RpcTarget.AllBuffered, !isFacingRight);
        }
    }

    private void CheckAnim()
    {
        if(!joystick.IsJoystickActive)
        {
            if(animState == 2)
                photonView.RPC("ChangeAnim", RpcTarget.AllBuffered, 1);
        }
        else
        {
            if(animState == 1)
                photonView.RPC("ChangeAnim", RpcTarget.AllBuffered, 2);
        }
    }

    private void SetWeaponDirection(Vector2 moveDirection)
    {
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
            weaponTransform.rotation = Quaternion.Slerp(weaponTransform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }

    private void OnDestroy() 
    {
        if(photonView.IsMine)
            FireController.Active.OnFire -= weapon.Fire;    
    }

    public void Damage()
    {
        hp -= GameData.Default.Weapon.Damage;
        Healthbar.fillAmount = hp / maxHp;
        
        if(hp <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        if(photonView.IsMine)
        {
            FireController.Active.OnFire -= weapon.Fire;
            GameStateController.Active.RemovePlayer();
            GameStateController.Active.Lose();
        }

        isDead = true;
        gameObject.SetActive(false);
    }

    public void AddCoin()
    {
        if(photonView.IsMine)
        {
            coins++;
            CoinsCounter.Active.ChangeCoinsCount(coins);
        }
    }

    [PunRPC]
    private void FlipSprite(bool isFacingRight)
    {
        this.isFacingRight = isFacingRight;
        weaponSprite.flipY = !isFacingRight;
        playerSprite.flipX = !isFacingRight;
    }
    [PunRPC]
    private void UpdatePlayerName(string playerName)
    {
        this.playerName = playerName;

        nickNameText.text = playerName;
    }

    [PunRPC]
    public void DamageRPC(int id)
    {
        foreach(var player in GameStateController.Active.players)
        {
            if(player.photonView.ViewID == id)
            {
                player.Damage();
                return;
            }
        }
    }

    [PunRPC]
    public void ChangeAnim(int id)
    {
        animState = id;
        if(id == 1)
            anim.Play("Idle");
        else
            anim.Play("Run");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isFacingRight);
            stream.SendNext(playerName);
            stream.SendNext(animState);
        }
        else
        {
            isFacingRight = (bool)stream.ReceiveNext();
            weaponSprite.flipY = !isFacingRight;
            playerSprite.flipX = !isFacingRight;

            playerName = (string)stream.ReceiveNext();
            nickNameText.text = playerName;

            animState = (int)stream.ReceiveNext();
            if(animState == 1)
                anim.Play("Idle");
            else
                anim.Play("Run");
        }
    }
}
