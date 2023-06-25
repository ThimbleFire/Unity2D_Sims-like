using AlwaysEast;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public Tilemap floor;

    public static bool Progressing { get; set; } = false;
    public static bool LifeSupport { get; set; } = true;

    private void Awake() {

        Debug.LogWarning( "This games UI is only compatible with a 512x512 viewport.\nIt has a pixel size of 128x128." );

        floor.CompressBounds();

        Pathfind.Setup( floor );

        // Adjust the size of the collider so it wraps around the ship
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Grid grid = GetComponent<Grid>();
        collider.size = new Vector3( floor.size.x * grid.cellSize.x, floor.size.y * grid.cellSize.y );
        collider.offset = new Vector2( floor.size.x * grid.cellSize.x / 2, floor.size.y * grid.cellSize.y / 2 );
    }
}