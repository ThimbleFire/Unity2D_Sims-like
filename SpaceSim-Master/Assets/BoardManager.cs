using UnityEngine;
using UnityEngine.Tilemaps;
using AlwaysEast;
using System;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    //private Dictionary<Core.Solutions, string> actionLibrary = new Dictionary<Core.Solutions, string>();

    public Tilemap floor;
    public Tilemap walls;
    
    public static bool Progressing { get; set; } = false;
    public static bool LifeSupport { get; set; } = true;

    private void Awake() {
        floor.CompressBounds();
        walls.CompressBounds();
        Pathfind.Setup( floor, walls );
        //walls.CompressBounds();

        //actionLibrary.Add( Core.Solutions.Chair, "spritesheet_192" );
        //actionLibrary.Add( Core.Solutions.Toilet, "spritesheet_195" );
        //actionLibrary.Add( Core.Solutions.Fridge, "spritesheet_196" );
        //actionLibrary.Add( Core.Solutions.Sink, "spritesheet_197" );
        
        GameTime.OnTck += GameTime_OnTck;
        //Entity.OnBehaviourChange += Entity_OnBehaviourChange;
    }

    //public Vector3Int FindFacility( Core.Solutions solution ) {
    //    foreach( Vector3Int cellPosition in walls.cellBounds.allPositionsWithin ) {
    //        if( walls.GetTile( cellPosition ) != null )
    //            if( walls.GetTile( cellPosition ).name == GetFacilityName( solution ) )
    //                return cellPosition;
    //    }
    //    return Vector3Int.zero;
    //}

    //public string GetFacilityName( Core.Solutions solution) {
    //    if( actionLibrary.ContainsKey( solution ) )
    //        return actionLibrary[solution];
    //    else {
    //        Debug.Log( $"no key exists for {solution}" );
    //        return string.Empty;
    //    }
    //}
    
    public void GameTime_OnTck() {
        if(Progressing == true)
            return;
            
        //every X minute an Event will occur
    }
    
    //public void Entity_OnBehaviourChange(Core.CurrentBehaviour currentBehaviour, Core.CurrentBehaviour lastBehaviour) {

    //    switch(lastBehaviour) {
    //        case Core.CurrentBehaviour.Captaining:
    //            Progressing = false;
    //        break;
    //    }
        
    //    switch(currentBehaviour) {
    //        case Core.CurrentBehaviour.Captaining:
    //            Progressing = true;
    //        break;
    //    }
    //}
}
