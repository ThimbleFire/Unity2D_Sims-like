using AlwaysEast;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public string Name { get; set; }
    public List<Core.Responsibility> Jobs { get; set: }
    private List<byte> impulses = new List<byte>(new byte[5] { 255, 255, 255, 255, 255 });
    public Vector3Int Coordinates { get; set; }

    public BoardManager boardManager;
    public ImpulseMeter impulseMeter;
    
    public delegate void OnBehaviourChangeHandler(Core.CurrentBehaviour currentBehaviour, Core.CurrentBehaviour lastBehaviour);
    public static event OnBehaviourChangeHandler OnBehaviourChange;
    
    public Core.CurrentBehaviour lastBehaviour = Core.CurrentBehaviour.Idling;
    public Core.CurrentBehaviour currentBehaviour = Core.CurrentBehaviour.Idling;
    
    protected List<Node> _chain = new List<Node>();

    protected Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
        Coordinates = new Vector3Int( 5, 4, 0 );
        GameTime.OnTck += GameTime_OnTck;
    }

    private void GameTime_OnTck() {

        impulses[( int )Impulses.Bladder] -= (byte)(impulses[(int)Impulses.Bladder] > 5 ? 1 : 0);
        impulses[( int )Impulses.Hunger] -= ( byte )( impulses[( int )Impulses.Hunger] > 5 ? 1 : 0 );
        impulses[( int )Impulses.Water] -= ( byte )( impulses[( int )Impulses.Water] > 5 ? 1 : 0 );

        switch( currentBehaviour ) {
            case Core.CurrentBehaviour.Eating:
            case Core.CurrentBehaviour.Drinking:
            case Core.CurrentBehaviour.Playing:
            case Core.CurrentBehaviour.Defecating:
                impulses[(byte)currentBehaviour] += 35;
                impulseMeter.SetMeter( impulses[( byte )currentBehaviour] );
                IdleReadyCheck();
                break;
            case Core.CurrentBehaviour.Working:
                
                break;
            case Core.CurrentBehaviour.Idling:
                impulseMeter.HideMeter();
                Action();
                break;
            default:
                break;
        }
        
        if(lastBehaviour != currentBehaviour) {

            OnBehaviourChange?.Invoke(currentBehaviour, lastBehaviour)
            lastBehaviour = currentBehaviour;
        }
    }

    /// <summary> If the NPC has filled its impulse, it'll return to a state where it can recieve a new behaviour. </summary>
    private void IdleReadyCheck() {
        if( impulses[( byte )currentBehaviour] >= 220 ) {
            currentBehaviour = Core.CurrentBehaviour.Idling;
        }
    }

    protected void UpdateAnimator(Vector3Int dir) {
        if(dir == Vector3Int.zero)
            return;
            
        animator.SetFloat("x", dir.x);
        animator.SetFloat("y", dir.y);
        animator.SetBool("Moving", currentBehaviour == Core.CurrentBehaviour.Walking);
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
                        currentBehaviour = (Core.CurrentBehaviour)i;
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
