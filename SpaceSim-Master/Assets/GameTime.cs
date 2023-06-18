using System;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public delegate void OnTickHandler();
    public static event OnTickHandler OnTck;

    private float Timer {get; set;} = 0.0f;
    
    // interval in seconds
    private readonly float interval = 0.6f;

    private static bool active = false;

    public static void ClockStart() => active = true;
    public static void ClockStop() => active = false;
    //public static void ClockReset() => timer = 0.0f;

    private void Awake()
    {
        ClockStart();
    }

    public void Update() {

        if (active == false)
            return;

        Timer += Time.deltaTime;

        if (Timer >= interval) {
            Timer -= interval;
            OnTck?.Invoke();
        }
    }
}
