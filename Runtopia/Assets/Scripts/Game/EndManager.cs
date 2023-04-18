using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.SceneManagement;

namespace sjb{
    public class EndManager : MonoBehaviour
    {
        [Tooltip("승리자이름 text")]
        public TMP_Text winnerName;
        [Tooltip("승리자 캐릭터")]
        public GameObject winnerCh;

        [Tooltip("승리")]
        public TMP_Text winner;
        [Tooltip("실패")]
        public TMP_Text loser;


        private void Start()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            GetPlayerInfo();
        }

        public void GetPlayerInfo()
        {
            foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
            {
                Debug.Log("모든 인원이요"+p.NickName);
                object isPlayerWin;
                if (p.CustomProperties.TryGetValue("isWin", out isPlayerWin))
                {
                    if ((bool)isPlayerWin)
                    {
                        winnerName.gameObject.SetActive(true);
                        winner.gameObject.SetActive(true);
                    
                        //winner의 캐릭터 번호 얻기
                        object characterIndex;
                        if (p.CustomProperties.TryGetValue("character", out characterIndex))
                        {
                            Debug.Log("character index"+(int)characterIndex);
                            winnerCh.transform.GetChild((int)characterIndex).gameObject.SetActive(true);
                        }
                    
                        winnerCh.SetActive(true);
                        winnerName.text = p.NickName;
                    }
                }
            }
            if (!winnerCh.activeSelf)
            {
                loser.gameObject.SetActive(true);
            }
        }
    
        public void OnGotoSquare()
        {
            PlayerPrefs.SetString("isFirst", "yes");
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Lobby");
        }

        public void OnGotoRoomList()
        {
            PhotonNetwork.LeaveRoom();
            SceneManager.LoadScene("Lobby");
        }

        void OnApplicationQuit()
        {
            Request.Instance.DisconnectServer();
        }

    }
}
