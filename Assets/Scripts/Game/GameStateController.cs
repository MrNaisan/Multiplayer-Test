using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using TMPro;

public class GameStateController : MonoBehaviourPunCallbacks
{
    #region Singletone
    public GameStateController()
    {
        Active = this;
    }
    public static GameStateController Active { get; private set; }
    #endregion

    public GameObject ProcessPanel;
    public GameObject WinPanel;
    public GameObject FailPanel;
    public TextMeshProUGUI PlayerName;
    public TextMeshProUGUI CoinsCount;
    public GameObject ProcessUI;
    new PhotonView photonView;
    List<string> playersIds = new List<string>();
    string currentPlayerID;
    [NonSerialized]
    public PlayerController playerController;
    [NonSerialized]
    public PlayerController[] players;

    private void Start() 
    {
        photonView = GetComponent<PhotonView>();
        currentPlayerID = PhotonNetwork.LocalPlayer.UserId;
        AddPlayer();
    }

    public void AddPlayer()
    {
        photonView.RPC("AddPlayerRPC", RpcTarget.AllBuffered, currentPlayerID);
    }

    public void RemovePlayer()
    {
        photonView.RPC("RemovePlayerRPC", RpcTarget.AllBuffered, currentPlayerID);
    }

    [PunRPC]
    public void AddPlayerRPC(string id)
    {
        players = FindObjectsOfType<PlayerController>();
        playersIds.Add(id);
    }

    [PunRPC]
    public void RemovePlayerRPC(string id)
    {
        players = FindObjectsOfType<PlayerController>();
        playersIds.Remove(id);
        
        if(playersIds.Count == 1 && playersIds[0] == currentPlayerID)
        {
            photonView.RPC("SendWinRPC", RpcTarget.All, playerController.playerName, playerController.coins);
        }
    }

    [PunRPC]
    public void SendWinRPC(string name, int coinsCount)
    {
        ProcessPanel.SetActive(false);
        FailPanel.SetActive(false);
        WinPanel.SetActive(true);
        PlayerName.text = name;
        CoinsCount.text = $"{coinsCount}";
    }

    public void Lose()
    {
        ProcessPanel.SetActive(false);
        WinPanel.SetActive(false);
        FailPanel.SetActive(true);
    }

    public void OK()
    {
        WinPanel.SetActive(false);
        FailPanel.SetActive(false);
        ProcessPanel.SetActive(true);
        ProcessUI.SetActive(false);
    }

    public void Lobby()
    {
        RemovePlayer();
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }
}
