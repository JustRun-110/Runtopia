using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class RoomData : MonoBehaviour
{
    public TMP_Text RoomInfoText_Title;
    public TMP_Text RoomInfoText_Num;
    private RoomInfo _roomInfo;

    private string userIdText;
    public RoomInfo RoomInfo
    {
        get
        {
            return _roomInfo;
        }
        set
        {
            _roomInfo = value;
            string[] words = _roomInfo.Name.Split('_');
            RoomInfoText_Title.text = $"{words[0]}";
            RoomInfoText_Num.text = $"({_roomInfo.PlayerCount}/{_roomInfo.MaxPlayers})";
            //해당 룸 클릭시 해당 룸으로 이동
            GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => OnEnterRoom(_roomInfo.Name));
        }
    }

    private void Awake()
    {
        userIdText = PlayerPrefs.GetString("USER_NickName");
    }

    void OnEnterRoom(string roomName)
    {
        PhotonNetwork.NickName = userIdText;
        PhotonNetwork.JoinRoom(roomName);
    }
}
