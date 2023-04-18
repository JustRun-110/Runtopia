using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bgmChange : MonoBehaviour
{

    AudioSource baby;
    AudioSource happy;
    AudioSource rock;


    private void Start() {
        if (GameObject.Find("Baby_BGM") != null)
        {
            baby = GameObject.Find("Baby_BGM").GetComponent<AudioSource>();
            happy = GameObject.Find("Happy_BGM").GetComponent<AudioSource>();
            rock = GameObject.Find("Rock_BGM").GetComponent<AudioSource>();
        }
    }

    private void OnCollisionEnter(Collision other) {
        if(other.gameObject.name == "Baby_BGM") {
            if(!baby.isPlaying) {
                baby.Play();
                happy.Stop();
                rock.Stop();
            } 
        }
        else if(other.gameObject.name == "Happy_BGM") {
            if(!happy.isPlaying) {
                baby.Stop();
                happy.Play();
                rock.Stop();
            } 
        }
        else if(other.gameObject.name == "Rock_BGM") {
            if(!rock.isPlaying) {
                baby.Stop();
                happy.Stop();
                rock.Play();
            } 
        }
    }
}
