using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;

using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System;
using System.Text;
//웹소캣
using WebSocketSharp;
using WebSocketSharp.Server;

using TMPro;

namespace sjb
{
    public class Result
    {
        public string message;
        public string statusCode;
    }
    public class RankingEntry
    {
        public string userId;
        public int mmr;
        public int overallRanking;
    }
    public class Request : MonoBehaviour
    {
        private string apiUrl = "https://j8b110.p.ssafy.io";
        //private string apiUrl = "http://localhost:8000";

        //socket ip
        private string socketIP = "wss://j8b110.p.ssafy.io/api/websocket";
        //private string socketIP = "ws://localhost:8000/api/websocket";
        public WebSocketSharp.WebSocket wSocket = null;
        private string token = null; 
        private static Request instance = null;
        private static LoginManager loginManager = null;
        private bool _lobbySwitch;
        private bool _waitingSwitch;
        private string _watingText;

        public static Request Instance
        {
            get
            {
                if (instance == null)
                {
                    //instance = new Request();
                    GameObject go = new GameObject();
                    instance = go.AddComponent<Request>();
                    //instance = GameObject.Find("Request").GetComponent<Request>();
                }
                return instance;
            }
        }

        private void Start()
        {
            loginManager = GameObject.Find("LoginManager").GetComponent<LoginManager>();
            _lobbySwitch = false;
            _waitingSwitch = false;
            DontDestroyOnLoad(this.gameObject);
        }

        private void Update()
        {
            if (_lobbySwitch)
            {
                _lobbySwitch = false;
                SceneManager.LoadScene("Lobby");
            }
            if (_waitingSwitch)
            {
                _waitingSwitch = false;
                loginManager.OpenWaiting();
                loginManager.SetWaiting(_watingText);

            }
        }

        #region api성공
        public void LoginSuccess(string loginId)
        {
            //loginId
            StartCoroutine(ApiGetRequest("/api/v1/users/me/"+loginId+"/","userInfo"));
        }

        public void UserInfoSuccess(string result)
        {
            UserInfo req = JsonUtility.FromJson<UserInfo>(result);
            PlayerPrefs.SetString("USER_NickName", req.nickname);
            PlayerPrefs.SetString("USER_ID", req.userId);
            PlayerPrefs.SetInt("USER_Gender", req.gender?1:0);
            
            //소켓 통신 시작
            //SceneManager.LoadScene("Lobby");
            Connect();
        }
        public void SignupSuccess()
        {
            GameObject.Find("LoginManager").GetComponent<LoginManager>().OnLoginPage();
        }
        public void RankingSuccess()
        {
            Debug.Log("랭킹 업데이트 완료");
        }

        public void GetRankingSuccess(string ranklist)
        {
            string pat = @"\{([^{}]+)\}";

            Regex r = new Regex(pat, RegexOptions.IgnoreCase);

            Match m = r.Match(ranklist);
            int matchCnt = 0;
            while (m.Success)
            {
                if (matchCnt > 5)
                {
                    break;
                }
                matchCnt += 1;
                for (int i = 1; i < 2; i++)
                {
                    Group g = m.Groups[i];
                    RankingEntry RE = JsonUtility.FromJson<RankingEntry>("{" + g + "}");
                    GameObject.Find("RankerManager").GetComponent<RankerManager>().AddRanker(RE.overallRanking, RE.userId, RE.mmr);
                }
                m = m.NextMatch();
            }

            Debug.Log("랭킹조회 완료");
        }
        public void SetMyInfo()
        {
            Debug.Log("마이페이지 수정 완료");
        }
        #endregion

        #region api실패
        public void LoginFail()
        {
            GameObject.Find("LoginManager").GetComponent<LoginManager>().OnPopup("로그인정보를 \n 확인해주세요");
        }
        #endregion
        
        #region API요청
        
        public IEnumerator ApiPostRequest(string api, string input,string type)
        {
            //참고 https://timeboxstory.tistory.com/83
            using (UnityWebRequest request = UnityWebRequest.Post(apiUrl+ api, input))
            {
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(input);
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                

                if (request.result == UnityWebRequest.Result.Success)
                {
                    if (type.Equals("login"))
                    {
                        LoginInfo req = JsonUtility.FromJson<LoginInfo>(input);
                        LoginSuccess(req.userId);

                    }else if(type.Equals("signup"))
                    {
                        SignupSuccess();
                    }else if (type.Equals("ranking"))
                    {
                        RankingSuccess();
                    }
                    //성공
                    request.Dispose(); // 메모리 누수로 해제 하기
                }
                else
                {

                    //에러
                    if (type.Equals("login"))
                    {
                        //Debug.Log(request.error);
                        //Debug.LogError("로그인 안됨");
                        LoginFail();
                    }else if (type.Equals("signup"))
                    {
                        //Debug.LogError("가입 안됨");
                    }
                    else if (type.Equals("ranking"))
                    {
                        //Debug.LogError("랭킹 실패");
                    }


                    request.Dispose(); // 메모리 누수로 해제 하기
                }
                request.Dispose();
            }
        }

