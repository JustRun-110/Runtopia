using RootMotion.Demos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterItemEffect : MonoBehaviour
{
    [Header("Is Character State Item Used?")]
    public bool speedItemUsed;
    public bool jumpItemUsed;
    public bool transballItemUsed;
    public bool useFirstTransball = true;
    public bool useSecondTransball = false;

    [Header("Jump")]
    [SerializeField]
    private float jumpPower = 1f;
    [SerializeField]
    private float jumpPowerUp = 5f;

    [Header("Speed")]
    [SerializeField]
    private float speedPower = 1f;
    [SerializeField]
    private float speedPowerUp = 5f;

    [Header("transball")]
    [SerializeField]
    private GameObject obj_transball;
    private bool isAllTransballUsed = false;
    private bool isFirstTransballUsed = false;
    private bool isSecondTransballUsed = false;
    private GameObject obj_transballStart = null;
    private GameObject obj_transballEnd = null;
    private Vector3 transBallStart = Vector3.zero;
    private Vector3 transBallEnd = Vector3.zero;
    private int transballCount = 0;

    [Header("Settings")]
    [SerializeField]
    private CharacterThirdPerson character;

    private void Start()
    {
        jumpPower = character.jumpPower;
        speedPower = character.speedItemMultiplier;
    }

    private void FixedUpdate()
    {
        StatusItem();
        TransballItem();
    }

    private void TransballItem()
    {
        // 트렌스볼 아이템을 사용했고, 아직 모든 트렌스볼 아이템을 사용하기 전이라면
        if (transballItemUsed)
        {
            if (useFirstTransball)
            {
                Debug.Log("첫번째 트렌스볼 생성");

                obj_transballStart = Instantiate(obj_transball, this.transform);
                useFirstTransball = false;
                isFirstTransballUsed = true;
            }
            else if(useSecondTransball)
            {
                Debug.Log("두번째 트렌스볼 생성");
                obj_transballEnd = Instantiate(obj_transball, this.transform);
                useSecondTransball = false;
                isSecondTransballUsed = true;
            }
        }

        if (Input.GetKey(KeyCode.Mouse0))
        {
            if (isFirstTransballUsed)
            {
                Debug.Log("첫번째 트렌스볼을 설치합니다");
                PlaceTransball(obj_transballStart);

                isFirstTransballUsed = false;
            }
            else if (isSecondTransballUsed)
            {
                Debug.Log("두번째 트렌스볼을 설치합니다");
                PlaceTransball(obj_transballEnd);

                isSecondTransballUsed = false;
            }
        }
    }

    private void PlaceTransball(GameObject transball)
    {
        Debug.Log("설지할 트랜스볼 : " + transball.name);

        transball.transform.SetParent(null); // 플레이어로 부터 분리하기

        if (transBallStart == Vector3.zero)
        {
            Debug.Log("첫번째 트랜스볼 위치 저장");
            Vector3 position = this.transform.position;

            Debug.Log("첫번째 트랜스 볼의 위치 : "+ position);

            transBallStart = position;
        }
        else if(transBallEnd == Vector3.zero)
        {
            Debug.Log("두번째 트랜스볼 위치 저장");
            Vector3 position = this.transform.position;
            Debug.Log("첫번째 트랜스 볼의 위치 : " + position);
            
            transBallEnd = position;

            Debug.Log("트랜스볼들의 위치를 지정합니다");
            obj_transballStart.gameObject.GetComponent<Transball>().setPosition(transBallEnd);
            obj_transballEnd.gameObject.GetComponent<Transball>().setPosition(transBallStart);
        }

        transballCount++;

        if (transballCount == 2) // 초기화
        {
            //키 입력을 통해 조작되는 불리언 값 초기화
            transballItemUsed = false;
            useFirstTransball = true;
            useSecondTransball = false;

            //코드를 통해 조작되는 불리언 값 초기화
            isFirstTransballUsed = false;
            isSecondTransballUsed = false;

            transBallStart = Vector3.zero;
            transBallEnd = Vector3.zero;
        }
    }

    private void StatusItem()
    {
        if (speedItemUsed)
        {
            SetSpeed(speedPowerUp);
        }
        else
        {
            SetSpeed(speedPower);
        }

        if (jumpItemUsed)
        {
            SetJumpPower(jumpPowerUp);
        }
        else
        {
            SetJumpPower(jumpPower);
        }
    }

    private void SetSpeed(float speed)
    {
        if (!character) return;
        character.speedItemMultiplier = speed;

    }

    private void SetJumpPower(float power)
    {
        if (!character) return;
        character.jumpPower = power;
    }
}
