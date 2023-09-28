using UnityEngine;
using UnityEditor;

public class Checkpoint : MonoBehaviour
{
    [Tooltip("This checkpoint will only activate if its order is greater than or equal to a previously reached checkpoint.")]
    public uint Order = 0;
}