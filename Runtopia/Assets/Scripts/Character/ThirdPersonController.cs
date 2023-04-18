using RootMotion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace garden
{
    public enum CharacterStance { Standing, Crouchng, Proning }
    public class ThirdPersonController : MonoBehaviour
    {
        private PhotonView pv;
        [Header("Speed(Normal, Sprinting)")]
        [SerializeField]
        private Vector2 standingSpeed = new Vector2(0, 0);
        [SerializeField]
        private Vector2 crouchingSpeed = new Vector2(0, 0);
        [SerializeField]
        private Vector2 proningSpeed = new Vector2(0, 0);

        [Header("Capsule (Radius, Height, YOffset")]
        [SerializeField] private Vector3 standingCapsule = Vector3.zero;
        [SerializeField] private Vector3 crouchingCapsule = Vector3.zero;
        [SerializeField] private Vector3 prouningCapsule = Vector3.zero;

        [Header("Movement")] //(블랜더와 매치되는 속도값과 맞춰줘야한다.)
        [SerializeField]
        private float walkSpeed = 2f;
        //[SerializeField]
        //private float runSpeed = 6f;
        //[SerializeField]
        //private float sprintSpeed = 8f;
        [SerializeField]
        private Vector3 moveInputVector = Vector3.zero;

        [Header("Sharpness")]
        [SerializeField]
        private float standingRotationSharpness = 10f;
        [SerializeField]
        private float crouchingRotationSharpness = 10f;
        [SerializeField]
        private float proningRotationSharpness = 10f;
        [SerializeField]
        private float moveShapness = 10f;

        [Header("Climb Settings")]
        [SerializeField]
        private float wallAngleMax; //벽과 캐릭터의 각도
        [SerializeField]
        private float groundAngleMax;
        [SerializeField]
        private float dropCheckDistance;
        [SerializeField]
        private LayerMask layerMaskClimb; //등산할 레이어

        [Header("Heights")]
        [SerializeField]
        private float overpassHeight;
        [SerializeField]
        private float hangHeight;
        [SerializeField]
        private float climbUpHeight;
        [SerializeField]
        private float vaultHeight;
        [SerializeField]
        private float stepHeight;

        [Header("Offsets")]
        [SerializeField]
        private Vector3 endOffset;
        [SerializeField]
        private Vector3 hangOffset;
        [SerializeField]
        private Vector3 dropOffset;
        [SerializeField]
        private Vector3 climbOriginDown;

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
        private CapsuleCollider collider;
        private PlayerInputs inputs;
        private CameraControllerBackUp cameraController;
        private SMBEventCurrator eventCurrator;

        private bool proning;

        private float runSpeed;
        private float sprintSpeed;
        private float rotationSharpness;
        private LayerMask layerMask;
        private CharacterStance stance;
        private Collider[] obstructions = new Collider[8];

        private bool strafing;
        private bool sprinting;
        private float strafeParameter;
        private Vector3 strafeParametersXZ;

        private float targetSpeed;
        private Quaternion targetRotation;

        private float newSpeed;
        private Vector3 newVelocity;
        private Quaternion newRotation;

        private bool climbing;
        private Vector3 endPosition;
        private Vector3 matchTargetPosition;
        private Quaternion matchTargetRotation;
        private Quaternion forwardNormalXZRotation;
        private RaycastHit downRaycastHit;
        private RaycastHit forwardRaycastHit;
        private MatchTargetWeightMask weightMask = new MatchTargetWeightMask(Vector3.one, 1);
        private Coroutine hangRoutine;

        [Header("Jump")]
        [SerializeField]
        private int inputCount = 0;//키 연속 입력 확인
        [SerializeField]
        private float doubleInputSpeed = 1f;//키 연속 입력 스피드
        
        [Header("GroundCheck")]
        [SerializeField]
        Transform groundCheck;
        [SerializeField]
        LayerMask ground;
        [SerializeField]
        Transform respawnPosition;
        [SerializeField]
        float fallingThreshold = -10f;

        private const string standToCrouch = "Base Layer.Base Crouching";
        private const string standToProne = "Base Layer.Stand To Prone";
        private const string crouchToStand = "Base Layer.Base Standing";
        private const string crouchToProne = "Base Layer.Crouch To Prone";
        private const string proneToStand = "Base Layer.Prone To Stand";
        private const string proneToCrouch = "Base Layer.Prone To Crouch";

        private void Start()
        {
            pv = GetComponent<PhotonView>();
            //animator = GetComponent<Animator>();
            rigidBody = GetComponent<Rigidbody>();
            capsule = GetComponent<CapsuleCollider>();
            collider = GetComponent<CapsuleCollider>();
            inputs = GetComponent<PlayerInputs>();
            eventCurrator = GetComponent<SMBEventCurrator>();
            cameraController = GetComponent<CameraControllerBackUp>();


            //초기화
            runSpeed = standingSpeed.x;
            sprintSpeed = standingSpeed.y;
            rotationSharpness = standingRotationSharpness;
            stance = CharacterStance.Standing;
            SetCapsuleDimensions((standingCapsule));
            eventCurrator.unityEvent.AddListener(OnSMBEvent);
            moveInputVector = new Vector3(inputs.MoveAxisRightRaw, 0, inputs.MoveAxisForwardRaw).normalized;
            respawnPosition = GameObject.Find("RespawnPosition").transform;

            int mask = 0;
            for (int i = 0; i < 32; i++)
            {
                if (!Physics.GetIgnoreLayerCollision(gameObject.layer, i))
                {
                    mask |= 1 << i;
                }
            }
            layerMask = mask;
            animator.applyRootMotion = false;
        }

        private void LateUpdate()
        {
            if (pv.IsMine)
            {
                //proning일때 마우스 회전 막기
                if (proning)
                {
                    return;
                }
                //키보드로 입력 받은 방향
                //Vector3 moveInputVector = new Vector3(inputs.MoveAxisRightRaw, 0, inputs.MoveAxisForwardRaw).normalized;
                moveInputVector = new Vector3(inputs.MoveAxisRightRaw, 0, inputs.MoveAxisForwardRaw).normalized;
                //카메라 평면 방향
                Vector3 cameraPlanarDirection = cameraController.cameraPlanarDirection;
                Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection);

                //키보드 입력 방향으로 기즈모 그리기
                //Debug.DrawLine(transform.position, transform.position + moveInputVector, Color.green);
                //키보드 입력 방향 벡터 * 카메라 방향 벡터
                Vector3 moveInputVectorOriented = cameraPlanarRotation * moveInputVector;
                //키보드 입력에 대한 카메라 방향으로 기즈모 그리기
                //Debug.DrawLine(transform.position, transform.position + moveInputVector, Color.red);

                if (!climbing)
                {
                    if (strafing)
                    {
                        sprinting = inputs.sprint.PressedDown() && moveInputVector != Vector3.zero;
                        strafing = inputs.aim.Pressed() && !sprinting;
                    }
                    else
                    {
                        sprinting = inputs.sprint.Pressed() && moveInputVector != Vector3.zero;
                        strafing = inputs.aim.PressedDown();
                    }

                    //이동 스피드
                    if (sprinting)
                    {
                        targetSpeed = moveInputVector != Vector3.zero ? sprintSpeed : 0;
                    }
                    else if (strafing)
                    {
                        targetSpeed = moveInputVector != Vector3.zero ? walkSpeed : 0;
                    }
                    else
                    {
                        targetSpeed = moveInputVector != Vector3.zero ? runSpeed : 0;
                    }
                    // 속도 보간
                    newSpeed = Mathf.Lerp(newSpeed, targetSpeed, Time.deltaTime * moveShapness);

                    //속도
                    newVelocity = moveInputVectorOriented * newSpeed;
                    transform.Translate(newVelocity * Time.deltaTime, Space.World);

                    //회전
                    if (strafing)
                    {
                        //카메라 방향 바라보기
                        targetRotation = Quaternion.LookRotation(cameraPlanarDirection);
                        newRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSharpness);
                        transform.rotation = newRotation;
                    }
                    //움직이고 있다면
                    else if (targetSpeed != 0)
                    {
                        //키보드 입력 방향 바라보기
                        targetRotation = Quaternion.LookRotation(moveInputVectorOriented);
                        newRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSharpness);
                        transform.rotation = newRotation;
                    }

                    //애니메이션
                    if (strafing)
                    {
                        strafeParameter = Mathf.Clamp01(strafeParameter + Time.deltaTime * 4);
                        strafeParametersXZ = Vector3.Lerp(strafeParametersXZ, moveInputVector * newSpeed, moveShapness * Time.deltaTime);
                    }
                    else
                    {
                        strafeParameter = Mathf.Clamp01(strafeParameter - Time.deltaTime * 4);
                        strafeParametersXZ = Vector3.Lerp(strafeParametersXZ, Vector3.forward * newSpeed, moveShapness * Time.deltaTime);
                    }

                    animator.SetFloat("Strafing", strafeParameter);
                    animator.SetFloat("StrafingX", Mathf.Round(strafeParametersXZ.x * 100) / 100f);
                    animator.SetFloat("StrafingZ", Mathf.Round(strafeParametersXZ.z * 100) / 100f);

                    JumpClimb();
                    IsGrounded();

                }
                ChangeCharacterStance();
            }
        }
        
        private void JumpClimb()
        {
            //점프
            if (inputs.jump.PressedDown() && IsGrounded())
            {
                inputCount++;

                animator.SetBool("IsJumping", true);

                StopAllCoroutines();//모든 코루틴 종료
                StartCoroutine(Jump());//점프 코루틴 시작

                animator.SetBool("IsGrounded", false);

                if (!IsInvoking("DoubleInput"))
                {
                    Invoke("DoubleInput", doubleInputSpeed);
                }
            }
            //매달리기
            else if (inputCount == 2)
            {
                Climb();
                Debug.Log("매달리기 입력");
            }
        }

        private IEnumerator Jump()
        {
            //Debug.Log("invoke jump");
            int steps = 0;
            int stepsToTake = 3;

            while (steps < stepsToTake)
            {
                rigidBody.AddForce(new Vector3(0, 5, 0) / stepsToTake, ForceMode.VelocityChange);
                steps++;
                if(inputs.jump.PressedDown()&&inputCount==2)
                {
                    Debug.Log("매달리기 입력");
                    Climb();
                    yield return null;
                }
                yield return new WaitForFixedUpdate();
            }
        }
        private void Climb()
        {
            StopAllCoroutines();//모든 코루틴 종료
            CancelInvoke("DoubleInput");
            animator.SetBool("IsJumping", false);
            if (CanClimb(out downRaycastHit, out forwardRaycastHit, out endPosition))
            {
                Debug.Log("매달리는상태로들어가봅니다");
                InitiateClimb();
            }
            DoubleInput();
        }

        private void DoubleInput()
        {
            inputCount = 0;
        }

        private bool IsGrounded()
        {
            //ground에 해당하는 레이어들 중에서 충돌한 게임 오브젝트를 체크한다
            bool check = Physics.CheckSphere(groundCheck.position, 0.1f, ground);
            //ground라면
            if(check)
            {
                animator.SetBool("IsGrounded", true);
                animator.SetBool("IsFalling", false);
                return true;
            }
            //ground가 아니라면
            else
            {
                animator.SetBool("IsGrounded", false);
                animator.SetBool("IsFalling", true);

                if(rigidBody.velocity.y < fallingThreshold)
                {
                    Respawn();
                }

                return false;
            }
        }

        private void Respawn()
        {
            gameObject.transform.position = respawnPosition.position;
            rigidBody.velocity = Vector3.zero;
        }

        private void OnAnimatorMove()
        {
            if (animator.isMatchingTarget)
            {
                animator.ApplyBuiltinRootMotion();
            }
        }

        private void ChangeCharacterStance()
        {
            switch (stance)
            {
                case CharacterStance.Standing:
                    if (inputs.crouching.PressedDown()) // c
                    {
                        //Debug.Log("LateUpdate : 난 일어서있었고 이제 쭈구릴거야");
                        RequestStanceChange(CharacterStance.Crouchng);
                    }
                    else if (inputs.proning.PressedDown()) // x
                    {
                        //Debug.Log("LateUpdate : 난 일어서있었고 이제 엎드릴거야");
                        RequestStanceChange(CharacterStance.Proning);
                    }
                    break;
                case CharacterStance.Crouchng:
                    if (inputs.crouching.PressedDown()) // c
                    {
                        //Debug.Log("LateUpdate : 난 쭈구려있었고 이제 일어설거야");
                        RequestStanceChange(CharacterStance.Standing);
                    }
                    else if (inputs.proning.PressedDown()) // x
                    {
                        //Debug.Log("LateUpdate : 난 쭈구려있었고 이제 엎드릴거야");
                        RequestStanceChange(CharacterStance.Proning);
                    }
                    break;
                case CharacterStance.Proning:
                    if (inputs.crouching.PressedDown()) // c
                    {
                        //Debug.Log("LateUpdate : 난 엎드려있었고 이제 쭈구릴거야");
                        RequestStanceChange(CharacterStance.Crouchng);
                    }
                    else if (inputs.proning.PressedDown()) // x
                    {
                        //Debug.Log("LateUpdate : 난 엎드려있었고 이제 엎드릴거야");
                        RequestStanceChange(CharacterStance.Proning);
                    }
                    break;
            }
        }
        private bool CanClimb(out RaycastHit _downRaycastHit, out RaycastHit _forwardRaycastHit, out Vector3 _endPosition)
        {
            _endPosition = Vector3.zero;
            _downRaycastHit = new RaycastHit();
            _forwardRaycastHit = new RaycastHit();

            bool downHit;
            bool forwardHit;
            bool overpassHit;
            float climbHeights;

            RaycastHit downRaycastHit;
            RaycastHit forwardRaycastHit;
            RaycastHit overpassRaycastHit;

            Vector3 endPosition;
            Vector3 forwardDirectionXZ;
            Vector3 forwardNormalXZ;

            Vector3 downDirection = Vector3.down;
            Vector3 downOrigin = transform.TransformPoint(climbOriginDown);
            CimbModifier climbModifier;

            downHit = Physics.Raycast(downOrigin, downDirection, out downRaycastHit, climbOriginDown.y - stepHeight, layerMaskClimb);

            climbModifier = downHit ? downRaycastHit.collider.GetComponent<CimbModifier>() : null;

            if (downHit)
            {

                Debug.Log("downHit" + downHit);
                if (climbModifier == null || climbModifier.Climbable)
                {

                    Debug.Log("climbModifier == null || climbModifier.Climbable");
                    //forward + overpass cast
                    float forwardDistance = climbOriginDown.z;
                    Vector3 forwardOrigin = new Vector3(transform.position.x, downRaycastHit.point.y - 0.1f, transform.position.z);
                    Vector3 overpassOrigin = new Vector3(transform.position.x, overpassHeight, transform.position.z);

                    forwardDirectionXZ = Vector3.ProjectOnPlane(transform.forward, Vector3.up);
                    forwardHit = Physics.Raycast(forwardOrigin, forwardDirectionXZ, out forwardRaycastHit, layerMaskClimb);
                    overpassHit = Physics.Raycast(overpassOrigin, forwardDirectionXZ, out overpassRaycastHit, layerMaskClimb);
                    climbHeights = downRaycastHit.point.y - transform.position.y;

                    if (forwardHit)
                    {
                        Debug.Log("forwardHit" + forwardHit);
                        if (overpassHit || climbHeights < overpassHeight)
                        {
                            Debug.Log("overpassHit || climbHeights < overpassHeight");
                            //Angles
                            forwardNormalXZ = Vector3.ProjectOnPlane(forwardRaycastHit.normal, Vector3.up);

                            //end offset
                            Vector3 vectSurface = Vector3.ProjectOnPlane(forwardDirectionXZ, forwardRaycastHit.normal);
                            endPosition = downRaycastHit.point + Quaternion.LookRotation(vectSurface, Vector3.up) * endOffset;

                            //de-penetration
                            Collider colliderB = downRaycastHit.collider;
                            bool penetrationOverlap = Physics.ComputePenetration(
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

            Debug.Log("매달리지마");
            return false;
        }

        private void InitiateClimb()
        {
            climbing = true;
            newSpeed = 0;
            animator.SetFloat("StrafingZ", 0);
            capsule.enabled = false;
            rigidBody.isKinematic = true;

            float climbHeight = downRaycastHit.point.y - transform.position.y;
            Vector3 forwardNormalXZ = Vector3.ProjectOnPlane(forwardRaycastHit.normal, Vector3.up);
            forwardNormalXZRotation = Quaternion.LookRotation(-forwardNormalXZ, Vector3.up);

            if (climbHeight > hangHeight)
            {
                Debug.Log("InitiateClimb:hangHeight");
                matchTargetPosition = forwardRaycastHit.point + forwardNormalXZRotation * hangOffset;
                matchTargetRotation = forwardNormalXZRotation;

                animator.CrossFadeInFixedTime(standToFreeHandSetting);
            }
            else if (climbHeight > climbUpHeight)
            {
                Debug.Log("InitiateClimb:climbUpHeight");
                matchTargetPosition = endPosition;
                matchTargetRotation = forwardNormalXZRotation;

                animator.CrossFadeInFixedTime(climbUpSetting);
            }
            else if (climbHeight > vaultHeight)
            {
                Debug.Log("InitiateClimb:vaultHeight");
                matchTargetPosition = endPosition;
                matchTargetRotation = forwardNormalXZRotation;

                animator.CrossFadeInFixedTime(vaultSetting);
            }
            else if (climbHeight > stepHeight)
            {
                Debug.Log("InitiateClimb:stepHeight");
                matchTargetPosition = endPosition;
                matchTargetRotation = forwardNormalXZRotation;

                animator.CrossFadeInFixedTime(stepUpSetting);
            }
            else
            {
                Debug.Log("InitiateClimb:아니야");
                climbing = false;
            }
        }

        public bool RequestStanceChange(CharacterStance newStance)
        {
            if (stance == newStance)
            {
                //Debug.Log("똑같은 자세야 바꿀필요없어");
                return true;
            }

            switch (stance)
            {
                // 일어서기
                case CharacterStance.Standing:
                    if (newStance == CharacterStance.Crouchng)
                    {
                        //Debug.Log("RequestStanceChange : 난 일어서있었고 이제 쭈구릴거야");
                        runSpeed = crouchingSpeed.x;
                        sprintSpeed = crouchingSpeed.y;
                        rotationSharpness = crouchingRotationSharpness;
                        stance = newStance;
                        animator.CrossFadeInFixedTime(standToCrouch, 0.5f);
                        SetCapsuleDimensions(crouchingCapsule);
                        return true;
                    }
                    else if (newStance == CharacterStance.Proning)
                    {
                        newSpeed = 0;
                        proning = true;
                        animator.SetFloat("StrafingZ", newSpeed);//speed 리셋

                        //Debug.Log("LateUpdate : 난 일어서있었고 이제 엎드릴거야");
                        runSpeed = proningSpeed.x;
                        sprintSpeed = proningSpeed.y;
                        rotationSharpness = proningRotationSharpness;
                        stance = newStance;
                        animator.CrossFadeInFixedTime(standToProne, 0.25f);
                        SetCapsuleDimensions(prouningCapsule);
                        return true;
                    }
                    break;
                // 쭈구리기
                case CharacterStance.Crouchng:
                    if (newStance == CharacterStance.Standing)
                    {
                        //Debug.Log("LateUpdate : 난 쭈구려있었고 이제 일어설거야");
                        runSpeed = standingSpeed.x;
                        sprintSpeed = standingSpeed.y;
                        rotationSharpness = standingRotationSharpness;
                        stance = newStance;
                        animator.CrossFadeInFixedTime(crouchToStand, 0.5f);
                        SetCapsuleDimensions(standingCapsule);
                        return true;
                    }
                    else if (newStance == CharacterStance.Proning)
                    {
                        //Debug.Log("LateUpdate : 난 쭈구려있었고 이제 엎드릴거야");
                        newSpeed = 0;
                        proning = true;
                        animator.SetFloat("StrafingZ", newSpeed);//speed 리셋

                        runSpeed = proningSpeed.x;
                        sprintSpeed = proningSpeed.y;
                        rotationSharpness = proningRotationSharpness;
                        stance = newStance;
                        animator.CrossFadeInFixedTime(crouchToProne, 0.25f);
                        SetCapsuleDimensions(prouningCapsule);
                        return true;
                    }
                    break;
                // 엎드리기
                case CharacterStance.Proning:
                    if (newStance == CharacterStance.Standing)
                    {
                        //Debug.Log("LateUpdate : 난 엎드려있었고 이제 쭈구릴거야");
                        newSpeed = 0;
                        proning = true;
                        animator.SetFloat("StrafingZ", newSpeed);//speed 리셋

                        runSpeed = standingSpeed.x;
                        sprintSpeed = standingSpeed.y;
                        rotationSharpness = standingRotationSharpness;
                        stance = newStance;
                        animator.CrossFadeInFixedTime(proneToStand, 0.5f);
                        SetCapsuleDimensions(standingCapsule);
                        return true;
                    }
                    else if (newStance == CharacterStance.Crouchng)
                    {
                        //Debug.Log("LateUpdate : 난 엎드려있었고 이제 엎드릴거야");
                        newSpeed = 0;
                        proning = true;
                        animator.SetFloat("StrafingZ", newSpeed);//speed 리셋

                        runSpeed = crouchingSpeed.x;
                        sprintSpeed = crouchingSpeed.y;
                        rotationSharpness = crouchingRotationSharpness;
                        stance = newStance;
                        animator.CrossFadeInFixedTime(proneToCrouch, 0.25f);
                        SetCapsuleDimensions(crouchingCapsule);
                        return true;
                    }
                    break;
            }
            //Debug.Log("나는 false야");
            return false;
        }

        private void SetCapsuleDimensions(Vector3 dimensions)
        {
            collider.center = new Vector3(collider.center.x, dimensions.z, collider.center.z);
            collider.radius = dimensions.x;
            collider.height = dimensions.y;
        }

        public void OnSMBEvent(string eventName)
        {
            switch (eventName)
            {
                case "ProneEnd":
                    //Debug.Log("쭈구리기 끝났다");
                    proning = false;
                    break;
                case "StandToFreeHangEnter":
                    Debug.Log("StandToFreeHangEnter");
                    animator.MatchTarget(matchTargetPosition, matchTargetRotation, AvatarTarget.Root, weightMask, 0.3f, 0.65f);
                    break;
                case "ClimbUpEnter":
                    Debug.Log("ClimbUpEnter");
                    animator.MatchTarget(matchTargetPosition, matchTargetRotation, AvatarTarget.Root, weightMask, 0, 0.9f);
                    break;
                case "VaultEnter":
                    Debug.Log("VaultEnter");
                    animator.MatchTarget(matchTargetPosition, matchTargetRotation, AvatarTarget.Root, weightMask, 0, 0.65f);
                    break;
                case "StepUpEnter":
                    Debug.Log("StepUpEnter");
                    animator.MatchTarget(matchTargetPosition, matchTargetRotation, AvatarTarget.Root, weightMask, 0.3f, 0.8f);
                    break;
                case "DropEnter":
                    Debug.Log("DropEnter");
                    animator.MatchTarget(matchTargetPosition, matchTargetRotation, AvatarTarget.Root, weightMask, 0.2f, 0.5f);
                    break;

                case "StandToFreeHangExit":
                    Debug.Log("StandToFreeHangExit");
                    hangRoutine = StartCoroutine(HangingRoutine());
                    break;
                case "ClimbUpExit":
                case "VaultExit":
                case "StepUpExit":
                case "DropExit":
                    Debug.Log("Exit");
                    climbing = false;
                    capsule.enabled = true;
                    rigidBody.isKinematic = false;
                    break;

                case "DropToAir":
                    Debug.Log("DropToAir");
                    climbing = false;
                    capsule.enabled = true;
                    rigidBody.isKinematic = false;
                    break;
                default:
                    break;
            }
        }

        private IEnumerator HangingRoutine()
        {
            //입력을 기다림
            while (inputs.MoveAxisForwardRaw == 0)
            {
                yield return null;
            }

            // 방향키 위키를 눌렀을때 위로 올라가기
            if (inputs.forward.Pressed())
            {
                matchTargetPosition = endPosition;
                matchTargetRotation = forwardNormalXZRotation;
                animator.CrossFadeInFixedTime(climbUpSetting);
            }
            //내려가기
            else if (inputs.backward.Pressed())
            {
                if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hitInfo, dropCheckDistance, layerMaskClimb))
                {
                    animator.CrossFade(dropToAirSetting);
                }
                else
                {
                    matchTargetPosition = hitInfo.point + forwardNormalXZRotation * dropOffset;
                    matchTargetRotation = forwardNormalXZRotation;
                    animator.CrossFadeInFixedTime(dropSetting);
                }
            }

            hangRoutine = null;
        }
    }
}

