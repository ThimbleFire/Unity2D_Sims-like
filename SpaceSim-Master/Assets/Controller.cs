using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public delegate void OnWDownHandler();
    public delegate void OnSDownHandler();
    public delegate void OnADownHandler();
    public delegate void OnDDownHandler();
    public delegate void OnRDownHandler();
    public delegate void OnFDownHandler();

    public static event OnWDownHandler OnWDown;
    public static event OnSDownHandler OnSDown;
    public static event OnADownHandler OnADown;
    public static event OnDDownHandler OnDDown;
    public static event OnRDownHandler OnRDown;
    public static event OnFDownHandler OnFDown;

    private void Update() {
        if( KeyDownW ) OnWDown?.Invoke();
        if( KeyDownS ) OnSDown?.Invoke();
        if( KeyDownA ) OnADown?.Invoke();
        if( KeyDownD ) OnDDown?.Invoke();
        if( KeyDownR ) OnRDown?.Invoke();
        if( KeyDownF ) OnFDown?.Invoke();
    }

    private bool KeyDownW { get { return Input.GetKeyDown( KeyCode.W ); } }
    private bool KeyDownS { get { return Input.GetKeyDown( KeyCode.S ); } }
    private bool KeyDownA { get { return Input.GetKeyDown( KeyCode.A ); } }
    private bool KeyDownD { get { return Input.GetKeyDown( KeyCode.D ); } }
    private bool KeyDownR { get { return Input.GetKeyDown( KeyCode.R ); } }
    private bool KeyDownF { get { return Input.GetKeyDown( KeyCode.F ); } }

}
