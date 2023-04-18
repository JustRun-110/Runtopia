using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class getCoin : MonoBehaviour
{
   private int coin;
   [SerializeField] GameObject coinTextObject; 
   private TextMeshProUGUI text;

   private void Start() {
    coin = 0;
    coinTextObject = GameObject.Find("coinCnt");
    text = coinTextObject.GetComponent<TextMeshProUGUI>();
    text.text = coin + "";
   }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.tag == "Coin") {
            coin ++;
            Destroy(other.gameObject, 0.001f);
            Debug.Log(coin);
            
            text.text = coin + "";

            
        }
    }

}
