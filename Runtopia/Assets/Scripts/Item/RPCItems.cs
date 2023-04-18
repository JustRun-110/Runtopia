//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Photon.Pun;

//namespace garden
//{
//    public class RPCItems : MonoBehaviour
//    {
//        [Header("Photon")]
//        [SerializeField]
//        private PhotonView pv;

//        [Header("Is Character State Item Used?")]
//        public bool useItem;

//        [Header("Character Switch")]
//        public ItemCharacterSwitcher item;

//        private void Start()
//        {
//            pv = GetComponent<PhotonView>();
//            item = GetComponentInParent<ItemCharacterSwitcher>();
//        }

//        void Update()
//        {
//            if (useItem)
//            {
//                pv.RPC("Switch", RpcTarget.All);
//                useItem = false;
//            }
//        }

//        [PunRPC]
//        public void Switch()
//        {
//            Debug.Log("Switch");
//            item.Switch();
//        }
//    }

//}
