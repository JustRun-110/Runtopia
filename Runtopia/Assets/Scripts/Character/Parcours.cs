using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.Demos;

namespace garden
{
    public class Parcours : MonoBehaviour
    {
        [Header("Climb Settings")]
        [SerializeField]
        private float wallAngleMax = 15f; //벽과 캐릭터의 각도
        [SerializeField]
        private float groundAngleMax = 10f;
        [SerializeField]
        private float dropCheckDistance = 4f;
        [SerializeField]
        private LayerMask layerMaskClimb; //등산할 레이어

        [Header("Heights")]
        [SerializeField]
        private float overpassHeight = 1.8f;
        [SerializeField]
        private float hangHeight = 1.35f;
        [SerializeField]
        private float climbUpHeight = 1.35f;
        [SerializeField]
        private float vaultHeight = 0.85f;
        [SerializeField]
        private float stepHeight = 0.25f;

        [Header("Offsets")]
        [SerializeField]
        private Vector3 endOffset = new Vector3(0, 0, 0.2f);
        [SerializeField]
        private Vector3 hangOffset = new Vector3(0, -1.5f, -0.1f);
        [SerializeField]
        private Vector3 dropOffset = new Vector3(0, 0, -0.2f);
        [SerializeField]
        private Vector3 climbOriginDown = new Vector3(0, 2.9f, 0.3f);

        [Header("Animation Settings")]
        public CrossFadeSettings standToFreeHandSetting;
        public CrossFadeSettings climbUpSetting;
        public CrossFadeSettings vaultSetting;
        public CrossFadeSettings stepUpSetting;
        public CrossFadeSettings dropSetting;
        public CrossFadeSettings dropToAirSetting;

        [SerializeField]
        private Animator animator;
        private Rigidbody rigidBody;
        private CapsuleCollider capsule;
        private PlayerInputs inputs;
        private SMBEventCurrator eventCurrator;
        public CharacterThirdPerson characterController;

        private bool climbing;//클라이밍 중인지 아닌지
        private Vector3 endPosition;
        private Vector3 matchTargetPosition; // 애니메이션의 끝 위치와 모델의 끝 위치 맞추기
        private Quaternion matchTargetRotation; // 애니메이션의 끝 방향과 모델의 끝 방향 맞추기
        private Quaternion forwardNormalXZRotation;
        private RaycastHit downRaycastHit;
        private RaycastHit forwardRaycastHit;
        private MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 1);
        private Coroutine hangRoutine;

        private void Start()
        {
            //animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();
            inputs = GetComponent<PlayerInputs>();
            eventCurrator = GetComponentInChildren<SMBEventCurrator>();

            eventCurrator.unityEvent.AddListener(OnSMBEvent);

            animator.applyRootMotion = false;
        }

        private void Update()
        {
            if (!climbing)
            {
                if (UnityEngine.Input.GetKeyDown(KeyCode.E))
                {
                    //Debug.Log("키입력 받음");
                    if (CanParcours(out downRaycastHit, out forwardRaycastHit, out endPosition))
                    {
                        //Debug.Log("매달리는상태로들어가봅니다");
                        InitiateParcours();
                    }
                }
            }
        }

        //private void OnAnimatorMove()
        //{
        //    Debug.Log("OnAnimatorMove");
        //    if (animator.isMatchingTarget)
        //    {
        //        Debug.Log("OnAnimatorMove isMatchingTarget");

        //        animator.ApplyBuiltinRootMotion();
        //    }
        //}

