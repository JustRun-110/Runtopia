using Photon.Chat;
using Photon.Pun;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

namespace sjb
{
    public class ChatManager : MonoBehaviour, IChatClientListener
    {
        [SerializeField]
        [Tooltip("채팅 패널")]
        GameObject chatPanel;

        ChatClient chatClient;
        private string userName;
        private string channelName;
        bool isConnect;

        [Tooltip("입력창")]
        public TMP_InputField inputField;
        
        [Tooltip("출력창")]
        public Transform chatScroll;

        [Tooltip("출력 텍스트 entry")] 
        public GameObject chatEntry;

        private bool isChatFocused;
        private void Start()
        {
            //연결
            Application.runInBackground= true;
            userName = PhotonNetwork.NickName;
            channelName = "Square001";
            ChatConnect();
        }

        private void Update()
        {
            if (isConnect)
            {
                chatClient.Service();
                OnKeyEvent();
            }
        }
        public void OnKeyEvent()
        {
            if (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter) || Input.GetKeyDown("enter"))
            {
                Input_OnEndEdit();
            }
        }

        public void Input_OnEndEdit()
        {
            if(chatClient.State== ChatState.ConnectedToFrontEnd && inputField.text.Length > 0)
            {
                chatClient.PublishMessage(channelName, inputField.text);
                inputField.text = "";
                inputField.ActivateInputField();
            }
        }
        public void AddChat(string lineString, bool isMine = false)
        {
            GameObject chat = Instantiate(chatEntry, chatScroll);
            chat.GetComponent<TMP_Text>().text = lineString;

        }

        public void OnApplicationQuit()
        {
            StartCoroutine("clientDisconnect");
        }

        IEnumerator clientDisconnect()
        {
            chatClient.PublishMessage(channelName, string.Format("{0}가 나감", userName));
            yield return new WaitForSeconds(1f);
            if(chatClient != null) {
                chatClient.Disconnect();
            }
        }
        public void ChatConnect()
        {
            chatClient = new ChatClient(this);
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, PhotonNetwork.AppVersion, new AuthenticationValues(userName));
            isConnect = true;
        }


        public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
        {
            if(level == ExitGames.Client.Photon.DebugLevel.ERROR)
            {
                Debug.LogError(message);
            }
            else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
            {
                Debug.LogError(message);
            }
            else
            {
                Debug.Log(message);
            }
        }

        public void OnChatStateChange(ChatState state)
        {
            Debug.Log("채팅상태 바뀜 = " + state);
        }

        public void OnConnected()
        {
            //연결 되고 나서
            chatClient.Subscribe(new string[] { channelName },-1);
        }

        public void OnDisconnected()
        {
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            //메시지를 받음
            for(int i = 0; i < messages.Length; i++)
            {
                bool isMine = userName == senders[i];
                AddChat(string.Format("[{0}] {1}", senders[i], messages[i].ToString()),isMine);
            }

        }

        public void OnPrivateMessage(string sender, object message, string channelName)
        {
        }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
        {
            Debug.Log($"status:{user} is {status} msg {message}");
        }

        public void OnSubscribed(string[] channels, bool[] results)
        {
            if (chatClient.State == ChatState.ConnectedToFrontEnd)
            {
                chatClient.PublishMessage(channelName, string.Format("{0}가 들어옴", userName));
                //구독 되었을때 
            }
        }

        public void OnUnsubscribed(string[] channels)
        {
            AddChat(string.Format("{0}가 나감", userName));

        }

        public void OnUserSubscribed(string channel, string user)
        {
            throw new System.NotImplementedException();
        }

        public void OnUserUnsubscribed(string channel, string user)
        {
            throw new System.NotImplementedException();
        }
    }


}
