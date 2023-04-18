using UnityEngine;
using System.Collections;
namespace garden {

	public class PlayerControlThirdPerson : MonoBehaviour {

		// Input state
		public struct State {
			public Vector3 move;
			public Vector3 lookPos;
			public bool crouch;
			public bool jump;
			public int actionIndex;
		}
		public bool walkByDefault;        // toggle for walking state
		public bool canCrouch = true;
		public bool canJump = true;

		public State state = new State();			// 유저가 입력한 정보에 대한 현재 상태

		protected Transform cam;                    // A reference to the main camera in the scenes transform 

		protected virtual void Start () {
            // 메인 카메라의 트랜스폼을 얻어온다.
            cam = Camera.main.transform;
		}

		protected virtual void Update () {

			// 입력 받기
			state.crouch = canCrouch && UnityEngine.Input.GetKey(KeyCode.C);
			state.jump = canJump && UnityEngine.Input.GetButton("Jump");

			float h = UnityEngine.Input.GetAxisRaw("Horizontal");
			float v = UnityEngine.Input.GetAxisRaw("Vertical");
			
			// 이동 방향 계산
			Vector3 move = cam.rotation * new Vector3(h, 0f, v).normalized;

			// Flatten move vector to the character.up plane
			if (move != Vector3.zero) {
				Vector3 normal = transform.up;
				Vector3.OrthoNormalize(ref normal, ref move);
				state.move = move;
			} else state.move = Vector3.zero;

			bool walkToggle = UnityEngine.Input.GetKey(KeyCode.LeftShift);

			// We select appropriate speed based on whether we're walking by default, and whether the walk/run toggle button is pressed:
			float walkMultiplier = (walkByDefault ? walkToggle ? 1 : 0.5f : walkToggle ? 0.5f : 1);

			state.move *= walkMultiplier;
			
			// 바라보는 방향 계산
			state.lookPos = transform.position + cam.forward * 100f;
		}
		
	}
}