        private bool CanParcours(out RaycastHit _downRaycastHit, out RaycastHit _forwardRaycastHit, out Vector3 _endPosition)
        {
            _endPosition = Vector3.zero; // 착지할 위치
            _downRaycastHit = new RaycastHit(); // 아래 방향 충동체
            _forwardRaycastHit = new RaycastHit(); // 앞쪽 방향 충돌체

            bool downHit;
            bool forwardHit;
            bool overpassHit;
            float climbHeight;

            RaycastHit downRaycastHit;
            RaycastHit forwardRaycastHit;
            RaycastHit overpassRaycastHit;

            Vector3 endPosition;
            Vector3 forwardDirectionXZ;

            Vector3 downDirection = Vector3.down;
            Vector3 downOrigin = transform.TransformPoint(climbOriginDown);
            CimbModifier climbModifier;

            //Debug.Log("downOrigin" + downOrigin);
            //Debug.Log("downDirection" + downDirection);
            //Debug.Log("climbOriginDown.y - stepHeight" + (climbOriginDown.y - stepHeight));

            // public static bool Raycast(Vector3 origin,Vector3 direction,out RaycastHit hitInfo,float maxDistance,int layerMask)
            downHit = Physics.Raycast(downOrigin, downDirection, out downRaycastHit, climbOriginDown.y - stepHeight, layerMaskClimb);

            // 클라임할 수 있는 오브젝트인지 확인 있으면 해당 클래스 반환 없으면 null 반환
            climbModifier = downHit ? downRaycastHit.collider.GetComponent<CimbModifier>() : null;

            //클라임할 수 있는 위치라면
            if (downHit)
            {
                //Debug.Log("파쿠르 할 수 있는 위치야");
                // 클라임할 수 있는 오브젝트라면
                if (climbModifier == null || climbModifier.Climbable)
                {
                    //forward + overpass cast
                    Vector3 forwardOrigin = new Vector3(transform.position.x, downRaycastHit.point.y - 0.1f,
                transform.position.z);
                    Vector3 overpassOrigin = new Vector3(transform.position.x, overpassHeight, transform.position.z);

                    forwardDirectionXZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
                    forwardHit = Physics.Raycast(forwardOrigin, forwardDirectionXZ, out forwardRaycastHit, layerMaskClimb);
                    overpassHit = Physics.Raycast(overpassOrigin, forwardDirectionXZ, out overpassRaycastHit,
                    layerMaskClimb);
                    climbHeight = downRaycastHit.point.y - transform.position.y;

                    if (forwardHit)
                    {
                        //Debug.Log("무언가에 부딪혔다");
                        if (overpassHit || climbHeight < overpassHeight)
                        {
                            //Debug.Log("올라갈 수 있는 것과 부딪힘");
                            // end offset
                            // plane의 벡터 반환
                            Vector3 vectSurface = Vector3.ProjectOnPlane(forwardDirectionXZ, forwardRaycastHit.normal);
                            endPosition = downRaycastHit.point +
                                          Quaternion.LookRotation(vectSurface, Vector3.up) * endOffset;

                            //de-penetration
                            Collider colliderB = downRaycastHit.collider;

                            Debug.Log(colliderB.transform);

                            bool penetrationOverlap = Physics.ComputePenetration( //nullrefernece가 계속 뜬다....
                                colliderA: capsule,
                                positionA: endPosition,
                                rotationA: transform.rotation,
                                colliderB: colliderB,
                                positionB: colliderB.transform.position,
                                rotationB: colliderB.transform.rotation,
                                direction: out Vector3 penetrationDirection,
                                distance: out float penetrationDistance);

                            if (penetrationOverlap)
                            {
                                endPosition += penetrationDirection * penetrationDistance;
                            }

                            _endPosition = endPosition;
                            _downRaycastHit = downRaycastHit;
                            _forwardRaycastHit = forwardRaycastHit;

                            Debug.Log("매달리는거 허락");
                            return true;
                        }
                    }
                }
            }
            Debug.Log("올라가지마");
            return false;
        }

