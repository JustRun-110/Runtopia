using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class WolfAgent : Agent
{
    [SerializeField] private float walkSpeed;  // 걷기 속력

    //private bool isAction = false;  // 행동 중인지 아닌지 판별
    private bool isWalking = false; // 걷는지, 안 걷는지 판별
    private bool isRunning;
    [SerializeField] public bool isPlaying=false;

    [SerializeField] private float walkTime;  // 걷기 시간
    [SerializeField] private float runTime; 
    private float currentTime;


    [SerializeField] private Animator anim;
    [SerializeField] private Rigidbody rigidl;

    // private void Awake()
    // {
    //     isAction = true;
    // }

    private void Start()
    {
        rigidl = gameObject.GetComponent<Rigidbody>();
        anim = gameObject.GetComponent<Animator>();
        //Debug.Log(anim);
    }

    private void Move()
    {
        if (isWalking)
            rigidl.MovePosition(transform.position + transform.forward * walkSpeed * Time.deltaTime);
        else if (isRunning)
            rigidl.MovePosition(transform.position + transform.forward * walkSpeed * 2 * Time.deltaTime);

    } 

    void Update()
    {
        Move();
        RandomAction();
        
    }

    private void RandomAction()
    {
        if (!isPlaying)
        {
            isPlaying = true;

            int _random = Random.Range(0, 5);

        if (_random == 0)
            Wait();
        else if (_random == 1)
            Sniff();
        else if (_random == 2)
            Bite();
        else if (_random == 3)
            Walk();
        else if (_random == 4)
            Run();

        //Debug.Log(_random);
        }

    }

    private void Wait()  // 대기
    {
        isPlaying = false;
        isRunning = false;
        isWalking = false;
    }

    private void Run()  // 달리기
    {
        isRunning = true;
        anim.SetBool("Running", true);
        isWalking = false;
        //Debug.Log("Running");
    }

    private void Sniff()  // 냄새맡기
    {
        isRunning = false;
        isWalking = false;
        anim.SetBool("Sniff", true);
        //Debug.Log("Sniff");
    }

    private void Bite()  // 물어뜯기
    {
        isRunning = false;
        isWalking = false;
        anim.SetBool("Bite", true);
        //Debug.Log("Bite");
    }

    private void Walk()  // 걷기
    {
        isWalking = true;
        isRunning = false;
        anim.SetBool("Walking", true);
        //Debug.Log("Walking");
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Fence" || col.gameObject.tag == "Wolf")
        {
            int _randomDir = Random.Range(90, 271);
            // transform.Rotate(0, 180, 0);
            transform.Rotate(0, _randomDir, 0);
            //Destroy(gameObject, .1f);
        }
    }

}
