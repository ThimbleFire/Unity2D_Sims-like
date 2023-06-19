using AlwaysEast;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
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

    public const float ImpulseMax = 350.0f;

    public string Name { get; set; }
    public List<float> Impulses  = new List<float>( new float[3] { ImpulseMax, ImpulseMax, ImpulseMax } );
    public bool[] Responsibilities = new bool[5] { false, false, false, false, false };

    protected Vector3Int Coordinates { get; set; }
    public Behaviour CurrentBehaviour = Behaviour.WonderingWhatToDo;
    protected byte BodyHeat { get; set; } = 37;
    protected List<Node> _chain = new List<Node>();

    private Facility facilityOfInterest;
    protected Animator animator;

    private void Awake() {
        Name = Helper.GetRandomName;
        gameObject.name = "NPC " + Name;
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

        if( CurrentBehaviour == Behaviour.Walking )
            return;

        if( CurrentBehaviour == Behaviour.WonderingWhatToDo || CurrentBehaviour == Behaviour.DoingJob) {
            GetTask();
            return;
        }

        if( facilityOfInterest == null )
            return;

        if( facilityOfInterest.IsImpulse ) {
            bool full = facilityOfInterest.Interact( ref Impulses );
            impulseMeter.SetMeter( Impulses[( byte )facilityOfInterest.Type] );
            if( full ) {
                CurrentBehaviour = Behaviour.WonderingWhatToDo;
                impulseMeter.HideMeter();
            }
        } else {
            facilityOfInterest.Interact();
        }
    }

    protected virtual void OnArrival() {

        UpdateAnimator( Coordinates - facilityOfInterest.Coordinates );
        CurrentBehaviour = facilityOfInterest.IsImpulse ? Behaviour.UsingFacility : Behaviour.DoingJob;
        //facilityOfInterest.InteractStart();
    }

    public void Restore() {

    }

    private void GetTask() {
        for( int i = 0; i < Impulses.Count; i++ ) {
            if( Impulses[i] <= ImpulseMax / 7 ) {
                facilityOfInterest = Facilities.Get( ( Facility.EType )i );
                _chain = Pathfind.GetPath( Coordinates, facilityOfInterest.Coordinates, facilityOfInterest.Size, out Pathfind.Report report );
                return;
            }
        }

        // I know we could use a for loop... However responsibilities entail more than just interacting with their facilities

        if( Responsibilities[0] ) {
            facilityOfInterest = Facilities.Get( Facility.EType.CaptainsChair );
            _chain = Pathfind.GetPath( Coordinates, facilityOfInterest.Coordinates, facilityOfInterest.Size, out Pathfind.Report report );
            return;
        }
        if( Responsibilities[1] ) {
            facilityOfInterest = Facilities.Get( Facility.EType.NPC );
            _chain = Pathfind.GetPath( Coordinates, facilityOfInterest.Coordinates, facilityOfInterest.Size, out Pathfind.Report report );
            return;
        }
        if( Responsibilities[2] ) {
            facilityOfInterest = Facilities.Get( Facility.EType.Gunnery );
            _chain = Pathfind.GetPath( Coordinates, facilityOfInterest.Coordinates, facilityOfInterest.Size, out Pathfind.Report report );
            return;
        }
        if( Responsibilities[3] ) {
            facilityOfInterest = Facilities.Get( Facility.EType.Engine );
            _chain = Pathfind.GetPath( Coordinates, facilityOfInterest.Coordinates, facilityOfInterest.Size, out Pathfind.Report report );
        }
        if( Responsibilities[4] ) {
            facilityOfInterest = Facilities.Get( Facility.EType.Navigations );
            _chain = Pathfind.GetPath( Coordinates, facilityOfInterest.Coordinates, facilityOfInterest.Size, out Pathfind.Report report );
            return;
        }
    }

    protected void UpdateAnimator(Vector3Int dir) {
        if(dir == Vector3Int.zero)
            return;
            
        animator.SetFloat("x", dir.x);
        animator.SetFloat("y", dir.y);
        animator.SetBool( "Moving", CurrentBehaviour == Behaviour.Walking );
    }
}
