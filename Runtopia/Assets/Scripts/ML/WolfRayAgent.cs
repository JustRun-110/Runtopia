using System.Collections;
using RootMotion.Demos;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using UnityEngine;
using garden;

public class WolfRayAgent : Agent
{
    private new Transform transform;
    private new Rigidbody rigidbody;
    private WolfStageManager wolfStageManager;

    public float moveSpeed = 1.0f;
    public float turnSpeed = 150.0f;
    //private int cnt = 0;
    
    [SerializeField] private Animator anim;

    public override void Initialize()
    {
        anim.SetBool("Running", true);
        // 한 에피소드(학습단위) 당 시도 횟수
        MaxStep = 0;

        transform = GetComponent<Transform>();
        rigidbody = GetComponent<Rigidbody>();
        wolfStageManager = transform.parent.GetComponent<WolfStageManager>();

    }
    public override void OnEpisodeBegin()
    {
        //cnt = 0;
        // 스테이지 초기화
        //wolfStageManager.SetStageObject();
        
        // 물리엔진 초기화
        rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
        
        // 에이전트의 위치를 변경
        transform.localPosition = new Vector3(Random.Range(-22.0f, 22.0f), 2f, Random.Range(-22.0f, 22.0f));
        
        //에이전트의 회전값 변경
        transform.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0, 360));
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var action = actionsOut.DiscreteActions;    //이산(-1.0, 0.0, +1.0

        actionsOut.Clear();
        
        // Branch 0 - 이동로직에 쓸 키 맵핑
        //Branch 0의 Size : 3
        // 정지   /  전진   / 후진
        // Non   /   W    /   S
        //  0   /   1   /   2
        
        if (UnityEngine.Input.GetKey(KeyCode.W))
        {
            action[0] = 1;
        }
        // if (UnityEngine.Input.GetKey(KeyCode.S))
        // {
        //     action[0] = 2;
        // }
        // Branch 1 - 회전로직에 쓸 키 맵핑
        //Branch 1의 Size : 3
        // 정지   /  전진   / 후진
        // Non   /   A    /   D
        //  0   /   1   /   2
        if (UnityEngine.Input.GetKey(KeyCode.A))
        {
            action[1] = 1;
        }if (UnityEngine.Input.GetKey(KeyCode.D))
        {
            action[1] = 2;
        }
    }

    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("Player"))
        {
            // 가속도가 붙을 수 있기 때문에 물리력 초기화
            rigidbody.velocity = rigidbody.angularVelocity = Vector3.zero;
            // Destroy(coll.gameObject);
            AddReward(1.0f);
            //cnt++;
            // if (cnt == 30)
            // {
            //     EndEpisode();
            // }

            coll.gameObject.transform.root.GetChild(2).GetComponent<CharacterPuppet>().DoRespawn();
        }
        
        if (coll.collider.CompareTag("WolfDoor"))
        {
            AddReward(-1.0f);
            //EndEpisode();
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        var action = actions.DiscreteActions;

        Vector3 dir = Vector3.zero;
        Vector3 rot = Vector3.zero;

        switch (action[0])
        {
            case 1: dir = transform.forward; break;
            // case 2: dir = -transform.forward; break;
        }
        switch (action[1])
        {
            case 1: rot = -transform.up; break;
            case 2: rot = transform.up; break;
        }
        
        transform.Rotate(rot, Time.fixedDeltaTime * turnSpeed);
        rigidbody.AddForce(dir * moveSpeed, ForceMode.VelocityChange);
        
        
        //마이너스 패널티를 적용
        // AddReward(-1.0f / (float)MaxStep);
    }

}
