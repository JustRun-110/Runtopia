using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace garden
{
    public enum SMBTiming { OnEnter, OnExit, OnUpdate, OnEnd }
    public class SMBEvent : StateMachineBehaviour // ~~ SMB_EVENT
    {
        /// <summary>
        /// 이벤트에 대한 정보 ~~ SMBEVENT
        /// </summary>
        [System.Serializable]
        public class SMBEventInfo
        {
            public bool fired; //이벤트 발생 여부 true:대기중 false:시작
            public string eventName; //이벤트 이름
            public SMBTiming timing; //이벤트가 발생할 시간
            public float onUpdateFrame; //이벤트 발생시킬 프레임
        }

        [SerializeField] private int totalFrames;// 애니메이션 클립의 프레임 수
        [SerializeField] private int currentFrames;// 현재 애니메이션 프레임의 숫자 -> 진행도 확인을 위함 
        [SerializeField] private float normalizedTime; // 애니메이션 시간 0~1 정규화
        [SerializeField] private float normalizedTimeUncapped; //  애니메이션 시간 0~1 정규화, 애니베이션 반복시 넘어감
        [SerializeField] private string motionTime = ""; // 애니메이터 매개변수 이름

        public List<SMBEventInfo> events = new List<SMBEventInfo>();

        private bool hasParam; // 모션 시간 매개변수가 존재하는지 여부
        private SMBEventCurrator eventCurrator;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            hasParam = HasParameter(animator, motionTime);
            eventCurrator = animator.GetComponent<SMBEventCurrator>();
            totalFrames = GetTotalFrames(animator, layerIndex);

            normalizedTimeUncapped = stateInfo.normalizedTime;
            normalizedTime = hasParam ? animator.GetFloat(motionTime) : GetNormalizedTime(stateInfo);
            currentFrames = GetCurrentFrame(totalFrames, normalizedTime);

            if (eventCurrator != null)
            {
                //이벤트 실행하기
                foreach (SMBEventInfo smbEvent in events)
                {
                    smbEvent.fired = false;

                    if (smbEvent.timing == SMBTiming.OnEnter)
                    {
                        smbEvent.fired = true;
                        eventCurrator.unityEvent.Invoke(smbEvent.eventName);
                    }
                }
            }
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            normalizedTimeUncapped = stateInfo.normalizedTime;
            normalizedTime = hasParam ? animator.GetFloat(motionTime) : GetNormalizedTime(stateInfo);
            currentFrames = GetCurrentFrame(totalFrames, normalizedTime);

            //이벤트가 없으면 넘어가기
            if (eventCurrator != null)
            {
                foreach (SMBEventInfo smbEvent in events)
                {
                    //이벤트가 실행됬다면
                    if (!smbEvent.fired)
                    {
                        //OnUpdate라면
                        if (smbEvent.timing == SMBTiming.OnUpdate)
                        {
                            if (currentFrames >= smbEvent.onUpdateFrame)
                            {
                                smbEvent.fired = true;
                                eventCurrator.unityEvent.Invoke(smbEvent.eventName);
                            }
                        }
                        //OnEnd라면
                        else if (smbEvent.timing == SMBTiming.OnEnd)
                        {
                            if (currentFrames >= totalFrames)
                            {
                                smbEvent.fired = true;
                                eventCurrator.unityEvent.Invoke(smbEvent.eventName);
                            }
                        }
                    }
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (eventCurrator != null)
            {
                foreach (SMBEventInfo smbEvent in events)
                {
                    if (smbEvent.timing == SMBTiming.OnExit)
                    {
                        smbEvent.fired = true;
                        eventCurrator.unityEvent.Invoke(smbEvent.eventName);
                    }
                }
            }

        }
        private bool HasParameter(Animator animator, string parameterName)
        {
            //parameterName이 null이거나 공백이면 false 리턴
            if (string.IsNullOrEmpty(parameterName) || string.IsNullOrWhiteSpace(parameterName))
            {
                return false;
            }

            //값이 있다면 같은 이름의 파라미터가 있는지 확인
            foreach (AnimatorControllerParameter parameter in animator.parameters)
            {
                if (parameter.name == parameterName)
                {
                    Debug.Log("parameter:" + parameterName + "찾았습니다.");
                    return true;
                }
            }

            // 이름과 매치되는게 없다면 false 리턴
            return false;
        }
        /// <summary>
        /// 프레임 수를 리턴합니다
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        private int GetTotalFrames(Animator animator, int layerIndex)
        {
            //다음 애니메이션 클립을 받아온다.
            AnimatorClipInfo[] clipInfos = animator.GetNextAnimatorClipInfo(layerIndex);

            //받아온 애니메이션 클립이 없다면
            if (clipInfos.Length == 0)
            {
                //현재 애니메이션 클립을 받아온다
                clipInfos = animator.GetCurrentAnimatorClipInfo(layerIndex);
            }

            Debug.Log(clipInfos.ToString());

            if (clipInfos.Length != 0)
            {
                AnimationClip clip = clipInfos[0].clip;//첫번째 애니메이션 클립을 받는다
                return Mathf.RoundToInt(clip.length * clip.frameRate);
            }

            return 0;
        }
        /// <summary>
        /// 정규화 시간을 리턴합니다
        /// </summary>
        /// <param name="stateInfo"></param>
        /// <returns></returns>
        private float GetNormalizedTime(AnimatorStateInfo stateInfo)
        {
            return stateInfo.normalizedTime > 1 ? 1 : stateInfo.normalizedTime;
        }
        /// <summary>
        /// 현재 프레임을 리턴합니다 1.12->1리턴
        /// </summary>
        /// <param name="totalFrame"></param>
        /// <param name="normalizedTime"></param>
        /// <returns></returns>
        private int GetCurrentFrame(int totalFrame, float normalizedTime)
        {
            return Mathf.RoundToInt(totalFrame * normalizedTime);
        }
    }
}