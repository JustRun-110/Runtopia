using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace sjb
{
    public class CoinFX : MonoBehaviour
    {

        [Tooltip("코인 음성가진 오브젝트")]
        public GameObject coin;
        private new AudioSource audio;
        private bool check;

        private void Start()
        {
            audio = coin.GetComponent<AudioSource>();
            check = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            check = true;
            if (other.gameObject.tag == "Player" && check)
            {
                check = false;
                audio.Play();
            }
        }
    }
}