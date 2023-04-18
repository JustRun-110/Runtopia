using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;


namespace sjb
{
    public class Player : MonoBehaviour
    {
        public int score = 0; // 플레이어의 점수

        public string sceneName;
        PhotonView pv;
        GameManager gm;


        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            gm = FindObjectOfType<GameManager>();
        }


        private void Start()
        {
            // 내 캐릭터는 씬 이동시에 삭제되지 않고 데이터를 유지함
            DontDestroyOnLoad(gameObject);
        }



        void Update()
        {
            if (pv.IsMine)
            {
                // 현재 씬의 이름을 항상 변수로 저장
                sceneName = SceneManager.GetActiveScene().name;
            }
        }

        // 접속중인 현재 씬을 다른 클라이언트에게 전달
        [PunRPC]
        void CheckMyScene()
        {
            pv.RPC("SendMyScene", target: RpcTarget.All, GetComponent<PhotonView>().ViewID, SceneManager.GetActiveScene().name);

        }

        // RPC 로 다른 플레이어의 GameManager의 SearchPlayer()를 호출하여 각자 플레이어 리스트의 갱신을 유도한다.
        // 처음 접속했을 때 뿐만 아니라 포탈 이동 등 다양한 씬 이동의 로직에 포함하도록 한다.
        [PunRPC]
        void AddPlayer()
        {
            //gm.SearchPlayer();
        }

    }
}