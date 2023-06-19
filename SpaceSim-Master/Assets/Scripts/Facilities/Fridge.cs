using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fridge : Facility
{
    public override bool Interact( ref List<float> impulse ) {
        impulse[1] += Entity.ImpulseMax / 7;
        return impulse[1] >= Entity.ImpulseMax ? true : false;
    }
}
