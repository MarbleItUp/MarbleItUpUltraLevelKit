using MIU;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Profiling;

[DisallowMultipleComponent]
public class ElevatorMover : MonoBehaviour
{
    [NonSerialized]
    public ushort Id;

    // Increment on every mover change. TriCache doesn't recalc triangles unless
    // this doesn't match last known value.
    [NonSerialized]
    public static int InvalidationToken = 0;

    // We set this to 0 to stop platform movement for a while.
    [NonSerialized]
    public float StopTime = 0.0f;

    // How is our movement driven?
    public enum Mode
    {
        Elevator,
        Spline,
        Loop
    };

    [Header("Behavior")]

    [Tooltip("Set the behavior for this moving object.\n\nElevator: The object will move back and forth between its start and end point.\n\nSpline: The object will follow a linear spline path.\n\nLoop: The object will teleport back to its start point once reaching its end point.")]
    public Mode mode;
    [Tooltip("Offsets the start of this object's cycle by this many seconds.")]
    public float StartOffsetTime = 0.0f;
    [Tooltip("If true, the object will only begin moving once the marble touches it.")]
    public bool WaitForTouched;
    [Tooltip("On startup, move first or pause first?")]
    public bool moveFirst = false;

    [Header("Movement")]

    [Tooltip("How much this object will be positionally translated.")]
    public Vector3 delta = Vector3.zero;
    [Tooltip("How much this object will be rotated.")]
    public Vector3 deltaRotation = Vector3.zero;
    [Tooltip("The amount of time this object will remain stationary once reaching its start or end point (in seconds).")]
    public float pauseTime = 2.0f;
    [Tooltip("The amount of time this object will take to move between its start and end point (in seconds).")]
    public float moveTime = 4.0f;

    [Header("Spline")]

    public float splineSpeed = 0.2f;
    [Tooltip("If set, platform stays in it's orientation.")]
    public bool KeepOrientation = false;
    public GameObject splineGo;
}