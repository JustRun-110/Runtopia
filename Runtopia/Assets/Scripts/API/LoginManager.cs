using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;



namespace sjb
{
    class UserInfo
    {
        public string userId;
        public string nickname;
        public string password;
        public bool gender;
    }
    class LoginInfo
    {
        public string userId;
        public string password;
    }
    public class LoginManager : MonoBehaviour
    {

        [Tooltip("로그인패널")]
        private GameObject loginPanel;

        [Tooltip("회원가입패널")]
        private GameObject signPanel;

        [Tooltip("시작패널")]
        private GameObject startPanel;

        [Tooltip("팝업패널")]
        private GameObject popupPanel;

        [Tooltip("대기패널")]
        public GameObject waitingPanel;

        //회원가입 폼
        public TMP_InputField signupID;
        public TMP_InputField signupPW1;
        public TMP_InputField signupPW2;
        public TMP_InputField signupNickname;
        public ToggleGroup signupGender;

        //팝업내용
        public TMP_Text popupText ;

        //로그인폼
        public TMP_InputField loginID;
        public TMP_InputField loginPW;

        //로그인 저장
        public Toggle remember;
        private void Awake()
        {
            startPanel = GameObject.Find("Main Panel");
            loginPanel = GameObject.Find("Login Panel");
            signPanel = GameObject.Find("Sign Panel");
            popupPanel = GameObject.Find("Popup");
            Debug.Log(waitingPanel);
        }
        // Start is called before the first frame update
        void Start()
        {
            SetPanel(startPanel.name);

            //로그인 저장 여부 확인
            if (PlayerPrefs.GetString("Remeber","false").Equals("true"))
            {
                remember.isOn = true;
                loginID.text = PlayerPrefs.GetString("USER_ID","");
            }
            else
            {
                remember.isOn = false;
            }
        }
        #region 로그인

        public void OnLoginClick()
        {
            //로그인 진행
            if(loginID.text == "")
            {
                OnPopup("아이디를 \n 입력해주세요");
                return;
            }
            if(loginPW.text == "")
            {
                OnPopup("비밀번호를 \n 입력해주세요.");
                return;
            }

            LoginInfo login = new LoginInfo();
            login.userId = loginID.text;
            login.password = loginPW.text;

            string json = JsonUtility.ToJson(login);
            loginReset();
            //온라인 로그인 처리
            StartCoroutine(Request.Instance.ApiPostRequest("/api/v1/users/login/", json,"login"));

            //오프라인 로그인 처리
            //PlayerPrefs.SetString("USER_NickName", $"nickTest-{UnityEngine.Random.Range(1, 100):000}");
            //PlayerPrefs.SetString("USER_ID", $"idTest-{UnityEngine.Random.Range(1, 100):000}");
            //PlayerPrefs.SetInt("USER_Gender", UnityEngine.Random.Range(0,1));
            //SceneManager.LoadScene("Lobby");
        }

        public void loginReset()
        {
            PlayerPrefs.SetString("isFirst", "yes");
            loginID.text = "";
            loginPW.text = "";
        }
        
        #endregion
        
        #region 회원가입
        public void signupReset()
        {
            signupID.text = "";
            signupPW1.text = "";
            signupPW2.text = "";
            signupNickname.text = "";
        }
        public void OnSignUp()
        {
            UserInfo userinfo = new UserInfo();
            //회원가입 진행
            if (signupID.text == "")
            {
                OnPopup("아이디를 \n 입력해주세요.");
                return;
            }
            else
            {
                //영어, 숫자만 입력했는지 확인
                Regex regex = new Regex("^[A-Za-z0-9]+$");
                Match m = regex.Match(signupID.text);
                if (!m.Success)
                {

                    OnPopup("아이디를 \n 확인해주세요.");
                    return;
                }
            }
            if (signupPW1.text == "")
            {
                OnPopup("비밀번호를 \n 입력해주세요.");
                return;
            }
            if (signupPW2.text == "" || signupPW2.text!= signupPW1.text)
            {
                OnPopup("비밀번호를 \n 확인해주세요.");
                return;
            }
            if (signupNickname.text == "" )
            {
                OnPopup("닉네임을 \n 확인해주세요.");
                return;
            }
            userinfo.nickname = signupNickname.text;
            userinfo.password = signupPW1.text;
            userinfo.userId = signupID.text;
            Toggle selectedToggle = signupGender.ActiveToggles().FirstOrDefault();
            if (selectedToggle != null)
            {
                Debug.Log(selectedToggle.ToString());
                if (selectedToggle.gameObject.name == "남성")
                {
                    userinfo.gender = true;
                }
                else
                {
                    userinfo.gender = false;
                }
            }
            
            string json = JsonUtility.ToJson(userinfo);
            StartCoroutine(Request.Instance.ApiPostRequest("/api/v1/users/",json,"signup"));
            signupReset();
        }
        #endregion

        #region 판넬 전환
        public void SetPanel(string panel)
        {
            startPanel.SetActive(panel.Equals(startPanel.name));
            loginPanel.SetActive(panel.Equals(loginPanel.name));
            signPanel.SetActive(panel.Equals(signPanel.name));
            popupPanel.SetActive(panel.Equals(popupPanel.name));
            waitingPanel.SetActive(panel.Equals(waitingPanel.name));
        }
        public void OnLoginPage()
        {
            SetPanel(loginPanel.name);

        }
        public void OnSignUpPage()
        {
            SetPanel(signPanel.name);
        }

        public void OnSignUpClose()
        {
            SetPanel(loginPanel.name);
            signupReset();
        }   

        public void OnLoginClose()
        {
            SetPanel(startPanel.name);
        }

        
        public void OnPopup(string text)
        {
            popupPanel.SetActive(true);
            Animator animator  = popupPanel.GetComponent<Animator>();
            if (animator != null)
            {
                bool isOpen = animator.GetBool("open");
                
                animator.SetBool("open",!isOpen);
            }
            popupText.text = text;
        }

        public void OnPopupClose()
        {
            popupPanel.SetActive(false);
        }



        public void OpenWaiting()
        {
            SetPanel(waitingPanel.name);
        }
        public void SetWaiting(string text)
        {
            GameObject.Find("waitingContent").GetComponent<TMP_Text>().text = text;
        }

        #endregion

        public void OnRemember()
        {
            if (remember.isOn)
            {
                PlayerPrefs.SetString("Remeber","true");
            }
            else
            {
                PlayerPrefs.SetString("Remeber","false");
            }
        }
        public void GameQuit()
        {
            Application.Quit();
        }




        void OnApplicationQuit()
        {
            Request.Instance.DisconnectServer();
        }

    }
}