        public IEnumerator ApiGetRequest(string api,string type)
        {
            //참고 https://timeboxstory.tistory.com/83
            using (UnityWebRequest request = UnityWebRequest.Get(apiUrl + api))
            {

                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    if (type.Equals("userInfo"))
                    {
                        UserInfoSuccess(request.downloadHandler.text);
                    }else if (type.Equals("ranking"))
                    {  
                        GetRankingSuccess(request.downloadHandler.text);
                    }
                    //성공
                    request.Dispose(); // 메모리 누수로 해제 하기
                }
                else
                {
                    //에러
                    if (type.Equals("userInfo"))
                    {
                        Debug.Log("마이페이지 조회 실패");
                    }else if (type.Equals("ranking"))
                    {
                        Debug.Log("랭킹 조회 실패");
                    }
                    request.Dispose(); // 메모리 누수로 해제 하기
                }
                request.Dispose();
            }
            
        }

        public IEnumerator ApiPatchRequest(string api,string input, string type)
        {
            using (UnityWebRequest request = UnityWebRequest.Put(apiUrl + api, input))
            {
                request.method = "PATCH"; // patch로 전송한다고 한다.
                byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(input);
                request.uploadHandler = new UploadHandlerRaw(jsonToSend);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();


                if (request.result == UnityWebRequest.Result.Success)
                {
                    if (type.Equals("mypage"))
                    {
                        SetMyInfo();
                    }

                    request.Dispose(); // 메모리 누수로 해제 하기
                }
                else
                {
                    if (type.Equals("mypage"))
                    {
                        //Debug.Log(request.error);
                        Debug.LogError("마이페이지 수정 안됨");
                    }
                    request.Dispose(); // 메모리 누수로 해제 하기
                }
                request.Dispose();
            }

        }

        #endregion


        #region websocket 요청

        //서버 연결
        public void Connect()
        {
            try
            {
                Debug.Log("소캣 연결");
                wSocket = new WebSocketSharp.WebSocket(socketIP);
                wSocket.OnMessage += RecvMessage; // 메시지가 수신될때 어떤 함수를 사용하는지 지정
                wSocket.OnClose += CloseConnect;
                wSocket.Connect();
            }
            catch
            {
                Debug.Log("소캣 실패");
            }
        }
        
        //연결 종료
        public void DisconnectServer()
        {
            try
            {
                Debug.Log("연결 끊음");
                if (wSocket == null) return;
                if(wSocket.IsAlive)
                    wSocket.Close();
            }
            catch (Exception e)
            {
                Debug.Log(e.ToString());
            }
        }

        private void CloseConnect(object sender, CloseEventArgs e)
        {
            Debug.Log("연결 끊을게");
            DisconnectServer();
        }
        
        //서버에서 데이터 수신
        public void RecvMessage(object sender, MessageEventArgs e)
        {
            Debug.Log("받은 데이터"+e.Data);
            if (e.Data.Equals("ok"))
            {
                _lobbySwitch = true;
            }
            else
            {
                string[] datas = e.Data.Split("_");
                //queueLength, position, estimatedWaitTime
                string queueLength = datas[1];
                string position = datas[2];
                string estimatedWaitTime = datas[3];
                _watingText = $"현재 대기 인원{queueLength}, 예상시간{estimatedWaitTime} 초";
                _waitingSwitch = true;
            }
        }

        public void OnApplicationQuit()
        {
            Debug.Log("끊기다~");
            DisconnectServer();
        }

        #endregion
        int SetToken(string _input)
        {
            // 로그아웃시 토큰 초기화
            if (_input == null)
            {
                token = null;
                return 0;
            }

            // 로그인시 토큰 설정
            string[] temp = _input.Split('"');

            if (temp.Length != 5 || temp[1] != "token")
                ErrorCheck(-1001); // 토큰 형식 에러

            token = temp[3];
            return 0;
        }

        public void Test()
        {
            Debug.Log("요청 테스트");
        }
        int ErrorCheck(int _code)
        {
            if (_code > 0) return 0;
            else if (_code == -1000) Debug.LogError(_code + ", Internet Connect Error");
            else if (_code == -1001) Debug.LogError(_code + ", Occur token type Error");
            else if (_code == -1002) Debug.LogError(_code + ", Category type Error");
            else if (_code == -1003) Debug.LogError(_code + ", Item type Error");
            else Debug.LogError(_code + ", Undefined Error");
            return _code;
        }

        int ErrorCheck(int _code, string _funcName)
        {
            if (_code > 0) return 0;
            else if (_code == -400) Debug.LogError(_code + ", Invalid request in " + _funcName);
            else if (_code == -401) Debug.LogError(_code + ", Unauthorized in " + _funcName);
            else if (_code == -404) Debug.LogError(_code + ", not found in " + _funcName);
            else if (_code == -500) Debug.LogError(_code + ", Internal Server Error in " + _funcName);
            else Debug.LogError(_code + ", Undefined Error");
            return _code;
        }
    }
}