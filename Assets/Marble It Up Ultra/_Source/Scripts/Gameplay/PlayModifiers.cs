using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class PlayModifiers : MonoBehaviour {

    [Tooltip("Scalar affecting the force of gravity on the marble.\n\n(Default: 1)")]
    public float GravityMult = 1;
    [Tooltip("Scalar affecting the marble's jump impulse.\n\n(Default: 1)")]
    public float JumpForceMult = 1;
    [Tooltip("Scalar affecting how much the marble bounces.\n\n(Default: 1)")]
    public float BounceMult = 1;
    [Tooltip("Scalar affecting the marble's friction.\n\n(Default: 1)")]
    public float FrictionMult = 1;
    [Tooltip("Scalar affecting the marble's movement on the ground.\n\nX affects left/right movement.\nY affects forward/backward movement.\n\n(Default: 1)")]
    public Vector2 RollForceMult = Vector2.one;
    [Tooltip("Scalar affecting the marble's movement in the air.\n\nX affects left/right movement.\nY affects forward/backward movement.\n\n(Default: 1)")]
    public Vector2 AirForceMult = Vector2.one;
    [Tooltip("Scalar affecting the marble's size.\n\n(Default: 1)")]
    public float ScaleMult = 1;
    [Tooltip("Toggle whether or not the marble can use the multiplayer blast ability.\n\n(Default: false)")]
    public bool CanBlast = false;
    [Tooltip("The number of times the marble can jump in mid-air.\n\n(Default: 0)")]
    public int AirJumps = 0;

    public string ToJSON()
    {
        JSONClass json = new JSONClass();
        json.Add("gravity", new JSONData(GravityMult));
        json.Add("jumpmult", new JSONData(JumpForceMult));
        json.Add("bouncemult", new JSONData(BounceMult));
        json.Add("scalemult", new JSONData(ScaleMult));
        json.Add("frictionmult", new JSONData(FrictionMult));

        json.Add("rollX", new JSONData(RollForceMult.x));
        json.Add("rollY", new JSONData(RollForceMult.y));

        json.Add("airX", new JSONData(AirForceMult.x));
        json.Add("airY", new JSONData(AirForceMult.y));

        json.Add("canblast", new JSONData(CanBlast));
        json.Add("airjumps", new JSONData(AirJumps));

        return json.ToString();

    }
}
