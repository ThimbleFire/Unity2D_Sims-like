using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entities : MonoBehaviour
{
    private static List<CrewBehaviour> crewMember = new List<CrewBehaviour>();

    private void Awake() {
        List<GameObject> p = new List<GameObject>(GameObject.FindGameObjectsWithTag( "Player" ));
        for(int i = 0; i < p.Count; i++ ) {
            crewMember.Add( p[i].GetComponent<CrewBehaviour>() );
        }        
    }

    public static List<Vector3Int> GetOccupied() {
        List<Vector3Int> occupied = new List<Vector3Int>();
        foreach( CrewBehaviour item in crewMember ) {
            occupied.Add( item.Coordinates );
        }
        return occupied;
    }
}
