using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

namespace sjb
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public GameObject menuPanel;
        private GameObject playerPrefab; //로컬 플레이어
        private List<GameObject> players; //접속 중인 플레이어 리스트

        //private PhotonView pv;

        // 스폰 위치
        Vector3 spawnPoint;
        Quaternion spawnRotation;

        private void Awake()
        {
            //DontDestroyOnLoad(gameObject);
            PlayerPrefs.SetInt("Coin", 0);
            PlayerPrefs.SetInt("Dead", 0);
            PlayerPrefs.SetString("Win", "False");
            PlayerPrefs.SetString("Time", "");

            GameManager[] gms = FindObjectsOfType<GameManager>();
            if (playerPrefab == null && gms.Length == 1)
            {
                CreatePlayer();
                //pv = playerPrefab.GetComponent<PhotonView>();
            }
        }

        public void CreatePlayer()
        {
            // 출현 위치 정보를 배열에 저장
            //게임 오브젝트의 이름은 프로젝트에 맞게 수정
            //Transform[] points = GameObject.Find("SpawnGroups").GetComponentsInChildren<Transform>();
            Transform[] points = GameObject.Find("InstantiateGroups").GetComponentsInChildren<Transform>();
            int idx = Random.Range(1, points.Length);

            // 네트워크상에 캐릭터 생성, 캐릭터 선택
            object characterNum;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("character", out characterNum))
            {
                switch ((int)characterNum)
                {
                    case 0:
                        playerPrefab = PhotonNetwork.Instantiate("panda", points[idx].position, points[idx].rotation, 0);
                        break;
                    case 1:
                        playerPrefab = PhotonNetwork.Instantiate("cat", points[idx].position, points[idx].rotation, 0);
                        break;
                    case 2:
                        playerPrefab = PhotonNetwork.Instantiate("dog", points[idx].position, points[idx].rotation, 0);
                        break;
                    case 3:
                        playerPrefab = PhotonNetwork.Instantiate("deer", points[idx].position, points[idx].rotation, 0);
                        break;
                    case 4:
                        playerPrefab = PhotonNetwork.Instantiate("duck", points[idx].position, points[idx].rotation, 0);
                        break;
                    case 5:
                        playerPrefab = PhotonNetwork.Instantiate("fox", points[idx].position, points[idx].rotation, 0);
                        break;
                    case 6:
                        playerPrefab = PhotonNetwork.Instantiate("raccoon", points[idx].position, points[idx].rotation, 0);
                        break;
                    case 7:
                        playerPrefab = PhotonNetwork.Instantiate("tiger", points[idx].position, points[idx].rotation, 0);
                        break;
                    case 8:
                        playerPrefab = PhotonNetwork.Instantiate("wolf", points[idx].position, points[idx].rotation, 0);
                        break;
                    case 9:
                        playerPrefab = PhotonNetwork.Instantiate("observer", points[idx].position, points[idx].rotation, 0);
                        break;
                }

                // 캐릭터 위치 초기화를 위해 스폰 위치 저장

                spawnPoint = points[idx].position;
                spawnRotation = points[idx].rotation;

            }

        }


        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (menuPanel.activeSelf)
                {
                    CloseMenu();
                }
                else
                {
                    OpenMenu();
                }
            }
        }
        public void OpenMenu()
        {
            menuPanel.SetActive(true);
        }

        public void CloseMenu() {
            menuPanel.SetActive(false);
        }

        public void GameQuit()
        {
            Application.Quit();
        }

        public void GoLobby()
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
