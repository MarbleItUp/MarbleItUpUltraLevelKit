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
    [Tooltip("Moving Platforms and Checkpoints with this script attached will only activate once this condition is met.")]
    public int Threshold;
}