        private void InitiateParcours()
        {
            climbing = true;
            animator.SetFloat("Forward", 0);
            capsule.enabled = false;
            rigidBody.isKinematic = true;

            float climbHeight = downRaycastHit.point.y - transform.position.y;
            Vector3 forwardNormalXZ = Vector3.ProjectOnPlane(forwardRaycastHit.normal, Vector3.up);
            forwardNormalXZRotation = Quaternion.LookRotation(-forwardNormalXZ, Vector3.up);

            //if (climbHeight > hangHeight)
            //{
            //    climbing = false;

            //    //matchTargetPosition = forwardRaycastHit.point + forwardNormalXZRotation * hangOffset;
            //    //matchTargetRotation = forwardNormalXZRotation;

            //    //animator.CrossFadeInFixedTime(standToFreeHandSetting);
            //}
            if (climbHeight > climbUpHeight)
            {
                matchTargetPosition = endPosition;
                matchTargetRotation = forwardNormalXZRotation;

                animator.CrossFadeInFixedTime(climbUpSetting);
            }
            else if (climbHeight > vaultHeight)
            {
                matchTargetPosition = endPosition;
                matchTargetRotation = forwardNormalXZRotation;

                animator.CrossFadeInFixedTime(vaultSetting);
            }
            else if (climbHeight > stepHeight)
            {
                matchTargetPosition = endPosition;
                matchTargetRotation = forwardNormalXZRotation;

                animator.CrossFadeInFixedTime(stepUpSetting);
            }
            else
            {
                climbing = false;
            }
        }

        public void OnSMBEvent(string eventName)
        {
            switch (eventName)
            {
                //애니메이션 시작이라면
                //case "StandToFreeHangEnter":
                //    characterController.fullRootMotion = true;
                //    animator.MatchTarget(matchTargetPosition, matchTargetRotation, AvatarTarget.Root, weightMask, 0.3f, 0.65f);
                //    break;
                case "ClimbUpEnter":
                    characterController.fullRootMotion = true;
                    animator.MatchTarget(matchTargetPosition, matchTargetRotation, AvatarTarget.Root, weightMask, 0, 0.9f);
                    break;
                case "VaultEnter":
                    characterController.fullRootMotion = true;
                    animator.MatchTarget(matchTargetPosition, matchTargetRotation, AvatarTarget.Root, weightMask, 0, 0.65f);
                    break;
                case "StepUpEnter":
                    characterController.fullRootMotion = true;
                    animator.MatchTarget(matchTargetPosition, matchTargetRotation, AvatarTarget.Root, weightMask, 0.3f, 0.8f);
                    break;
                case "DropEnter":
                    characterController.fullRootMotion = true;
                    animator.MatchTarget(matchTargetPosition, matchTargetRotation, AvatarTarget.Root, weightMask, 0.2f, 0.5f);
                    break;
                //애니메이션 끝났다면
                //case "StandToFreeHangExit":
                //    hangRoutine = StartCoroutine(HangingRoutine());
                //    characterController.fullRootMotion = false;
                //    break;
                case "ClimbUpExit":
                case "VaultExit":
                case "StepUpExit":
                case "DropExit":
                    Debug.Log("~~~~~Climb Exit~~~~~");
                    climbing = false;
                    capsule.enabled = true;
                    rigidBody.isKinematic = false;
                    characterController.fullRootMotion = false;
                    break;
                //case "DropToAir":
                //    climbing = false;
                //    capsule.enabled = true;
                //    rigidBody.isKinematic = false;
                //    characterController.fullRootMotion = false;
                //    break;
                default:
                    break;
            }
        }

        public void InitClimb()
        {                    
            climbing = false;
            capsule.enabled = true;
            rigidBody.isKinematic = false;
            characterController.fullRootMotion = false;        
        }
        
        //private IEnumerator HangingRoutine()
        //{
        //    //입력을 기다림
        //    while (inputs.MoveAxisForwardRaw == 0)
        //    {
        //        yield return null;
        //    }

        //    // 방향키 위키를 눌렀을때 위로 올라가기
        //    if (inputs.forward.Pressed())
        //    {
        //        matchTargetPosition = endPosition;
        //        matchTargetRotation = forwardNormalXZRotation;
        //        animator.CrossFadeInFixedTime(climbUpSetting);
        //    }
        //    //내려가기
        //    else if (inputs.backward.Pressed())
        //    {
        //        if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, dropCheckDistance, layerMaskClimb))
        //        {
        //            animator.CrossFade(dropToAirSetting);
        //        }
        //        else
        //        {
        //            matchTargetPosition = hitInfo.point + forwardNormalXZRotation * dropOffset;
        //            matchTargetRotation = forwardNormalXZRotation;
        //            animator.CrossFadeInFixedTime(dropSetting);
        //        }
        //    }

        //    hangRoutine = null;
        //}

    }
}