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
    public class OtherPlayerListEntry : MonoBehaviour
    {
        [Header("UI References")]
        public TMP_Text PlayerNameText;
        public Image PlayerReadyImage;

        private int ownerId;
        private bool isPlayerReady = false;

        //캐릭터 설정
        [Tooltip("캐릭터들")]
        public GameObject panda;//기본 0
        public GameObject cat;//1
        public GameObject dog;//2
        public GameObject deer;//3
        public GameObject duck;//4
        public GameObject fox;//5
        public GameObject racoon;//6
        public GameObject tiger;//7
        public GameObject wolf;//8
        
        
        public int CharacterNum = 0;

        public void Initialize(Photon.Realtime.Player p,int playerId, string playerName)
        {
            //초기 값으로 설정
            if (p.CustomProperties.ContainsKey("character"))
            {
                object characterNum;
                object playerReady;
                if (p.CustomProperties.TryGetValue("character",out characterNum))
                {
                    CharacterNum = (int)characterNum;
                }
                if (p.CustomProperties.TryGetValue("isReady", out playerReady))
                {
                    isPlayerReady = (bool)playerReady;
                }
            }
            Hashtable initialProps = new Hashtable() { { "isReady", isPlayerReady }, { "character", CharacterNum },{"isWin",false} };
            p.SetCustomProperties(initialProps);

            ownerId = playerId;
            if (PlayerNameText != null)
            {
                PlayerNameText.text = playerName;
            }
            
            SetPanda();
        }
        public void SetPlayerReady(bool playerReady)
        {
            //해당 플레이어에 ready를 했을때 로직
            isPlayerReady = playerReady;
            PlayerReadyImage.enabled = playerReady;
        }
        public void SetCharacter(int num)
        {
            switch (num)
            {
                case 0:
                    SetPanda();
                    break;
                case 1:
                    SetCat();
                    break;
                case 2:
                    SetDog();
                    break;
                case 3:
                    SetDeer();
                    break;
                case 4:
                    SetDuck();
                    break;
                case 5:
                    SetFox();
                    break;
                case 6:
                    SetRacoon();
                    break;
                case 7:
                    SetTiger();
                    break;
                case 8:
                    SetWolf();
                    break;
            }
        }


        public void SetPanda()
        {
            ActiveCharacter("panda");
            CharacterNum = 0;
        }
        public void SetCat()
        {
            ActiveCharacter("cat");
            CharacterNum = 1;
        }
        public void SetDog()
        {
            ActiveCharacter("dog");
            CharacterNum = 2;
        }
        
        public void SetDeer()
        {
            ActiveCharacter("deer");
            CharacterNum = 3;
        }
        public void SetDuck()
        {
            ActiveCharacter("duck");
            CharacterNum = 4;
        }

        public void SetFox()
        {
            ActiveCharacter("fox");
            CharacterNum = 5;
        }
        public void SetRacoon()
        {
            ActiveCharacter("racoon");
            CharacterNum = 6;
        }
        public void SetTiger()
        {
            ActiveCharacter("tiger");
            CharacterNum = 7;
        }
        public void SetWolf()
        {
            ActiveCharacter("wolf");
            CharacterNum = 8;
        }

        public void ActiveCharacter(string character)
        {
            panda.SetActive(character.Equals(panda.name));
            cat.SetActive(character.Equals(cat.name));
            dog.SetActive(character.Equals(dog.name));
            deer.SetActive(character.Equals(deer.name));
            duck.SetActive(character.Equals(duck.name));
            fox.SetActive(character.Equals(fox.name));
            tiger.SetActive(character.Equals(tiger.name));
            wolf.SetActive(character.Equals(wolf.name));
            racoon.SetActive(character.Equals(racoon.name));
        }
    }
}