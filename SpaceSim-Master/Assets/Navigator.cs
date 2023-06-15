using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : Impulses
{
    private readonly Vector3 offset = new Vector3(0.04f, 0.04f);

    protected List<AlwaysEast.Node> chain = new List<AlwaysEast.Node>();
    private bool walking = false;
    private int dashElapsedFrames = 0;
    private const int dashFrameCount = 800;


    public Vector3Int cellPosition { get {
            return new Vector3Int(
                        Mathf.FloorToInt( transform.position.x / 0.08f ),
                        Mathf.FloorToInt( transform.position.y / 0.08f ),
                        0 ); } }

    private void Update() {

        if( chain.Count == 0 )
            return;

        if(walking == false ) {
            walking = true;
        }

        float interpolationRatio = (float)dashElapsedFrames / dashFrameCount;
        Vector3 newPosition = Vector3.Lerp(transform.position, chain[0].worldPosition + offset, interpolationRatio);
        transform.position = newPosition;

        dashElapsedFrames = ( dashElapsedFrames + 1 ) % ( dashFrameCount + 1 );

        if( dashElapsedFrames == 0 ) {
            walking = false;
            chain.RemoveAt( 0 );
        }
    }
}
