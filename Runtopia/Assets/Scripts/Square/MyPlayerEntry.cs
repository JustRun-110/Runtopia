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
    public class MyPlayerEntry : MonoBehaviour
    {
        [Header("UI References")]
        public Image PlayerReadyImage;

        private int ownerId;
        private bool isPlayerReady = false;

        private void Start()
        {
            Hashtable initialProps = new Hashtable() { { "isReady", isPlayerReady } };
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
        }

        public void Initialize(int playerId)
        {
            ownerId = playerId;
        }

        public void SetPlayerReady(bool playerReady)
        {
            //해당 플레이어에 ready를 했을때 로직
            isPlayerReady = playerReady;
            PlayerReadyImage.enabled = playerReady;
        }
    }
}