using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

namespace garden
{
    public class CameraController : MonoBehaviour
    {
        PhotonView pv;
        [Header("카메라 기본 설정")]
        [SerializeField]
        private Camera camera = null;
        [SerializeField]
        private Transform followTransrom = null;
        [SerializeField]
        private Vector3 offset = new Vector3(0, 1.5f, 0.5f); // 타겟과의 오프셋
        [SerializeField]
        private Transform rotationSpace; // 할당된 경우 이 변환의 회전을 월드 공간 대신 회전 공간으로 사용 (구형 행성에 유용)
        [SerializeField]
        private bool lockCursor = true; // 참이라면 마우스가 화면의 가운데에 고정되고, 숨겨진다
        [SerializeField]
        private bool smoothFollow; // 0보다 크다면, 타겟을 향해 부드럽게 움직임
        [SerializeField]
        private float followSpeed; // 부드러운 이동의 이동 속도

        [Header("거리")]
        [SerializeField]
        private float zoomSpeed = 10f; // 줌 속도
        [SerializeField]
        private float defaultDistance = 5f; // 게임 시작했을때 플레이어와 카메라 사이의 기본 거리
        [SerializeField]
        private float minDistance = 0f; // 플레이어와 카메라 사이의 최소 거리
        [SerializeField]
        private float maxDistance = 0f; // 플레이어와 카메라 사이의 최대 거리
        public float distance = 10.0f; // 현재 카메라와 타겟 사이의 거리

        [Header("회전")]
        [SerializeField]
        private bool invertX = false; // 좌우 회전 반전할지 말지
        [SerializeField]
        private bool invertY = false; // 상하 회전 반전할지 말지
        [SerializeField]
        private float rotationSharpness = 35f; // 회전 감도
        [SerializeField]
        private float defaultVerticalAngle = 20f; // 카메라 회전 초기값
        [SerializeField]
        [Range(-90, 90)]
        private float minVerticalAngle = -90; // 아래로 카메라를 회전할 수 있는 정도
        [SerializeField]
        [Range(-90, 90)]
        private float maxVerticalAngle = 90; // 위로 카메라를 회전할 수 있는 정도
        [SerializeField]
        private float mouseSensitivity = 1; // 마우스 감도

        //캐릭터 콜라이더랑 자꾸 충돌함... regdoll 문제일거 같다.. 아마..
        //[Header("카메라가 장애물에 막히는 것 방지를 위한 변수들")]
        //[SerializeField]
        //private float checkRadius = 0.2f; // 장애물 판정 반경
        //[SerializeField] 
        //private LayerMask obstructionLayers = -1; // 장애물 판정될 레이어
        //[SerializeField]
        //List<Collider> ignoreColliders = new List<Collider>(); // 장애물로 처리하지 않을 레이어

        public Vector3 cameraPlanarDirection { get => planarDirection; }

        //Privates
        private Vector3 planarDirection; // xz 평면의 전방 카메라
        private float targetDistance; // 카메라와 플레이어 사이의 거리
        private Vector3 targetPosition; // 카메라의 위치
        private Quaternion targetRotation; //가로 회전
        private float targetVerticalAngle; //세로 회전
        private Vector3 smoothPositon;
        private Vector3 lastUp;
        private bool fixedFrame;
        private float fixedDeltaTime;
        private Quaternion rotation = Quaternion.identity;
        private Vector3 position;

        private Vector3 newPosition;
        private Quaternion newRotation;

        public float x { get; private set; } // 카메라의 현재 x 회전 값
        public float y { get; private set; } // 카메라의 현재 y 회전 값
        public float distanceTarget { get; private set; } // 거리 get/set

        //인스펙터의 값이 바뀔때마다 호출된다.
        private void OnValidate()
        {
            defaultDistance = Mathf.Clamp(defaultDistance, minDistance, maxDistance);
            defaultVerticalAngle = Mathf.Clamp(defaultVerticalAngle, minVerticalAngle, maxVerticalAngle);
        }

        private void Awake()
        {
            pv = transform.root.GetComponent<PhotonView>();
            //int characterIndex = transform.root.GetComponent<ItemCharacterSwitcher>().currentCharacterIndex;
            //pv = transform.root.GetChild(characterIndex).GetComponent<PhotonView>();

            Vector3 angles = transform.eulerAngles;
            x = angles.y;
            y = angles.x;

            distanceTarget = distance;
            smoothPositon = transform.position;

            camera = GetComponent<Camera>();
            if (pv.IsMine)
            {
                camera = Camera.main;
                camera.transform.position = Vector3.zero;
                camera.transform.parent = gameObject.transform;
            }

            //if (pv.IsMine)
            //{
            //    camera = GetComponentInChildren<Camera>();
            //}

            lastUp = rotationSpace != null ? rotationSpace.up : Vector3.up;
        }

