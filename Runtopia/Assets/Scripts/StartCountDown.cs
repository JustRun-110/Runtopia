using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCountDown : MonoBehaviour
{
    [SerializeField]private float Timer = 0.0f;
    public bool onGame;

    public GameObject Num_A;   //1번
    public GameObject Num_B;   //2번
    public GameObject Num_C;   //3번
    public GameObject Num_GO;
    public Camera cam;
    public GameObject Obj_Stop;



    void Start () {

        //시작시 카운트 다운 초기화
        Timer = 0.0f;
        onGame = false;
        // cam = GetComponent<Camera>();


        Num_A.SetActive(false);
        Num_B.SetActive(false);
        Num_C.SetActive(false);
        Num_GO.SetActive(false);
    }



    void Update () {
       


    // 카메라 앵글을 캐릭터 뒤통수 윗 부분 고정. 

        //게임 시작시 정지
        if(Timer == 0.0f){
            // Time.timeScale = 0.0f;
        }


        if(Timer < 3.4f){
            Timer += Time.deltaTime;

            // 3번켜기
            if(Timer <= 1.3f) {
                Num_C.SetActive(true);
            }

            // 3번끄고 2번켜기
            if(Timer > 1.3f && Timer <= 2.4f) {
                Num_C.SetActive(false);
                Num_B.SetActive(true);
            }

            // 2번끄고 1번켜기
            if(Timer > 2.4f && Timer < 3.4f) {
                Num_B.SetActive(false);
                Num_A.SetActive(true);
            }

            // 1번끄고 GO 켜기 LoadingEnd () 코루틴호출
            if(Timer >= 3.4f) {
                Num_A.SetActive(false);
                Num_GO.SetActive(true);
                Obj_Stop.SetActive(false);
                StartCoroutine(this.LoadingEnd());
                // Time.timeScale = 1.0f; //게임시작
            }
        }
    }



    IEnumerator LoadingEnd(){
        yield return new WaitForSeconds(1.0f);
        Num_GO.SetActive(false);
    }


}
