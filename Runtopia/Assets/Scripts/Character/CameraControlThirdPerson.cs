// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// namespace garden
// {
//     public class CameraControlThirdPerson : MonoBehaviour
//     {
//         public Transform target; // ī�޶� ����ٴ� transform
//         public bool lockCursor = true;// true��� ���콺�� �������� ȭ���� �߾ӿ� �����ȴ�.
//
//         [Header("Position")]
//         public bool smoothFollow; // 0���� ũ�ٸ� ī�޶�� Ÿ�� Ʈ�������� ���� õõ�� �����Ѵ�.
//         public Vector3 offset = new Vector3(0, 1.5f, 0.5f); // ī�޶� ȸ���� Ÿ�� Ʈ������ ���̿� ���� ������
//         public float followSpeed = 10f; // õõ�� �����ϴ� �Ϳ� ���� ���ǵ�
//
//         [Header("Rotation")]
//         public float rotationSensitivity = 3.5f; // ī�޶� ȸ�� ����
//         public float yMinLimit = 20; // ī�޶� ���� ȸ���� ���� �ּ� ��
//         public float yMaxLimit = 80; // ī�޶� ���� ȸ���� ���� �ִ� ��
//         public bool rotateAlways = true; // ���콺 �������� �׻� ȸ���ϴ����� ���� ����
//
//         [Header("Distance")]
//         public float distance = 10.0f; // ������ġ���� Ÿ�ٱ����� �Ÿ�
//         public float minDistance = 4; // Ÿ�ٱ����� �ּ� �Ÿ�
//         public float maxDistance = 10; // Ÿ�ٱ����� �ִ� �Ÿ�
//         public float zoomSpeed = 10f; // ī�޶� Ȯ��/��� ���ǵ�
//         public float zoomSensitivity = 1f; // ī�޶� Ȯ��/��� ����
//
//         public float x { get; private set; } // ī�޶��� ���� x ȸ����
//         public float y { get; private set; } // ī�޶��� ���� y ȸ����
//         public float distanceTarget { get; private set; } // �Ÿ� get/set
//         public Vector3 CameraPlanerDirection { get => planarDirection; }
//
//         private Vector3 position;
//         private Vector3 planarDirection; // xz 평면의 전방 카메라
//         private Quaternion rotation = Quaternion.identity;
//         private Vector3 smoothPosition;
//         private Camera cam;
//         private bool fixedFrame;
//         private float fixedDeltaTime;
//
//         protected virtual void Awake()
//         {
//             planarDirection = Camera.main.transform.forward;//카메라의 처음 방향으로 초기화
//
//             Vector3 angles = transform.eulerAngles;
//             x = angles.y;
//             y = angles.x;
//
//             distanceTarget = distance;
//             smoothPosition = transform.position;
//
//             cam = GetComponent<Camera>();
//         }
//
//         private void Update()
//         {
//             float mouseX = PlayerInputs.mouseXInput;
//             planarDirection = Quaternion.Euler(0, mouseX, 0) * planarDirection;
//             //방향 그려보기
//             Debug.DrawLine(transform.position, transform.position + planarDirection, Color.red);
//         }
//
//         public void FixedUpdate()
//         {
//             fixedFrame = true;
//             fixedDeltaTime += Time.deltaTime;
//             UpdateTransform();
//         }
//
//         public void LateUpdate()
//         {
//             UpdateInput();
//         }
//
//         public void UpdateInput()
//         {
//             if (!cam.enabled) return;
//
//             // Cursors
//             Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
//             Cursor.visible = lockCursor ? false : true;
//
//             // Should we rotate the camera?
//             bool rotate = rotateAlways;
//
//             // delta rotation
//             if (rotate)
//             {
//                 x += UnityEngine.Input.GetAxis("Mouse X") * rotationSensitivity;
//                 y = ClampAngle(y - UnityEngine.Input.GetAxis("Mouse Y") * rotationSensitivity, yMinLimit, yMaxLimit);
//             }
//
//             // Distance
//             distanceTarget = Mathf.Clamp(distanceTarget + zoomAdd, minDistance, maxDistance);
//         }
//
//         //ī�޶� transform ������Ʈ
//         public void UpdateTransform()
//         {
//             UpdateTransform(Time.deltaTime);
//         }
//
//         public void UpdateTransform(float deltaTime)
//         {
//             if (!cam.enabled) return;
//
//             //ȸ��
//             rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);
//
//             if(target != null)
//             {
//                 //�Ÿ�
//                 distance += (distanceTarget - distance) * zoomSpeed * deltaTime;
//
//                 // ����
//                 if (!smoothFollow) smoothPosition = target.position;
//                 else smoothPosition = Vector3.Lerp(smoothPosition, target.position, deltaTime * followSpeed);
//
//                 // ��ġ
//                 Vector3 t = smoothPosition + rotation * offset;
//                 Vector3 f = rotation * -Vector3.forward;
//
//                 position = t + f * distance;
//
//                 // ī�޶� �̵�
//                 transform.position = position;
//             }
//
//             transform.rotation = rotation;
//         }
//
//         // Zoom input
//         private float zoomAdd
//         {
//             get
//             {
//                 float scrollAxis = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
//                 if (scrollAxis > 0) return -zoomSensitivity;
//                 if (scrollAxis < 0) return zoomSensitivity;
//                 return 0;
//             }
//         }
//
//         // Clamping Euler angles
//         private float ClampAngle(float angle, float min, float max)
//         {
//             if (angle < -360) angle += 360;
//             if (angle > 360) angle -= 360;
//             return Mathf.Clamp(angle, min, max);
//         }
//     }
// }
//
//
//
