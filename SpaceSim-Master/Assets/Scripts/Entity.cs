using AlwaysEast;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public enum ResponsibleDelegate
    {
        Captain, Medic, Gunnery, Navigator, Engineer
    };
    public enum Behaviour
    {
        WonderingWhatToDo,
        Walking,
        UsingFacility,
        DoingJob
    };
    public enum Impulse
    {
        Bladder, Hunger, Water
    };

    public ImpulseMeter impulseMeter;

    public const float ImpulseMax = 350.0f;

    public string Name { get; set; }
    public List<float> Impulses  = new List<float>( new float[3] { ImpulseMax, ImpulseMax, ImpulseMax } );
    public bool[] Responsibilities = new bool[5] { false, false, false, false, false };

    public Vector3Int Coordinates { get; set; }
    public Behaviour CurrentBehaviour = Behaviour.WonderingWhatToDo;
    protected byte BodyHeat { get; set; } = 37;
    protected List<Node> _chain = new List<Node>();

    protected Facility facilityOfInterest;
    protected Animator animator;

    private void Awake() {
        Name = Helper.GetRandomName;
        gameObject.name = "NPC " + Name;
        animator = GetComponent<Animator>();
        Coordinates = new Vector3Int( 5, 4, 0 );    // Need to figure out a way to get coordinates from position on placement
        GameTime.OnTck += GameTime_OnTck;
    }

    private void OnDestroy() {
        GameTime.OnTck -= GameTime_OnTck;
    }

    public void GameTime_OnTck() {

        //bladder
        Impulses[0] -= 1 + Impulses[2] / ImpulseMax;
        //hunger
        Impulses[1] -= 2 - Impulses[1] / ImpulseMax;
        //hydration
        Impulses[2] -= 1 + ( BodyHeat - 37 ) / 10.0f;

        if( CurrentBehaviour != Behaviour.Walking )
            HandleBehaviours();
    }

    private void HandleBehaviours() {

        if( CurrentBehaviour == Behaviour.WonderingWhatToDo ||
            CurrentBehaviour == Behaviour.DoingJob ) {
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
                EndInteract();
            }
        } else facilityOfInterest.Interact();
    }

    private void EndInteract() {

        impulseMeter.HideMeter();
        if( facilityOfInterest != null )
            facilityOfInterest.InteractEnd();
    }

    protected virtual void OnArrival() {


    }

    private void GetTask() {
        for( int i = 0; i < Impulses.Count; i++ ) {

            // if an impulse is below 50%, ever additional percent gives a 2% chance to use the facility

            if( Impulses[i] > ImpulseMax / 2 )
                continue;

            float r = UnityEngine.Random.Range(0, Impulses[i]);

            if( r > ImpulseMax / 100.0f && Impulses[i] > ImpulseMax / 10 )
                continue;

            facilityOfInterest = Facilities.Get( ( Facility.EType )i );

            if( facilityOfInterest == null )
                continue;

            EndInteract();
            _chain = Pathfind.GetPath( Coordinates, ref facilityOfInterest.Coordinates, facilityOfInterest.Size, out Pathfind.Report report );
            return;

        }
        for( int i = 0; i < Responsibilities.Length; i++ ) {
            if( Responsibilities[i] ) {
                facilityOfInterest = Facilities.Get( ( Facility.EType )i + 5 );
                if( facilityOfInterest == null )
                    continue;
                _chain = Pathfind.GetPath( Coordinates, ref facilityOfInterest.Coordinates, facilityOfInterest.Size, out Pathfind.Report report );
                return;
            }
        }

        // when unable to find a task...
        CurrentBehaviour = Behaviour.WonderingWhatToDo;
    }

    public void SetCoordinates( Vector3Int coordinates ) {
        Coordinates = coordinates;
    }
}

public class Entities
{
    private static List<Entity> entities = new List<Entity>();

    public static void Add( Entity entity) {
        entities.Add( entity );
    }

    public static Entity Get( Vector3Int coordinates ) {
        Entity[] e = entities.FindAll( x => x.Coordinates == coordinates ).ToArray();

        if( e.Length == 0 ) {
            return null;
        } else
            return e[0];
    }
}
