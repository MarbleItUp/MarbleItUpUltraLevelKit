using System;
using UnityEngine;

public class TutorialMessage : MonoBehaviour
{

    [Tooltip("The message that will display when the marble is inside the collider volume.")]
    [TextArea]
    public string message = "Hello, World";

    [Tooltip("If true, this message will only display once.")]
    public bool ShowOnce = false;
}
