using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public float dragSpeed = 0.5f;
    private Vector3 oldPos = Vector3.zero;
    private Vector3 panOrigin = Vector3.zero;

    public void SetFocus( Transform target ) {
        transform.position = target.transform.position + Vector3.back * 10;
    }

    private void Update() {
        if( Input.GetMouseButtonDown( 2 ) ) {
            oldPos = transform.position;
            panOrigin = Camera.main.ScreenToViewportPoint( Input.mousePosition );                    //Get the ScreenVector the mouse clicked
        }

        if( Input.GetMouseButton( 2 ) ) {
            Vector3 pos = Camera.main.ScreenToViewportPoint( Input.mousePosition ) - panOrigin;    //Get the difference between where the mouse clicked and where it moved
            transform.position = oldPos + -pos * dragSpeed;                                        //Move the position of the camera to simulate a drag, speed * 10 for screen to worldspace conversion
        }
    }
}
