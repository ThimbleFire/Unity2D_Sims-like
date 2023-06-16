public class Entity : MonoBehaviour
{
    public enum CurrentBehaviour {
        Eating, Drinking, Playing, Defecating, Working, Walking, Idling
    }
    public enum Impulses {
        Hunger, Water, Happiness, Bladder, Oxygen
    }
    public enum Solutions {
        Fridge, Sink, Play, Toilet, LifeSupport
    }
    
    private List<byte> impulses = new List<byte>(new byte[5] { 255, 255, 255, 255, 255 });

    public Vector3Int Coordinates { get; set; }
    
    protected currentBehaviour { get; set; } = CurrentBehaviour.Idling;
    
    // Chain is the list of tiles returned by the pathfinder
    protected List<Node> _chain = new List<Node>();
    
    protected Animator animator
    
    private void Awake() => animator = GetComponent<Animator>();
    
    protected void UpdateAnimator(Vector3Int dir) {
        if(dir == Vector3Int.zero)
            return;
            
        animator.SetInt("x", dir.x);
        animator.SetInt("y", dir.y);
        animator.SetBool("Moving", currentBehaviour == CurrentBehaviour.Walking);
    }
    
    protected virtual void Action() {
        
        /*
            BoardManager calls Action the entity is  idle
            Action calculates what the unit will do and assigns a chain
            Navigate then performs that chain
        */
        
        for(int i = 0; i < impulses.Count; i++) {
        
            if(impulses[i] <= 100) {
            
                Vector3Int facilityPosition = BoardManager.FindFacility(Solutions[i]);
                _chain = PathFind(Coordinates, facilityPosition, false);
            }
        }        
    }      
}
