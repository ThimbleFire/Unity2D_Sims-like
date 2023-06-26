using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIToggle : MonoBehaviour
{
    public Animation animation;

    public void Enable() => animation.Play( "OnEnable" );
    public void Disable() => animation.Play( "OnDisable" );
}
