using AlwaysEast;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public enum CurrentBehaviour {
        Eating, Drinking, Playing, Defecating, Working, Walking, Idling
    }
    public enum Impulses {
        Hunger, Water, Happiness, Bladder, Oxygen
    }
    public enum Solutions {
        Fridge, Sink, Play, Toilet, LifeSupport, Chair
    }
    
    public List<byte> impulses = new List<byte>(new byte[5] { 255, 255, 255, 255, 255 });

    public Vector3Int Coordinates { get; set; }

    public BoardManager boardManager;
    
    public CurrentBehaviour currentBehaviour = CurrentBehaviour.Idling;
    
    // Chain is the list of tiles returned by the pathfinder
    protected List<Node> _chain = new List<Node>();

    protected Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        Coordinates = new Vector3Int( 5, 4, 0 );
        GameTime.OnTck += GameTime_OnTck;
    }

    private void GameTime_OnTck() {

        impulses[( int )Impulses.Bladder] -= 1;
        impulses[( int )Impulses.Hunger] -= 1;
        impulses[( int )Impulses.Water] -= 1;

        if( currentBehaviour == CurrentBehaviour.Idling )
            Action();

    }

    protected void UpdateAnimator(Vector3Int dir) {
        if(dir == Vector3Int.zero)
            return;
            
        animator.SetFloat("x", dir.x);
        animator.SetFloat("y", dir.y);
        animator.SetBool("Moving", currentBehaviour == CurrentBehaviour.Walking);
    }
    
    protected virtual void Action() {

        /*
            BoardManager calls Action the entity is  idle
            Action calculates what the unit will do and assigns a chain
            Navigate then performs that chain
        */
        Vector3Int facilityPosition = Vector3Int.zero;
        Pathfind.Report report;

        for(int i = 0; i < impulses.Count; i++) {

            if(impulses[i] <= 30) {

                facilityPosition = boardManager.FindFacility((Solutions)i);
                _chain = Pathfind.GetPath(Coordinates, facilityPosition, false, out report);
                switch( report ) {
                    case Pathfind.Report.OK:
                    case Pathfind.Report.ATTEMPTING_TO_MOVE_ON_SELF:
                    case Pathfind.Report.NO_ADJACENT_NEIGHBOURS_TO_START_NODE:
                        break;
                    case Pathfind.Report.DESTINATION_IS_OCCUPIED_AND_IS_ADJACENT_TO_PLAYER_CHARACTER:
                        impulses[i] = 255;
                        currentBehaviour = CurrentBehaviour.Idling;
                        UpdateAnimator( Coordinates - facilityPosition );
                        break;
                }
                return;
            }

            facilityPosition = boardManager.FindFacility(Solutions.Chair);
            _chain = Pathfind.GetPath( Coordinates, facilityPosition, false, out report );
        }
    }      
}
