using UnityEngine;

public class Navigator : Entity
{
    private Vector3 offset = new Vector3(0.04f, 0.04f);
    public float moveAcrossBoardSpeed = 0.08f;

    private void Update() {

        if( _chain == null )
            return;

        if( _chain.Count == 0 )
            return;

        StepFrame();
    }

    private void StepFrame() {
        // Calculate position after moving
        Vector3 _stepDestination = _chain[0].worldPosition + offset;
        Vector3 positionAfterMoving = Vector3.MoveTowards(transform.position, _stepDestination, moveAcrossBoardSpeed * Time.deltaTime);

        // Move the gameobject
        transform.position = positionAfterMoving;
        CurrentBehaviour = Behaviour.Walking; // unless we're interacting
        UpdateAnimator( Coordinates - _chain[0].coordinate );

        // If the distance between the unit and the stepDestination is less than or equal to zero, we have arrived
        bool unitHasArrivedAtDestination = Vector2.Distance(transform.position, _stepDestination) <= 0.0f;
        if( unitHasArrivedAtDestination ) {
            OnTileChanged();
        }
    }

    protected virtual void OnTileChanged() {
        // Set the new coordinate at our current position
        Coordinates = _chain[0].coordinate;
        // Remove the last chain since we're not where we used to be
        _chain.RemoveAt( 0 );
        // If we've arrived at our destination
        if( _chain.Count <= 0 )
            OnArrival();
    }

    protected override void OnArrival() {
        animator.SetBool( "Moving", false );
        base.OnArrival();
    }

    // primarily used for spawning entities
    public void SetCoordinates( Vector3Int coordinates ) {
        Coordinates = coordinates;
    }
}
