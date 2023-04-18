using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace garden
{
    [System.Serializable]
    public class Input
    {
        public KeyCode primary;
        public KeyCode alternate;

        public bool Pressed()
        {
            return UnityEngine.Input.GetKey(primary) || UnityEngine.Input.GetKey(alternate);
        }
        public bool PressedDown()
        {
            return UnityEngine.Input.GetKeyDown(primary) || UnityEngine.Input.GetKeyDown(alternate);
        }
        public bool PressedUp()
        {
            return UnityEngine.Input.GetKeyUp(primary) || UnityEngine.Input.GetKeyUp(alternate);
        }
    }


    public class PlayerInputs : MonoBehaviour
    {
        public Input forward;
        public Input backward;
        public Input right;
        public Input left;
        public Input sprint;
        public Input aim;
        public Input crouching;
        public Input proning;
        public Input jump;

        /// <summary>
        /// 앞 뒤 input 정보
        /// </summary>
        public int MoveAxisForwardRaw
        {
            get
            {
                if(forward.Pressed() && backward.Pressed()) { return 0; } //위 아래가 같이 눌렸을 때
                else if (forward.Pressed() ) { return 1; } //앞이 눌렸을 때
                else if (backward.Pressed()) { return -1; } //뒤가 눌렸을 때
                else { return 0; } // 아무것도 아닐때
            }
        }

        /// <summary>
        /// 양옆 input 정보
        /// </summary>
        public int MoveAxisRightRaw
        {
            get
            {
                if (right.Pressed() && left.Pressed()) { return 0; } //양옆이 같이 눌렸을 때
                else if (right.Pressed()) { return 1; } //오른쪽이 눌렸을 때
                else if (left.Pressed()) { return -1; } //왼쪽이 눌렸을 때
                else { return 0; } // 아무것도 아닐때
            }
        }

        [Header("Mouse Input")]
        public const string mouseXString = "Mouse X";
        public const string mouseYString = "Mouse Y";
        public const string mouseScrollString = "Mouse ScrollWheel";

        public static float mouseXInput { get => UnityEngine.Input.GetAxis(mouseXString); }
        public static float mouseYInput { get => UnityEngine.Input.GetAxis(mouseYString); }
        public static float mouseScrollInput { get => UnityEngine.Input.GetAxis(mouseScrollString); }
    }
}
