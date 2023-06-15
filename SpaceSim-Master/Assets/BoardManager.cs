using UnityEngine;
using UnityEngine.Tilemaps;
using AlwaysEast;

public class BoardManager : MonoBehaviour
{
    public Tilemap floor;
    public Tilemap walls;

    private void Awake() {
        Pathfind.Setup( floor, walls );
    }
}