using AlwaysEast;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public delegate void OnBehaviourChangeHandler( Behaviour currentBehaviour, Behaviour lastBehaviour );
    public static event OnBehaviourChangeHandler OnBehaviourChange;

    public enum ResponsibleDelegate {
        Captain, Medic, Gunnery, Navigator, Engineer
    };
    public enum Behaviour {
        WonderingWhatToDo,
        Walking,
        UsingFacility,
        DoingJob
    };
    public enum Impulse {
        Bladder, Hunger, Water
    };

    public ImpulseMeter impulseMeter;

    public const float ImpulseMax = 700.0f;

    public string Name { get; set; }
    public List<float> Impulses  = new List<float>( new float[3] { ImpulseMax, ImpulseMax, ImpulseMax } );
    public bool[] Responsibilities { get; set; } = new bool[5] { false, false, false, false, false };

    protected Vector3Int Coordinates { get; set; }
    public Behaviour CurrentBehaviour = Behaviour.WonderingWhatToDo;
    protected Behaviour LastBehaviour { get; set; } = Behaviour.WonderingWhatToDo;
    protected byte BodyHeat { get; set; } = 37;
    protected List<Node> _chain = new List<Node>();

    private Facility facilityOfInterest;
    protected Animator animator;

    private void Awake() {
        Name = Helper.GetRandomName;
        animator = GetComponent<Animator>();
        Coordinates = new Vector3Int( 5, 4, 0 );    // Need to figure out a way to get coordinates from position on placement
        GameTime.OnTck += GameTime_OnTck;
    }    

    public void GameTime_OnTck() {

        //bladder
        Impulses[0] -= 1 + Impulses[2] / ImpulseMax;
        //hunger
        Impulses[1] -= 2 - Impulses[1] / ImpulseMax;
        //hydration
        Impulses[2] -= 1 + ( BodyHeat - 37 ) / 10.0f;

        HandleBehaviours();
    }

    private void HandleBehaviours() {

        if( CurrentBehaviour == Behaviour.WonderingWhatToDo || CurrentBehaviour == Behaviour.DoingJob)
        {
            impulseMeter.HideMeter();

            bool result = HandleImpulses();

            if( result )
                return;

            result = HandleResponsibilities();

            if( result )
                return;
        }

        if( CurrentBehaviour == Behaviour.UsingFacility ) {
            facilityOfInterest.InteractStart();

            UpdateAnimator( Coordinates - facilityOfInterest.Coordinates );

            if( facilityOfInterest.IsImpulse ) {
                Impulses[( byte )facilityOfInterest.Type] = facilityOfInterest.Interact( Impulses[( byte )facilityOfInterest.Type] );
                impulseMeter.SetMeter( Impulses[( byte )facilityOfInterest.Type] );
                if( Impulses[( byte )facilityOfInterest.Type] >= ImpulseMax ) {
                    CurrentBehaviour = Behaviour.WonderingWhatToDo;
                    facilityOfInterest.InteractEnd();
                }
            }
        }
        if( CurrentBehaviour == Behaviour.DoingJob ) {
            facilityOfInterest.InteractStart();

            UpdateAnimator( Coordinates - facilityOfInterest.Coordinates );
            facilityOfInterest.Interact( 0.0f );
        }

        // not currently used...
        if( LastBehaviour != CurrentBehaviour ) {
            OnBehaviourChange?.Invoke( CurrentBehaviour, LastBehaviour );
            LastBehaviour = CurrentBehaviour;
        }
    }

    private bool HandleImpulses() {
        for( int i = 0; i < Impulses.Count; i++ ) {
            if( Impulses[i] <= ImpulseMax / 7 ) {
                // Get the coordinates of a facility that will raise the impulse
                facilityOfInterest = Facilities.Get( ( Facility.EType )i );
                //add a check for no facility found
                _chain = Pathfind.GetPath( Coordinates, facilityOfInterest.Coordinates, facilityOfInterest.Size, out Pathfind.Report report );
                if( report == Pathfind.Report.DESTINATION_IS_OCCUPIED_AND_IS_ADJACENT_TO_PLAYER_CHARACTER )
                    CurrentBehaviour = Behaviour.UsingFacility;
                return true;
            }
        }

        return false;
    }

    private bool HandleResponsibilities() {
        // If they're the captain...
        if( Responsibilities[0] ) {

            facilityOfInterest = Facilities.Get( Facility.EType.CaptainsChair );
            _chain = Pathfind.GetPath( Coordinates, facilityOfInterest.Coordinates, facilityOfInterest.Size, out Pathfind.Report report );
            if( report == Pathfind.Report.DESTINATION_IS_OCCUPIED_AND_IS_ADJACENT_TO_PLAYER_CHARACTER ) {
                CurrentBehaviour = Behaviour.DoingJob;
                return true;
            }
        }
        return false;
    }

    protected void UpdateAnimator(Vector3Int dir) {
        if(dir == Vector3Int.zero)
            return;
            
        animator.SetFloat("x", dir.x);
        animator.SetFloat("y", dir.y);
        animator.SetBool( "Moving", CurrentBehaviour == Behaviour.Walking );
    }
}
