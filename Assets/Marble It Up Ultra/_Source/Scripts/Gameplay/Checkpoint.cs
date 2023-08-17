using UnityEngine;
using UnityEditor;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("Checkpoints will only be active when the new checkpoint order is greater or equal to the last checkpoint's Order.")]
    public uint Order = 0;
}