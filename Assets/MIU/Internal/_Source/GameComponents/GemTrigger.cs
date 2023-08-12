using UnityEngine;
using MIU;
using System;

public class GemTrigger : MonoBehaviour
{
    public enum Comparison
    {
        LessThan,
        Equal,
        GreaterThanOrEqual,
    }

    public Comparison ComparisonMode;
    public int Threshold;
}