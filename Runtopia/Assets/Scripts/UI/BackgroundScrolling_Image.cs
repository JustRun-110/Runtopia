using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace garden
{

    public class BackgroundScrolling_Image : MonoBehaviour
    {
        private Material material;

        public float speed;
        private float offset;

        private void Start()
        {
            material = GetComponent<Image>().material;
        }

        private void Update()
        {
            offset += Time.deltaTime * speed ;
            material.mainTextureOffset = new Vector2(offset, 0);
        }
    }
}


