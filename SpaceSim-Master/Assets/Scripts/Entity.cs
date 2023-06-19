using AlwaysEast;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public enum ResponsibleDelegate
    {
        Captain, Medic, Gunnery, Navigator, Engineer
    };

    public const float ImpulseMax = 1800.0f;
    public string Name { get; set; }
    public string task;
    public List<float> impulses = new List<float>(new float[5] { ImpulseMax, ImpulseMax, ImpulseMax, ImpulseMax, ImpulseMax });
    public bool[] responsibilities = new bool[5] { false, false, false, false, false };
    protected byte BodyHeat { get; set; } = 37;
    public Vector3Int Coordinates { get; set; }

    public ImpulseMeter impulseMeter;
    
    protected List<Node> _chain = new List<Node>();

    protected Animator animator;

    private void Awake() {
        Name = Helper.GetRandomName;
        animator = GetComponent<Animator>();
        Coordinates = new Vector3Int( 5, 4, 0 );
        GameTime.OnTck += GameTime_OnTck;
    }

    private void GameTime_OnTck() {

    }

    protected void UpdateAnimator(Vector3Int dir) {
        if(dir == Vector3Int.zero)
            return;
            
        animator.SetFloat("x", dir.x);
        animator.SetFloat("y", dir.y);
        animator.SetBool("Moving", true);
    }
}
