using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewBehaviour : Navigator
{
    private void Start() {
        chain = AlwaysEast.Pathfind.GetPath( cellPosition, cellPosition + Vector3Int.down * 2 + Vector3Int.right * 2, false );
    }
}
