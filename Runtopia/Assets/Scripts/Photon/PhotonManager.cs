using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using System;

using Hashtable = ExitGames.Client.Photon.Hashtable;
using System.Linq;
using UnityEngine.Rendering.Universal;
using System.Globalization;

namespace sjb
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        private readonly string version = "1.0";
        private string userNick = "NickInit";
        [Tooltip("로딩패널")]
        public GameObject loadingPanel;

        [Tooltip("방목록패널")]
        public GameObject roomListPanel;

        [Tooltip("대기방패널")]
        public GameObject roomPanel;

        private Dictionary<string, GameObject> rooms = new Dictionary<string, GameObject>();
        private List<RoomInfo> roomListSave;

        [Tooltip("방 프리팹")]
        public GameObject roomItemPrefab;
        [Tooltip("방 스크롤 목록")]
        public Transform scrollContent;

        [Tooltip("참가자 프리팹")]
        public GameObject joinItemPrefab;
        [Tooltip("참가한 나")]
        public GameObject myPrefab;

        [Tooltip("다른 참가자 목록")]
        public Transform scrollJoinContent;
        private Dictionary<int, GameObject> playerListEntries;
        [Tooltip("참가자목록 패널")]
        public GameObject otherPanel;

        [Tooltip("게임시작 버튼")]
        public Button StartGameButton;

        //선택한 캐릭터 번호
        [Tooltip("캐릭터")]
        public static int CharacterNum = 0;

        // 방등록
        public TMP_InputField roomName;
        public TMP_InputField roomPerson;
        public TMP_InputField roomPassword;


        // 참가자
        public Image PlayerColorImage;
        public Button PlayerReadyButton;
        public Image PlayerReadyImage;
        public bool isPlayerReady = false;


        void Awake()
        {

            PhotonNetwork.AutomaticallySyncScene = true;

            PhotonNetwork.GameVersion = version;


            if (PhotonNetwork.IsConnected == false)
            {
                PhotonNetwork.ConnectUsingSettings();
            }

        }
        public void Reset()
        {
            PlayerPrefs.DeleteAll();
        }
        void Start()
        {
            userNick = PlayerPrefs.GetString("USER_NickName");
            PhotonNetwork.NickName = userNick;
        }

        public override void OnConnectedToMaster()
        {
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            Debug.Log("로비에 접속함");
            loadingPanel.SetActive(false);
            //로비 입장
            if (!PlayerPrefs.GetString("isFirst", "yes").Equals("no"))
            {
                PlayerPrefs.SetString("isFirst", "no");
                MakeSquareRoom();
            }
        }
        public void MakeSquareRoom()
        {
            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = 20;
            ro.IsOpen = true; // 해당 방 입장가능
            ro.IsVisible = false; //목록에 표시 안되게
            PhotonNetwork.JoinOrCreateRoom("SquareSquareSquareSquare_fehfuehfue", ro, null);
        }

        public void OnOpenRoomList()
        {
            roomListPanel.SetActive(true);
            UpdateRoom();
        }

        public void UpdateRoom()
        {
            GameObject tempRoom = null;
            foreach (var room in roomListSave)
            {
                if (room.RemovedFromList == true)
                {
                    rooms.TryGetValue(room.Name, out tempRoom);
                    Destroy(tempRoom);
                    rooms.Remove(room.Name);
                }
                else
                {
                    if (rooms.ContainsKey(room.Name) == false)
                    {
                        GameObject _room = Instantiate(roomItemPrefab, scrollContent);
                        _room.GetComponent<RoomData>().RoomInfo = room;
                        rooms.Add(room.Name, _room);
                    }
                    else
                    {
                        rooms.TryGetValue(room.Name, out tempRoom);
                        tempRoom.GetComponent<RoomData>().RoomInfo = room;
                    }
                }
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            roomListSave = roomList;
            if (roomListPanel.activeSelf == true)
            {
                UpdateRoom();
            }
        }


        public override void OnCreatedRoom()
        {
            //방생성
        }
        #region 방 생성

        string SetRoomName()
        {
            String name;
            if (string.IsNullOrEmpty(roomName.text))
            {
                name = $"{userNick}";
            }
            else
            {
                name = $"{roomName.text}";
            }

            if (!roomPassword) return name;

            if (string.IsNullOrEmpty(roomPassword.text))
            {
                name += $"_{UnityEngine.Random.Range(1, 101):000}";
            }
            else
            {
                name += $"_{roomPassword.text}";
            }

            return name;
        }

        byte SetRoomPerson()
        {
            if (!string.IsNullOrEmpty(roomPerson.text))
            {
                return byte.Parse(roomPerson.text);
            }

            return 8;
        }

        public void OnMakeRoomClick(TMP_Text input)
        {
            //int num = 0;

            //Debug.Log(input.GetParsedText());
            //if(int.TryParse(input.GetParsedText(), out num))
            //{
            //    if (num > 9) return;
            //}
            //else
            //{
            //    return;
            //}

            RoomOptions ro = new RoomOptions();
            ro.MaxPlayers = SetRoomPerson();
            ro.IsOpen = true;
            ro.IsVisible = true;
            PhotonNetwork.CreateRoom(SetRoomName(), ro);
        }
        #endregion


        #region 방 입장
        public override void OnJoinedRoom()
        {
            //방에 접속시 진행
            if ("SquareSquareSquareSquare_fehfuehfue".Equals(PhotonNetwork.CurrentRoom.Name))
            {
                PhotonNetwork.LoadLevel("square");
                return;
            }
            //foreach (var player in PhotonNetwork.CurrentRoom.Players)
            //{
            //    Debug.Log($"{player.Value.NickName}, {player.Value.ActorNumber}");
            //}

            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }

            roomPanel.SetActive(true);


            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }
            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                //접속중인 모든 플레이어 확인
                object isPlayerReady;
                if (p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    //자기 자신에 값 적용
                    myPrefab.GetComponent<OtherPlayerListEntry>().Initialize(p, p.ActorNumber, p.NickName);

                    if (p.CustomProperties.TryGetValue("isReady", out isPlayerReady))
                    {
                        //자기 자신이 레디 되어있는지 설정
                        //myPrefab.GetComponent<OtherPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                    }
                    //플레이어 리스트에 추가
                    playerListEntries.Add(p.ActorNumber, myPrefab);
                }
                else
                {
                    GameObject entry = Instantiate(joinItemPrefab, scrollJoinContent);
                    //플레이어 추가
                    entry.GetComponent<OtherPlayerListEntry>().Initialize(p, p.ActorNumber, p.NickName);
                    if (p.CustomProperties.TryGetValue("isReady", out isPlayerReady))
                    {
                        entry.GetComponent<OtherPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                    }

                    object isPlayerCharacter;
                    if (p.CustomProperties.TryGetValue("character", out isPlayerCharacter))
                    {
                        entry.GetComponent<OtherPlayerListEntry>().SetCharacter((int)isPlayerCharacter);
                    }
                    playerListEntries.Add(p.ActorNumber, entry);
                }
            }
        }

        public void CleanOther()
        {
            Transform[] childList = scrollJoinContent.GetComponentsInChildren<Transform>();
            for (int i = 1; i < childList.Length; i++)
            {
                Destroy(childList[i].gameObject);
            }

            for (int i = 0; i < playerListEntries.Count; i++)
            //foreach (int actorNumber in playerListEntries.Keys)
            {
                int actorNumber = playerListEntries.Keys.ToList()[i];
                if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    //내가 아닌 다른 인원제거
                    GameObject entry = playerListEntries[actorNumber];
                    Destroy(entry.gameObject);
                    playerListEntries.Remove(actorNumber);
                }
            }
        }
        //playerListEntries를 출력하는 부분
        public void SetOtherPlayer()
        {
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }
            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                //접속중인 모든 플레이어 확인
                object isPlayerReady;
                object isPlayerCharacter;
                if (p.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    GameObject entry = Instantiate(joinItemPrefab, scrollJoinContent);
                    //플레이어 추가
                    entry.GetComponent<OtherPlayerListEntry>().Initialize(p, p.ActorNumber, p.NickName);
                    if (p.CustomProperties.TryGetValue("isReady", out isPlayerReady))
                    {
                        entry.GetComponent<OtherPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                    }
                    if (p.CustomProperties.TryGetValue("character", out isPlayerCharacter))
                    {
                        entry.GetComponent<OtherPlayerListEntry>().SetCharacter((int)isPlayerCharacter);
                    }
                    playerListEntries.Add(p.ActorNumber, entry);
                }
            }
        }
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            if (!otherPanel.activeSelf)
            {
                return;
            }
            //누군가 방에 입장
            GameObject entry = Instantiate(joinItemPrefab, scrollJoinContent);
            entry.GetComponent<OtherPlayerListEntry>().Initialize(newPlayer, newPlayer.ActorNumber, newPlayer.NickName);

            playerListEntries.Add(newPlayer.ActorNumber, entry);
        }
        #endregion
        #region 퇴장
        public void OnLeaveRoomButton()
        {
            //방에서 나갈때
            roomPanel.SetActive(false);
            roomPanel.GetComponentInChildren<GetRoomTitle>().ResetTitle();

            //플레이 인원 리셋
            foreach (int actorNumber in playerListEntries.Keys)
            {
                if (actorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    //내가 아닌 다른 인원이 나갈때 제거
                    GameObject entry = playerListEntries[actorNumber];
                    Destroy(entry.gameObject);
                }
            }

            playerListEntries.Clear();
            playerListEntries = null;
            if (PhotonNetwork.IsMasterClient)
            {
                //시작 버튼 비활성화
                StartGameButton.gameObject.SetActive(false);
            }

            //나가기
            PhotonNetwork.LeaveRoom();
        }

        //room에서 사람이 나갈때 호출
        public override void OnPlayerLeftRoom(Photon.Realtime.Player outPlayer)
        {

            //누군가 방에서 나감
            //해당 사람 내역 제거
            if (outPlayer.ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber)
            {
                //나 자신은 제가하지 않기
                if (otherPanel.activeSelf)
                {
                    Destroy(playerListEntries[outPlayer.ActorNumber].gameObject);
                    playerListEntries.Remove(outPlayer.ActorNumber);
                }
            }
            else
            {
                playerListEntries.Remove(outPlayer.ActorNumber);
            }

            if (PhotonNetwork.IsMasterClient)
            {
                //방장인 경우 시작 버튼 확인
                StartGameButton.gameObject.SetActive(CheckEveryPlayersReady());
            }
        }
        #endregion

        #region 게임 레디 하기
        public void OnReadyButton()
        {
            //자신의 레디 상태 변경
            isPlayerReady = !isPlayerReady;
            SetPlayerReady(isPlayerReady);

            //포톤을 통해 상태 변경
            Hashtable props = new Hashtable() { { "isReady", isPlayerReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }
        public void SetPlayerReady(bool playerReady)
        {
            //start는 레디를 누른상태
            //false는 안누름
            PlayerReadyButton.GetComponentInChildren<TMP_Text>().text = playerReady ? "Cancle!" : "Ready";
        }

        //모든인원 준비 확인
        private bool CheckEveryPlayersReady()
        {
            //모든 인원이 ready했는지 확인
            if (!PhotonNetwork.IsMasterClient)
            {
                return false;
            }

            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                object isPlayerReady;
                if (p.CustomProperties.TryGetValue("isReady", out isPlayerReady))
                {
                    if (!(bool)isPlayerReady)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void OnStartGameButtonClicked()
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible = false;

            PhotonNetwork.LoadLevel("Game_Multi");
        }

        #endregion

        #region 캐릭터 변경
        public void OnCharacterSet(int characterNum)
        {
            CharacterNum = characterNum;

            //포톤을 통해 상태 변경
            Hashtable props = new Hashtable() { { "character", CharacterNum } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
            //이제 출력
            SetOtherPlayer();

        }


        public void SetPanda()
        {
            OnCharacterSet(0);
        }
        public void SetCat()
        {
            OnCharacterSet(1);
        }
        public void SetDog()
        {
            OnCharacterSet(2);
        }
        public void SetDeer()
        {
            OnCharacterSet(3);
        }
        public void SetDuck()
        {
            OnCharacterSet(4);
        }
        public void SetFox()
        {
            OnCharacterSet(5);
        }
        public void SetRacoon()
        {
            OnCharacterSet(6);
        }
        public void SetTiger()
        {
            OnCharacterSet(7);
        }
        public void SetWolf()
        {
            OnCharacterSet(8);
        }

        public void SetObserver()
        {
            OnCharacterSet(9);
        }

        #endregion


        public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            GameObject entry;
            if (playerListEntries == null)
            {
                playerListEntries = new Dictionary<int, GameObject>();
            }
            //누군가 변경됨
            if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
            {
                object isPlayerReady;
                object isPlayerCharacter;

                if (targetPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
                {
                    if (changedProps.TryGetValue("isReady", out isPlayerReady))
                    {
                        myPrefab.GetComponent<OtherPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                    }
                    if (changedProps.TryGetValue("character", out isPlayerCharacter))
                    {
                        myPrefab.GetComponent<OtherPlayerListEntry>().SetCharacter((int)isPlayerCharacter);
                    }
                }
                else
                {
                    if (otherPanel.activeSelf)
                    {
                        if (changedProps.TryGetValue("isReady", out isPlayerReady))
                        {
                            entry.GetComponent<OtherPlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
                        }
                        if (changedProps.TryGetValue("character", out isPlayerCharacter))
                        {
                            entry.GetComponent<OtherPlayerListEntry>().SetCharacter((int)isPlayerCharacter);
                        }
                    }
                }
            }

            if (PhotonNetwork.IsMasterClient)
            {
                StartGameButton.gameObject.SetActive(CheckEveryPlayersReady());
            }
        }
    }

}
