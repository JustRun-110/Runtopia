using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CimbModifier : MonoBehaviour
{
    [SerializeField] private bool climbable;
    public bool Climbable
    {
        get => climbable;
        set => climbable = value;
    }
}
