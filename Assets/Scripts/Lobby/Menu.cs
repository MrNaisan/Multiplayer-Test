using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviourPunCallbacks
{
    public InputField CreateInputField;
    public InputField JoinInputField;
    public InputField NickName;
    public GameObject LobbyPanel;
    public GameObject LoadingPanel;
    public GameObject CreateError;
    public GameObject JoinError;
    public GameObject NickError;
    int minPlayersToStart => GameData.Default.MinPlayersToStart;
    int maxPlayersInRoom => GameData.Default.MaxPlayersInRoom;
    int connectedPlayers = 0;
    PhotonView pView;
    GameObject prevError;

    private void Start() 
    {
        pView = GetComponent<PhotonView>();    
        if(!string.IsNullOrWhiteSpace(PhotonNetwork.NickName))
            NickName.text = PhotonNetwork.NickName;
    }

    public void CreateRoom()
    {
        if(!string.IsNullOrWhiteSpace(NickName.text))
        {
            if(!string.IsNullOrWhiteSpace(CreateInputField.text))
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = maxPlayersInRoom;
                roomOptions.PublishUserId = true;
                PhotonNetwork.CreateRoom(CreateInputField.text, roomOptions);
            }
            else
            {
                OnCreateRoomFailed(1, "Not valide string");
            }
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ShowError(NickError));
        }
    }

    

    public void JoinRoom()
    {
        if(!string.IsNullOrWhiteSpace(NickName.text))
        {
            if(!string.IsNullOrWhiteSpace(JoinInputField.text))
                PhotonNetwork.JoinRoom(JoinInputField.text);
            else
                OnJoinRoomFailed(1, "Not valide string");
        }
        else
        {
            StopAllCoroutines();
            StartCoroutine(ShowError(NickError));
        }
    }

    public override void OnJoinedRoom()
    {
        LoadingPanel.SetActive(true);
        LobbyPanel.SetActive(false);
        PhotonNetwork.NickName = NickName.text;
        pView.RPC("StartGame", RpcTarget.AllBuffered);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowError(JoinError));
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        StopAllCoroutines();
        StartCoroutine(ShowError(CreateError));
    }

    [PunRPC]
    public void StartGame()
    {
        connectedPlayers++;
        if (connectedPlayers >= minPlayersToStart)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }

    [PunRPC]
    public void LeaveLobby()
    {
        connectedPlayers--;
    }

    public void Lobby()
    {
        LoadingPanel.SetActive(false);
        LobbyPanel.SetActive(true);
        pView.RPC("LeaveLobby", RpcTarget.AllBuffered);
        PhotonNetwork.LeaveRoom();
    }

    IEnumerator ShowError(GameObject obj)
    {
        if(prevError != null)
            prevError.SetActive(false);
        prevError = obj;
        obj.SetActive(true);
        yield return new WaitForSeconds(2f);
        obj.SetActive(false);
        prevError = null;
    }
}
