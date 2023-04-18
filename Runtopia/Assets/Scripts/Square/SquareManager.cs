using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using sjb;
using JetBrains.Annotations;
using TMPro;

public class SquareManager : MonoBehaviourPunCallbacks
{
    [Tooltip("튜툐리얼")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;

    private int tutoPage = 0;

    private GameObject playerPrefab; //로컬 플레이어
    private List<GameObject> players; //접속 중인 플레이어 리스트

    private PhotonView pv;

    [Tooltip("로딩 패널")]
    private GameObject loadingPanel;

    List<string> scripts = new List<string>()
        { "런토피아에 어서와!\r\n여기는 모두가 모이는 광장이야!",
            "여기에서 간단하게 \r\n 조작법을 연습해보자!",
            "기본적인 이동은 wasd로 \r\n 시점은 마우스로 조작할수있어",
            "이제 앉아볼까? \r\n c를 눌르면 앉을 수 있어",
            "이제 점프를 해보자 \r\n space바를 누르면 점프가 돼",
            "이젠 고급기술을 해보자 \r\n 벽에서 space바를 연타해봐",
            "이 기술을 통해 \r\n 벽을 탈수가 있어",
            "장애물에서 e키를 눌러봐 \r\n 파쿠르 모션을 통해 장애물을 넘을 수 있어",
            "이제 특수기술을 써보자 \r\n 광장의 무지개를 지나쳐봐",
            "좌측 상단의 파란 박스에 \r\n 아이템이 들어갈거야",
            "특수 아이템은 \r\n Q를 통해 사용이 가능해",
            "좌클릭으로 \r\n 다른 유저도 잡는게 가능해",
            "마지막으로 이상한곳에 갖히면 \r\n G키를 눌러서 다시 리스폰하자!",
            "좀 더 자세히 설명이 보고 싶다면 \r\n 조작법을 열어봐!",
            "이제 게임을 하러 가고싶으면 \r\n 우측 상단의 게임하러가기로 가자고!"
        };

    // 스폰 위치
    Vector3 spawnPoint;
    Quaternion spawnRotation;

    private void Awake()
    {
        loadingPanel = GameObject.Find("Loading Panel");
        tutorialPanel.SetActive(false);
        if (playerPrefab == null)
        {
            CreatePlayer();
            pv = playerPrefab.GetComponent<PhotonView>();
        }
    }

    private void Start()
    {
        loadingPanel.SetActive(false);
        if (PlayerPrefs.GetString("haveTuto","False").Equals("False"))
        {
            Debug.Log(tutorialPanel);
            tutorialPanel.SetActive(true);
            StartCoroutine(NextTuto(4f));
        }
    }


    void CreatePlayer()
    {
        // 출현 위치 정보를 배열에 저장
        //게임 오브젝트의 이름은 프로젝트에 맞게 수정
        //Transform[] points = GameObject.Find("SpawnGroups").GetComponentsInChildren<Transform>();
        Transform[] points = GameObject.Find("InstantiateGroups").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);

        // 네트워크상에 캐릭터 생성, 캐릭터 선택
        if (PlayerPrefs.GetInt("USER_Gender") == 1)
        {
            playerPrefab = PhotonNetwork.Instantiate("dog", points[idx].position, points[idx].rotation, 0);
            //playerPrefab.GetComponent<garden.ItemCharacterSwitcher>().useItem = true;
        }
        else
        {
            playerPrefab = PhotonNetwork.Instantiate("cat", points[idx].position, points[idx].rotation, 0);
            //playerPrefab.GetComponent<garden.ItemCharacterSwitcher>().useItem = true;
        }

        // 캐릭터 위치 초기화를 위해 스폰 위치 저장
        spawnPoint = points[idx].position;
        spawnRotation = points[idx].rotation;

    }

    public void OnMoveLobby()
    {
        loadingPanel.SetActive(true);
        PhotonNetwork.LeaveRoom();//룸에서 나가기
    }
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    IEnumerator NextTuto(float delayTime)
    {
        tutorialText.text = scripts[tutoPage];
        tutoPage++;
        yield return new WaitForSeconds(delayTime);
        if (tutoPage < scripts.Count)
        {
            StartCoroutine(NextTuto(4f));
        }
        else
        {
            tutorialPanel.SetActive(false);
            PlayerPrefs.SetString("haveTuto", "True");
        }
    }

    public void StopTuto()
    {
        tutoPage = 100;
        tutorialPanel.SetActive(false);
        PlayerPrefs.SetString("haveTuto", "True");
    }

}
