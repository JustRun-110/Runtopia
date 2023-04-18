using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace garden
{
    public class CameraControllerBackUp : MonoBehaviour
    {
        PhotonView pv;
        [Header("카메라 기본 설정")]
        [SerializeField]
        private Camera camera = null;
        [SerializeField]
        private Transform followTransrom = null;
        [SerializeField]
        private Vector3 framing = Vector3.zero; // 타겟과의 오프셋

        [Header("거리")]
        [SerializeField]
        private float zoomSpeed = 10f; // 줌 속도
        [SerializeField]
        private float defaultDistance = 5f; // 게임 시작했을때 플레이어와 카메라 사이의 기본 거리
        [SerializeField]
        private float minDistance = 0f; // 플레이어와 카메라 사이의 최소 거리
        [SerializeField]
        private float maxDistance = 0f; // 플레이어와 카메라 사이의 최대 거리

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

        private Vector3 newPosition;
        private Quaternion newRotation;

        //인스펙터의 값이 바뀔때마다 호출된다.
        private void OnValidate()
        {
            defaultDistance = Mathf.Clamp(defaultDistance, minDistance, maxDistance);
            defaultVerticalAngle = Mathf.Clamp(defaultVerticalAngle, minVerticalAngle, maxVerticalAngle);
        }

        private void Start()
        {
            pv = GetComponentInParent<PhotonView>();
            camera = Camera.main;
            //ignoreColliders.AddRange(GetComponentInChildren<Collider>()); // 값이 잘 안들어가서 그냥 public으로 빼서 하나씩 넣어줌

            planarDirection = Camera.main.transform.forward;//카메라의 처음 방향으로 초기화

            //타겟들 기본값 설정
            targetDistance = defaultDistance;//카메라 위치를 기본 위치로 초기화
            targetVerticalAngle = defaultVerticalAngle;
            targetRotation = Quaternion.LookRotation(planarDirection) * Quaternion.Euler(targetVerticalAngle, 0, 0);
            //Debug.Log(followTransrom);
            targetPosition = followTransrom.position - (targetRotation * Vector3.forward) * targetDistance;

            //마우스 Lock
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void LateUpdate()
        {
            if (pv.IsMine)
            {
                //커서가 Lock이 아니라면 카메라 움직이지 않기
                if (Cursor.lockState != CursorLockMode.Locked) return;

                //인풋 받아오기
                float zoom = -PlayerInputs.mouseScrollInput * zoomSpeed;
                float mouseX = PlayerInputs.mouseXInput * mouseSensitivity;
                float mouseY = PlayerInputs.mouseYInput * mouseSensitivity;

                if (invertX) { mouseX *= -1f; }
                if (invertY) { mouseY *= -1f; }

                Vector3 focusPosition = followTransrom.position + camera.transform.TransformDirection(framing);

                planarDirection = Quaternion.Euler(0, mouseX, 0) * planarDirection;
                targetDistance = Mathf.Clamp(targetDistance + zoom, minDistance, maxDistance);
                targetVerticalAngle = Mathf.Clamp(targetVerticalAngle + mouseY, minVerticalAngle, maxVerticalAngle);

                //방향 그려보기
                Debug.DrawLine(camera.transform.position, camera.transform.position + planarDirection, Color.red);

                //최종 target
                targetRotation = Quaternion.LookRotation(planarDirection) * Quaternion.Euler(targetVerticalAngle, 0, 0);
                //targetPosition = focusPosition - (targetRotation * Vector3.forward) * smallestDistance; 캐릭터 콜라이더랑 자꾸 충돌함...
                targetPosition = focusPosition - (targetRotation * Vector3.forward) * targetDistance;

                //부드러운 회전
                newRotation = Quaternion.Slerp(camera.transform.rotation, targetRotation, Time.deltaTime * rotationSharpness);
                //부드러운 이동
                newPosition = Vector3.Lerp(camera.transform.position, targetPosition, Time.deltaTime * rotationSharpness);

                //카메라 회전값 적용
                camera.transform.rotation = newRotation;
                //카메라 위치값 적용
                camera.transform.position = newPosition;
            }
        }
    }
}




