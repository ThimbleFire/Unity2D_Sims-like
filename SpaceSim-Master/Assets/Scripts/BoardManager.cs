using AlwaysEast;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public delegate void OnMouseCoordinateChangeHandler( Vector3Int lastCoordinate, Vector3Int newCoordinate, Vector3 cellToWorld );
    public static event OnMouseCoordinateChangeHandler OnMouseCoordinateChange;

    public delegate void OnMouseClickHandler( Vector3Int newCoordinate );
    public static event OnMouseClickHandler OnMouseClick;

    private Vector3Int lastCoordinates = Vector3Int.zero;

    public Tilemap floor;
    public Tilemap walls;

    public static bool Progressing { get; set; } = false;
    public static bool LifeSupport { get; set; } = true;

    private void Awake() {

        Debug.LogWarning( "This games UI is only compatible with 512x512." );

        floor.CompressBounds();

        Pathfind.Setup( floor, walls );

        GameTime.OnTck += GameTime_OnTck;

        // Adjust the size of the collider so it wraps around the ship
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Grid grid = GetComponent<Grid>();
        collider.size = new Vector3( floor.size.x * grid.cellSize.x, floor.size.y * grid.cellSize.y );
        collider.offset = new Vector2( floor.size.x * grid.cellSize.x / 2, floor.size.y * grid.cellSize.y / 2 );
    }

    public void GameTime_OnTck() {
        if( Progressing == true )
            return;
    }

    private void OnMouseDown() {

        if( UIController.activeElement == null )
            return;

        ///<summary> prevent clicking on game objects when attempting to click on the UI </summary>
        if( EventSystem.current.IsPointerOverGameObject() ) {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        Vector3Int coordinates = floor.WorldToCell( mousePosition );

        if( floor.GetTile( coordinates ) == null )
            return;

        switch( UIController.SelectedBuildWindow ) {
            case UIController.BuildWindow.Floor:
                floor.SetTile( coordinates, UIController.activeElement.tileBase );
                break;
            case UIController.BuildWindow.Wall:
                Pathfind.Occupy( coordinates );
                walls.SetTile( coordinates, UIController.activeElement.tileBase );
                break;
            case UIController.BuildWindow.Facility:
            case UIController.BuildWindow.NPCs:
                OnMouseClick?.Invoke( coordinates );
                break;
        }
    }

    private void OnMouseOver() {

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
        Vector3Int coordinates = floor.WorldToCell( mousePosition );
        Vector3 worldPosition = floor.CellToWorld(coordinates);

        if( coordinates != lastCoordinates ) {

            OnMouseCoordinateChange?.Invoke( lastCoordinates, coordinates, worldPosition );

            lastCoordinates = coordinates;
        }
    }

    private void Update() {
        if( Input.GetMouseButtonDown( 1 ) ) {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
            Vector3Int coordinates = floor.WorldToCell( mousePosition );

            switch( UIController.SelectedBuildWindow ) {
                case UIController.BuildWindow.Wall:
                    walls.SetTile( coordinates, null );
                    Pathfind.Unoccupy( coordinates );
                    break;
                case UIController.BuildWindow.Facility:
                    Facilities.Remove( coordinates );
                    break;
            }
        }
    }
}
