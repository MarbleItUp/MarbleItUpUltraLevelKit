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
    [Tooltip("Moving Platforms with this script attached will only move once this condition is met.")]
    public int Threshold;
}