using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using TMPro;
using Hashtable = ExitGames.Client.Photon.Hashtable;
namespace sjb
{
    public class PlayerListEntry : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_Text PlayerNameText;
        public Image PlayerReadyImage;

        private int ownerId;
        private bool isPlayerReady = false;


        private void Start()
        {
            Hashtable initialProps = new Hashtable() { { "isReady", isPlayerReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);
        }

        public void Initialize(int playerId, string playerName)
        {
            ownerId = playerId;
            PlayerNameText.text = playerName;
        }

        public void SetPlayerReady(bool playerReady)
        {
            //해당 플레이어에 ready를 했을때 로직
            isPlayerReady = playerReady;
            PlayerReadyImage.enabled = playerReady;
        }
    }
}
