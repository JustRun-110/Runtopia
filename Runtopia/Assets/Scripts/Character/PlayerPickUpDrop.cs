using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;
using Photon.Pun;
using Photon.Realtime;

namespace garden
{

    public class PlayerPickUpDrop : MonoBehaviourPunCallbacks
    {
        [Header("Grab")]
        [SerializeField]
        private Transform objectGrabPointTransform;//잡기 시도할 위치
        [SerializeField]
        private Transform objectGrabSocketTransform;//잡은 오브젝트를 배치할 위치
        [SerializeField]
        private LayerMask pickUpLayerMask;//해당 레이어만 잡는다
        [SerializeField]
        private float pickUpDistance = 2f;//잡기 거리
        [SerializeField]
        private float pickUpRadius = 1f;//잡기 반지름
        [SerializeField]
        private CharacterPuppet grabbedObject = null;//오브젝트 잡은 유무
        [SerializeField]
        private CharacterPuppet puppet = null;//내 퍼펫;
        [SerializeField]
        private GameObject grabParticle;
        [SerializeField]
        private bool drawGizmo = false;

        [Header("Delay")]
        public float skillDelay = 3f; // the amount of time before the skill can be used again
        private bool skillAvailable = true; // whether or not the skill can be used

        [Header("Photon")]
        [SerializeField]
        private PhotonView pv;

        public UserControlThirdPerson userControl; // user input

        private void Start()
        {
            PhotonNetwork.LogLevel = PunLogLevel.Full;
            PhotonNetwork.ConnectUsingSettings();

            //puppet = GetComponent<CharacterPuppet>();
            pv = GetComponent<PhotonView>();
            //int characterIndex = transform.root.GetComponent<ItemCharacterSwitcher>().currentCharacterIndex;
            //pv = transform.root.GetChild(characterIndex).GetComponent<PhotonView>();
        }

        private void Update()
        {
            if (!pv.IsMine) return;
            //잡기를 시도했다면
            if (userControl.state.doGrab)
            {
                //빈손이라면
                if (grabbedObject == null)
                {
                    //if (skillAvailable && !isPhoton)
                    if (skillAvailable)
                    {
                        //Grab();
                        pv.RPC("Grab", RpcTarget.All);
                        // start the delay coroutine
                        //StartCoroutine(SkillDelay());
                    }
                    //else
                    //{
                    //    pv.RPC("Grab", RpcTarget.All);
                    //}
                }
            }
        }

        [PunRPC]
        public void Grab()
        {
            Debug.Log("잡기시도");

            //충돌한 오브젝트 검사
            Ray ray = new Ray(objectGrabPointTransform.position, objectGrabPointTransform.forward);
            RaycastHit hit;

            // if (Physics.Raycast(objectGrabPointTransform.position, objectGrabPointTransform.forward, out RaycastHit raycastHit, pickUpDistance, pickUpLayerMask))
            if (Physics.SphereCast(ray, pickUpRadius, out hit, pickUpDistance))
            {
                if (hit.transform.gameObject.tag == "Player")
                {
                    //int characterIndex = hit.transform.root.GetComponent<ItemCharacterSwitcher>().currentCharacterIndex;
                    //CharacterPuppet grabbedObject = hit.transform.root.GetChild(characterIndex).GetChild(2).GetComponent<CharacterPuppet>();
                    CharacterPuppet grabbedObject = hit.transform.root.GetChild(2).GetComponent<CharacterPuppet>();

                    Debug.Log("잡기성공");
                    puppet.animState.successGrab = true;
                    grabbedObject.Grab(objectGrabSocketTransform);
                    Instantiate(grabParticle, objectGrabSocketTransform);
                    this.grabbedObject = grabbedObject;
                    //Debug.Log(grabbedObject);
                    Invoke("Drop", 0.8f);
                }
            }
        }

        [PunRPC]
        public void Drop()
        {
            Debug.Log("놓기");

            if (!grabbedObject) return;
            grabbedObject.Drop();
            puppet.animState.successGrab = false;
            grabbedObject = null;
        }

        void OnDrawGizmos()
        {
            if (!drawGizmo) return;

            Gizmos.color = Color.green;

            Gizmos.DrawSphere(objectGrabPointTransform.position, pickUpRadius);
            Gizmos.DrawLine(objectGrabPointTransform.position, objectGrabPointTransform.position + objectGrabPointTransform.forward * pickUpDistance);
            Gizmos.DrawSphere(objectGrabPointTransform.position + objectGrabPointTransform.forward * pickUpDistance, pickUpRadius);
        }
    }

}
