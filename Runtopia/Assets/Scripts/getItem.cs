using UnityEngine;
using UnityEngine.UI;

public class getItem : MonoBehaviour
{

    private GameObject item;
    private new AudioSource audio;
    [SerializeField] public bool itemCheck;
    [SerializeField] private Sprite hourGlass;
    [SerializeField] private Sprite bomb;
    [SerializeField] private Sprite arrowsRandom;
    [SerializeField] private Sprite colorfulCandy;
    [SerializeField] private Sprite apple;

    private void Start() {
        item = GameObject.Find("item");
        item.SetActive(false);
        audio = item.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.tag == "Player" && !itemCheck)
        {
            itemCheck = true;
            item.SetActive(true);
            audio.Play();
            Image image = item.GetComponent<Image>();

            int random = randomNum();

            if(random > 40) image.sprite = apple;
            else if(random > 20) image.sprite = colorfulCandy;
            else if(random > 15) image.sprite = arrowsRandom;
            else if(random > 10) image.sprite = bomb;
            else image.sprite = hourGlass;
        }
    }

    private int randomNum() {
        System.Random random = new System.Random();
        int value = (int)(random.NextDouble()*100);
        return value;
    }
}
