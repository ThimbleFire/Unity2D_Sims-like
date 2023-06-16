using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AlwaysEast;

public class Navigator : Entity
{
    private readonly Vector3 offset = new Vector3(0.04f, 0.04f);
    public float moveAcrossBoardSpeed = 0.08f;

    private void Update() {

        if (_chain == null)
            return;

        if (_chain.Count == 0)
            return;

        StepFrame();
    }
    
    private void StepFrame()
    {
        // Calculate position after moving
        Vector3 _stepDestination = _chain[0].worldPosition + offset;
        Vector3 positionAfterMoving = Vector3.MoveTowards(transform.position, _stepDestination, moveAcrossBoardSpeed * Time.deltaTime);
        
        // Move the gameobject
        transform.position = positionAfterMoving;
        currentBehaviour = CurrentBehaviour.Walking; // unless we're interacting
        UpdateAnimator( Coordinates - _chain[0].coordinate );

        // If the distance between the unit and the stepDestination is less than or equal to zero, we have arrived
        bool unitHasArrivedAtDestination = Vector2.Distance(transform.position, _stepDestination) <= 0.0f;
        if (unitHasArrivedAtDestination) {
            OnTileChanged();
        }
    }
    
    protected virtual void OnTileChanged() {
        // Unoccupy last coordinates
        Pathfind.Unoccupy(Coordinates);
        // Set the new coordinate at our current position
        Coordinates = _chain[0].coordinate;
        // Let the pathfinder know this tile is now occupied
        Pathfind.Occupy( Coordinates );
        // Remove the last chain since we're not where we used to be
        _chain.RemoveAt(0);
        // If we've arrived at our destination
        if (_chain.Count <= 0) {
            OnArrival();
            return;
        }
    }

    protected virtual void OnArrival() {
        _chain.Clear();
        animator.SetBool("Moving", false);
        currentBehaviour = CurrentBehaviour.Idling; // unless we're interacting

        Action();
    }

    // primarily used for spawning entities
    //protected void MoveUnitTo(Vector3Int coordinates) {
    //    transform.position = coordinates + offset;
    //    _coordinates = coordinates;
    //}
}
