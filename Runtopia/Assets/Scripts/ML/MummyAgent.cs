using System.Collections;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;

public class MummyAgent : Agent
{
    private Transform tr;
    private Rigidbody rb;
    public Transform targetTr;
    public Renderer floorRd;        

    // 바닥의 색상을 변경하기 위한 머터리얼
    private Material originMt;      
    public Material badMt;          
    public Material goodMt;         

    //초기화 작업을 위해 한번 호출되는 메소드
    public override void Initialize()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        originMt = floorRd.material; 

        MaxStep = 1000;
    }
    //에피소드(학습단위)가 시작할때마다 호출
    public override void OnEpisodeBegin()
    {
        //물리력을 초기화
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        //agent의 위치를 불규칙하게 변경
        tr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f)
                                     , 0.05f
                                     , Random.Range(-4.0f, 4.0f));
        targetTr.localPosition = new Vector3(Random.Range(-4.0f, 4.0f)
                                            , 0.55f
                                            , Random.Range(-4.0f, 4.0f));
        StartCoroutine(RevertMaterial());   
    }
    IEnumerator RevertMaterial()
    {
        yield return new WaitForSeconds(0.2f);
        floorRd.material = originMt;
    }
    //환경 정보를 관측 및 수집해 정책 결정을 위해 브레인에 전달하는 메소드
    public override void CollectObservations(Unity.MLAgents.Sensors.VectorSensor sensor)
    {
        sensor.AddObservation(targetTr.localPosition);  //3 (x,y,z)
        sensor.AddObservation(tr.localPosition);        //3 (x,y,z)
        sensor.AddObservation(rb.velocity.x);           //1 (x)
        sensor.AddObservation(rb.velocity.z);           //1 (z)
    }
    //브레인 으로 부터 전달받은 액션(행위)를 실행하는 메소드
    public override void OnActionReceived(ActionBuffers actions)
    {
        // 데이터를 정규화
        float h = Mathf.Clamp(actions.ContinuousActions[0], -1.0f, 1.0f);
        float v = Mathf.Clamp(actions.ContinuousActions[1], -1.0f, 1.0f);
        Vector3 dir = (Vector3.forward * v) + (Vector3.right * h);
        rb.AddForce(dir.normalized * 40.0f);

        // 지속적으로 이동을 이끌어내기 위한 마이너스 보상
        SetReward(-0.001f);
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        Debug.Log($"[0]={continuousActionsOut[0]} [1]={continuousActionsOut[1]}");

    }
    void OnCollisionEnter(Collision coll)
    {
        if (coll.collider.CompareTag("DEAD_ZONE"))
        {
            floorRd.material = badMt;       

            // 잘못된 행동일 때 마이너스 보상을 준다.
            SetReward(-1.0f);
            EndEpisode();       
        }

        if (coll.collider.CompareTag("GOOD_ITEM"))
        {
            floorRd.material = goodMt;     

            //올바른 행동일 떄 플로스 보상을 준다.
            SetReward(+1.0f);
            EndEpisode();
        }
    }
}
