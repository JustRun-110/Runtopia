using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using TMPro;
using UnityEngine.UIElements;

namespace sjb
{
    class Ranking
    {
        public bool getWin;
        public int userCoin;
        public int userDrop;
        public string userId;
        public string userPlayTime;
    }

    public class GameEnd : MonoBehaviour
    {
        [Tooltip("게임종료 판넬")]
        public GameObject endPanel;

        [Tooltip("타이머 text")]
        public TMP_Text timeText;

        bool isEnd = false;
        private int initTime = 600;//전체 시간
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag == "Player"&& other.gameObject.name== "Character Controller" && !isEnd)
            {
                isEnd = true;
                endPanel.SetActive(true);
                other.gameObject.GetComponent<PlayerInfo>().SetWinner();

                GameSet();
            }
        }

        public void GameSet()
        {
            int time = String2Sec(timeText.text);
            time = initTime - time;
            String PlayTime = Sec2String(time);
            PlayerPrefs.SetString("Time", PlayTime);
            //입장중인 모든 인원에게 ui를 띄운다.
            Invoke("EndEvent", 5.0f);
            //승리자로 지정
            SendRanking();
        }

        public void EndEvent()
        {
            //다같이 씬 이동
            PhotonNetwork.LoadLevel("Game_End");
        }

        public void SendRanking()
        {
            Ranking ranking = new Ranking();
            ranking.userId = PlayerPrefs.GetString("USER_ID");
            ranking.userCoin = PlayerPrefs.GetInt("Coin");
            ranking.userDrop = PlayerPrefs.GetInt("Dead");
            ranking.getWin = PlayerPrefs.GetString("Win").Equals("True") ? true : false;
            ranking.userPlayTime = PlayerPrefs.GetString("Time"); //01:30:30 / h:m:s
            string json = JsonUtility.ToJson(ranking);
            StartCoroutine(Request.Instance.ApiPostRequest("/api/v1/ranking", json, "ranking"));
        }

        public int String2Sec(String time)
        {
            //string형태의 시간을 int로 
            int res = 0;
            String[] times = time.Split(":");
            res = int.Parse(times[0])*60;
            res += int.Parse(times[1]);

            return res;
        }

        public String Sec2String(int time)
        {
            //int를 string으로
            int m = time / 60;
            int s = time % 60;

            return $"00:{m:00}:{s:00}";
        }
    }

}