        private void Start()
        {
            //camera = Camera.main;
            //ignoreColliders.AddRange(GetComponentInChildren<Collider>()); // 값이 잘 안들어가서 그냥 public으로 빼서 하나씩 넣어줌

            planarDirection = Camera.main.transform.forward;//카메라의 처음 방향으로 초기화

            //타겟들 기본값 설정
            targetDistance = defaultDistance;//카메라 위치를 기본 위치로 초기화
            targetVerticalAngle = defaultVerticalAngle;
            targetRotation = Quaternion.LookRotation(planarDirection) * Quaternion.Euler(targetVerticalAngle, 0, 0);
            targetPosition = followTransrom.position - (targetRotation * Vector3.forward) * targetDistance;

            //마우스 Lock
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = lockCursor ? false : true;
        }

        private void FixedUpdate()
        {
            if (pv.IsMine)
            {
                fixedFrame = true;
                fixedDeltaTime += Time.deltaTime;
                UpdateTransform();
            }
        }
        private void Update()
        {
            if (pv.IsMine)
            {
                //esc를 누르면 마우스 커서 표시
                if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
                {
                    lockCursor = !lockCursor;
                    Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
                    Cursor.visible = lockCursor ? false : true;
                }

            }

        }

        private void LateUpdate()
        {
            if (pv.IsMine)
            {
                UpdateInput();

                if (fixedFrame)
                {
                    UpdateTransform(fixedDeltaTime);
                    fixedDeltaTime = 0;
                    fixedFrame = false;
                }
            }
        }

        public void UpdateInput()
        {
            //if (!camera.enabled) return;

            //커서가 Lock이 아니라면 카메라 움직이지 않기
            if (Cursor.lockState != CursorLockMode.Locked) return;

            //인풋 받아오기
            x += PlayerInputs.mouseXInput * mouseSensitivity;
            y = ClampAngle(y - PlayerInputs.mouseYInput * mouseSensitivity, minVerticalAngle, maxVerticalAngle);
            //float mouseX = PlayerInputs.mouseXInput * mouseSensitivity;
            //float mouseY = PlayerInputs.mouseYInput * mouseSensitivity;

            //if (invertX) { mouseX *= -1f; }
            //if (invertY) { mouseY *= -1f; }

            //float zoom = -PlayerInputs.mouseScrollInput * zoomSpeed;

            //distance
            distanceTarget = Mathf.Clamp(distanceTarget + Zoom, minDistance, maxDistance);
        }

        private float Zoom
        {
            get
            {
                float scrollAxis = PlayerInputs.mouseScrollInput;
                if (scrollAxis < 0) return -zoomSpeed;
                if (scrollAxis > 0) return zoomSpeed;
                return 0;
            }
        }

        public void UpdateTransform()
        {

            UpdateTransform(Time.deltaTime);
        }

        public void UpdateTransform(float deltaTime)
        {
            //if (!camera.enabled) return;

            rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);

            if (!followTransrom) return;

            //distance
            distance += (distanceTarget - distance) * zoomSpeed * deltaTime;

            //smooth follow
            if (!smoothFollow)
            {
                smoothPositon = followTransrom.position;
            }
            else
            {
                smoothPositon = Vector3.Lerp(smoothPositon, followTransrom.position, deltaTime * followSpeed);
            }

            //position
            Vector3 t = smoothPositon + rotation * offset;
            Vector3 f = rotation * -Vector3.forward;

            position = t + f * distance;

            camera.transform.position = position;
            camera.transform.rotation = rotation;

            //Vector3 focusPosition = followTransrom.position + camera.transform.TransformDirection(framing);

            //planarDirection = Quaternion.Euler(0, x, 0) * planarDirection;
            //targetDistance = Mathf.Clamp(targetDistance + zoomSpeed, minDistance, maxDistance);
            //targetVerticalAngle = Mathf.Clamp(targetVerticalAngle + y, minVerticalAngle, maxVerticalAngle);

            ////방향 그려보기
            //Debug.DrawLine(camera.transform.position, camera.transform.position + planarDirection, Color.red);

            ////최종 target
            //targetRotation = Quaternion.LookRotation(planarDirection) * Quaternion.Euler(targetVerticalAngle, 0, 0);
            ////targetPosition = focusPosition - (targetRotation * Vector3.forward) * smallestDistance; 캐릭터 콜라이더랑 자꾸 충돌함...
            //targetPosition = focusPosition - (targetRotation * Vector3.forward) * targetDistance;

            ////부드러운 회전
            //newRotation = Quaternion.Slerp(camera.transform.rotation, targetRotation, Time.deltaTime * rotationSharpness);
            ////부드러운 이동
            //newPosition = Vector3.Lerp(camera.transform.position, targetPosition, Time.deltaTime * rotationSharpness);

            ////카메라 회전값 적용
            //camera.transform.rotation = newRotation;
            ////카메라 위치값 적용
            //camera.transform.position = newPosition;
        }

        // Clamping Euler angles
        private float ClampAngle(float angle, float min, float max)
        {
            if (angle < -360) angle += 360;
            if (angle > 360) angle -= 360;
            return Mathf.Clamp(angle, min, max);
        }
    }
}



