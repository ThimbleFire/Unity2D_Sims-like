using UnityEngine;
using UnityEngine.Tilemaps;
using AlwaysEast;
using System;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    private Dictionary<Entity.Solutions, string> actionLibrary = new Dictionary<Entity.Solutions, string>();

    public Tilemap floor;
    public Tilemap walls;

    private void Awake() {
        floor.CompressBounds();
        walls.CompressBounds();
        Pathfind.Setup( floor, walls );
        //walls.CompressBounds();

        actionLibrary.Add( Entity.Solutions.Chair, "spritesheet_192" );
        actionLibrary.Add( Entity.Solutions.Toilet, "spritesheet_195" );
        actionLibrary.Add( Entity.Solutions.Fridge, "spritesheet_196" );
        actionLibrary.Add( Entity.Solutions.Sink, "spritesheet_197" );
    }

    public Vector3Int FindFacility( Entity.Solutions solution ) {

        foreach( Vector3Int cellPosition in walls.cellBounds.allPositionsWithin ) {
            if( walls.GetTile( cellPosition ) != null )
                if( walls.GetTile( cellPosition ).name == GetFacilityName( solution ) )
                    return cellPosition;
        }

        return Vector3Int.zero;
    }

    public string GetFacilityName(Entity.Solutions solution) {
        if( actionLibrary.ContainsKey( solution ) )
            return actionLibrary[solution];
        else {
            Debug.Log( $"no key exists for {solution}" );
            return string.Empty;
        }
    }
}