using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : Facility
{
    public override float Interact( float impulse ) {
        Debug.Log( "Toilet" );
        return impulse + Entity.ImpulseMax / 14;
    }
}
