using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] GameObject timerUI;
    [SerializeField] GameObject itemObj1;
    private getItem itemCheck1;
    private GameObject loseUI;
    private TextMeshProUGUI text;
    private TextMeshProUGUI lose;
    private float time;
    private int min, sec;
    private string minString, secString;

    private void Start() {
        timerUI = GameObject.Find("timer");
        loseUI = GameObject.Find("Lose");
        itemObj1 = GameObject.Find("itemObj1");
        itemCheck1 = itemObj1.GetComponent<getItem>();
        text = timerUI.GetComponent<TextMeshProUGUI>();
        loseUI.SetActive(false);
        time = 180;
        min = 10;
        sec = 0;
        text.text = min+":"+sec;
    }

    private void Update() {
        time -= Time.deltaTime;

        min = (int)time/60;
        sec = (int)(time - (60*min))%60;

        if(time > 40 && time <41) itemCheck1.itemCheck = false;

        if(min < 10) minString = "0"+min;
        if(sec < 10) secString = "0"+sec;
        else secString  = sec+"";

        if(time>0)
            text.text = minString+":"+secString;
        else {
            loseUI.SetActive(true);
            // 광장으로 내보내는 로직이 작성되어야 할 곳
        }
    }

}
