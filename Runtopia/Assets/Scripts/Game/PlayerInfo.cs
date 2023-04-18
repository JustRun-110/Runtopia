using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
using garden;

namespace sjb
{
    public class PlayerInfo : MonoBehaviour
    {
        [Tooltip("player의 이름")]
        public TMP_Text playerName;

        [Tooltip("코인 출력")]
        private TMP_Text coinText;

        [Tooltip("아이템 출력")]
        private GameObject itemUI;

        private int cointCnt = 0;
        private string itemName = "";


        private new AudioSource audio;
        private bool itemCheck1=false; //1번아이템
        private bool itemCheck2=false; //2번아이템
        private bool itemCheck3=false; //2번아이템

        [SerializeField] 
        private Sprite speed;
        [SerializeField] 
        private Sprite jump;
        [SerializeField] 
        private Sprite tree;
        [SerializeField] 
        private Sprite cannon;
        [SerializeField] 
        private Sprite transball;

        [SerializeField]
        private ItemInventory inventory;

        private PhotonView pv;
        void Start()
        {
            cointCnt = 0;
            pv = transform.root.GetComponent<PhotonView>();
            //int characterIndex = transform.root.GetComponent<ItemCharacterSwitcher>().currentCharacterIndex;
            //pv = transform.root.GetChild(characterIndex).GetComponent<PhotonView>();
            playerName.text = pv.Owner.NickName;
            if (pv.IsMine)
            {
                Color color;
                ColorUtility.TryParseHtmlString("#FF9500", out color);//주황
                playerName.color = color;
            }
            else
            {
                Color color;
                ColorUtility.TryParseHtmlString("#059100", out color);//초록
                playerName.color = color;
            }

            //coinCnt 찾기
            if (GameObject.Find("coinCnt") != null)
            {
                coinText = GameObject.Find("coinCnt").GetComponent<TMP_Text>();
                coinText.text = PlayerPrefs.GetInt("Coin") + "";
            }


            //item출력위치 찾기
            if(GameObject.Find("ItemImage") != null) {
                itemUI = GameObject.Find("ItemImage");
                itemUI.SetActive(false);
                audio = itemUI.GetComponent<AudioSource>();
            }

            inventory = GetComponentInParent<ItemInventory>();
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Coin")
            {
                Destroy(other.gameObject, 0.001f);
                if (pv.IsMine)
                {
                    cointCnt += 1;
                    int coin = PlayerPrefs.GetInt("Coin");
                    coin++;
                    PlayerPrefs.SetInt("Coin", coin);
                    coinText.text = PlayerPrefs.GetInt("Coin") + "";

                    if(cointCnt%10 == 0)
                    {
                        getItem();
                    }
                }
            }
            if (other.gameObject.tag == "GetItem")
            {
                if (pv.IsMine)
                {
                    if (other.gameObject.name.Equals("ItemArch") && !itemCheck1)
                    {
                        //1번째 아이템 획득
                        itemCheck1 = true;
                    }
                    else if (other.gameObject.name.Equals("ItemArch (2)") && !itemCheck2)
                    {
                        //2번째 아이템 획득
                        itemCheck2 = true;
                    }
                    else if (other.gameObject.name.Equals("ItemArch (3)") && !itemCheck3)
                    {
                        //3번째 아이템 획득
                        itemCheck3 = true;
                    }
                    else
                    {
                        //return;
                    }
                    itemUI.SetActive(true); // 아이템 이미지 ui표시
                    audio.Play();

                    getItem();
                }
            }


            if (other.gameObject.tag == "GetItemSquare")
            {
                itemUI.SetActive(true);
                Image image = itemUI.GetComponent<Image>();

                image.sprite = speed;
                itemName = "speed";
                inventory.currentItem = itemName;
            }
        }

        private void getItem()
        {
            itemUI.SetActive(true);
            Image image = itemUI.GetComponent<Image>();

            int random = randomNum();

            if (random > 80)
            {
                image.sprite = speed;
                itemName = "speed";
                inventory.currentItem = itemName;
            }
            else if (random > 60)
            {
                image.sprite = jump;
                itemName = "jump";
                inventory.currentItem = itemName;
            }
            else if (random > 40)
            {
                image.sprite = tree;
                itemName = "tree";
                inventory.currentItem = itemName;
            }
            else if (random > 20)
            {
                image.sprite = cannon;
                itemName = "cannon";
                inventory.currentItem = itemName;
            }
            else
            {
                image.sprite = transball;
                itemName = "transball";
                inventory.currentItem = itemName;
            }
        }

        private int randomNum()
        {
            System.Random random = new System.Random();
            int value = (int)(random.NextDouble() * 100);
            return value;
        }

        public void SetWinner()
        {
            if (pv.IsMine)
            {
                Hashtable property = new Hashtable(){{"isWin",true}};
                PhotonNetwork.LocalPlayer.SetCustomProperties(property);
                PlayerPrefs.SetString("Win", "True");
            }
        }


    }
}