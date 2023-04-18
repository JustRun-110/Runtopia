using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sjb {
    public class RankerManager : MonoBehaviour
    {


        [Tooltip("랭커 패널")]
        private GameObject rankerPanel;

        [Tooltip("랭킹 스크롤")]
        public Transform RankingScroll;
        [Tooltip("랭킹 entry")]
        public GameObject RankerEntry;
        public void Start()
        {
            rankerPanel = GameObject.Find("Ranking Panel");
            rankerPanel.SetActive(false);
        }

        public void OpenRanker()
        {
            Debug.Log("랭킹 열음");
            rankerPanel.SetActive(true);
            CleanRanker();
            StartCoroutine(Request.Instance.ApiGetRequest("/api/v1/ranking", "ranking"));
        }
        public void CloseRanker()
        {
            CleanRanker();
            rankerPanel.SetActive(false);
        }
        public void AddRanker(int rank, string name, int mmr)
        {
            GameObject entry = Instantiate(RankerEntry, RankingScroll);
            entry.GetComponent<RankerEntry>().Initialize(rank+"등", name, mmr);
        }
        public void CleanRanker()
        {
            Debug.Log("랭킹 삭제중");

            Transform[] childList = RankingScroll.GetComponentsInChildren<Transform>();
            for (int i = 1; i < childList.Length; i++)
            {
                Destroy(childList[i].gameObject);
            }
        }
    }
}