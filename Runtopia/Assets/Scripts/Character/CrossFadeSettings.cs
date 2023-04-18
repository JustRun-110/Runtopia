using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CrossFadeSettings
{
    public string stateName;
    [Min(-1)]public int layer;
    [Min(0)] public float timeOffset;
    [Min(0)] public float transitionDuration;
}
