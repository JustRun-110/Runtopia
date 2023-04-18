using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SMBEventCurrator : MonoBehaviour
{
    [SerializeField]
    private bool debug = false;//로그 출력 할지에 대한 여부
    [SerializeField] private UnityEvent<string> SMBEvent = new UnityEvent<string>();

    public UnityEvent<string> unityEvent { get => SMBEvent; }

    private void Awake()
    {
        SMBEvent.AddListener(OnSMBEvent);
    }
    private void OnSMBEvent(string eventName)
    {
        if (debug)
        {
            Debug.Log("Currator:" + eventName);
        }
    }
}
