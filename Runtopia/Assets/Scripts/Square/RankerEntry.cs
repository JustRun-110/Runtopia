using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace sjb
{
    public class RankerEntry : MonoBehaviour
    {
        // Start is called before the first frame update
        [Header("UI References")]
        public TMP_Text rank;
        public TMP_Text name;
        public TMP_Text mmr;


        public void Initialize(string rankInput, string nameInput, int mmrInput)
        {
            rank.text = rankInput;
            name.text = nameInput;
            mmr.text = mmrInput+"";
        }
    }
}