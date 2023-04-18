using UnityEngine;
using TMPro;

namespace sjb
{
    public class GameTimer : MonoBehaviour
    {
        [Tooltip("타이머표시 텍스트")]
        public GameObject timerUI;
        [Tooltip("아이템받는 곳")]
        public GameObject itemObj1;
        [Tooltip("지는 ui")]
        public GameObject loseUI;
        [Tooltip("게임종료 UI")]
        public GameObject endUI;


        private GameItem itemCheck1;
        private TextMeshProUGUI text;
        private float time;
        private int min, sec;
        private string minString, secString;

        private void Start()
        {
            itemCheck1 = itemObj1.GetComponent<GameItem>();
            text = timerUI.GetComponent<TextMeshProUGUI>();
            loseUI.SetActive(false);
            time = 600;
            min = 10;
            sec = 0;
            text.text = min + ":" + sec;
        }

        private void Update()
        {
            time -= Time.deltaTime;

            min = (int)time / 60;
            sec = (int)(time - (60 * min)) % 60;

            if (time > 40 && time < 41) itemCheck1.itemCheck = false;

            if (min < 10) minString = "0" + min;
            if (sec < 10) secString = "0" + sec;
            else secString = sec + "";

            if (time > 0)
                text.text = minString + ":" + secString;
            else
            {
                if (!loseUI.activeSelf)
                {
                    loseUI.SetActive(true);
                    endUI.GetComponent<GameEnd>().GameSet();
                }
            }
        }
    }
}
