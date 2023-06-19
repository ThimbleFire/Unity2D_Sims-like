using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : Facility
{
    public override float Interact( float impulse ) {
        return impulse + Entity.ImpulseMax / 7;
    }
}
