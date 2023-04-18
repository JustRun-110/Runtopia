using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

namespace sjb
{
    public class LobbyManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("방목록패널")]
        private GameObject roomListPanel;

        [Tooltip("방생성 패널")]
        private GameObject createPanel;

        [Tooltip("대기방 패널")]
        private GameObject roomPanel;

        [Tooltip("캐릭터 선택 패널")]
        private GameObject charactersPanel;

        [Tooltip("참가자목록 패널")]
        private GameObject otherPanel;

        PhotonManager pm;

        private void Awake()
        {
            roomListPanel = GameObject.Find("RoomList Panel");
            createPanel = GameObject.Find("Create Panel");
            roomPanel = GameObject.Find("Room Panel");
            charactersPanel = GameObject.Find("Characters");
            otherPanel = GameObject.Find("Other Player");
        }

        #region 판넬 관련
        private void Start()
        {
            roomListPanel.SetActive(true);
            createPanel.SetActive(false);
            roomPanel.SetActive(false);
            charactersPanel.SetActive(false);
            pm = GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
        }

        public void OnCloseRoomList()
        {
            roomListPanel.SetActive(false);
        }

        public void OnCloseCreate()
        {
            createPanel.SetActive(false);
        }
        public void OnOpenCreate()
        {
            createPanel.SetActive(true);
        }
        public void OnOpenCharacters()
        {
            if (charactersPanel.activeSelf)
            {
                charactersPanel.SetActive(false);
                otherPanel.SetActive(true);
            }
            else
            {
                charactersPanel.SetActive(true);
                pm.CleanOther();
                otherPanel.SetActive(false);
            }
        }
        public void OnCloseCharacters()
        {
            charactersPanel.SetActive(false);
            otherPanel.SetActive(true);
            //pm.SetOtherPlayer();
        }
        #endregion

        public void GameQuit()
        {
            Application.Quit();
        }
        void OnApplicationQuit()
        {
            Request.Instance.DisconnectServer();
        }
    }
}
