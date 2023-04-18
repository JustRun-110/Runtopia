using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sjb
{
    public class CharacterSelect : MonoBehaviour
    {
        [Tooltip("참가한 나")]
        public GameObject myPrefab;

        [Tooltip("캐릭터의이름")]
        [SerializeField]
        public string characterName;

        [Tooltip("캐릭터판넬")]
        public GameObject characterPanel;

        [Tooltip("참가자목록 패널")]
        public GameObject otherPanel;

        PhotonManager pm;
        LobbyManager lp;
        // Start is called before the first frame update

        private void Start()
        {
            pm =GameObject.Find("PhotonManager").GetComponent<PhotonManager>();
            lp = GameObject.Find("LobbyManager").GetComponent<LobbyManager>();
        }
        public void ChangeCharacter()
        {
            lp.OnCloseCharacters();
            if (characterName.Equals("panda"))
            {
                pm.SetPanda();
            }
            else if (characterName.Equals("cat"))
            {
                pm.SetCat();
            }
            else if (characterName.Equals("dog"))
            {
                pm.SetDog();
            }
            else if (characterName.Equals("fox"))
            {
                pm.SetFox();
            }
            else if (characterName.Equals("deer"))
            {
                pm.SetDeer();
            }
            else if (characterName.Equals("duck"))
            {
                pm.SetDuck();
            }
            else if (characterName.Equals("tiger"))
            {
                pm.SetTiger();
            }
            else if (characterName.Equals("wolf"))
            {
                pm.SetWolf();
            }
            else if (characterName.Equals("raccoon"))
            {
                pm.SetRacoon();
            }
            else if (characterName.Equals("observer"))
            {
                pm.SetObserver();
            }
        }
    }

}