using System;
using UnityEngine;

public class GameTime : MonoBehaviour
{
    public delegate void OnTickHandler();
    public static event OnTickHandler OnTck;

    public float timer = 0.0f;
    
    // interval in seconds
    private readonly float interval = /*60.0f*/ 5.0f;

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

        timer += Time.deltaTime;

        if (timer >= interval) {
            timer -= interval;
            OnTck?.Invoke();
        }
    }
}
