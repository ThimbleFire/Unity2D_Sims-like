using System.Collections.Generic;

public class Toilet : Facility
{
    public override bool Interact( ref List<float> impulse ) {
        impulse[0] += Entity.ImpulseMax / 7;
        return impulse[0] >= Entity.ImpulseMax ? true : false;
    }
}
