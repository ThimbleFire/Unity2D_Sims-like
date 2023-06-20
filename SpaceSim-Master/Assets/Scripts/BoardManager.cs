using AlwaysEast;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public Tilemap floor;
    public Tilemap walls;

    public static bool Progressing { get; set; } = false;
    public static bool LifeSupport { get; set; } = true;

    private void Awake() {

        Debug.LogWarning( "This games UI is only compatible with a 512x512 viewport.\nIt has a pixel size of 128x128." );

        floor.CompressBounds();

        Pathfind.Setup( floor, walls );

        // Adjust the size of the collider so it wraps around the ship
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        Grid grid = GetComponent<Grid>();
        collider.size = new Vector3( floor.size.x * grid.cellSize.x, floor.size.y * grid.cellSize.y );
        collider.offset = new Vector2( floor.size.x * grid.cellSize.x / 2, floor.size.y * grid.cellSize.y / 2 );
    }

    //public void GameTime_OnTck() {
    //    if( Progressing == true )
    //        return;
    //}
    //private void OnMouseDown() {

    //    if( UIController.activeElement == null )
    //        return;

    //    ///<summary> prevent clicking on game objects when attempting to click on the UI </summary>
    //    if( EventSystem.current.IsPointerOverGameObject() ) {
    //        return;
    //    }

    //    Vector3 mousePosition = Camera.main.ScreenToWorldPoint( Input.mousePosition );
    //    Vector3Int coordinates = floor.WorldToCell( mousePosition );

    //    if( floor.GetTile( coordinates ) == null )
    //        return;

    //    switch( UIController.SelectedBuildWindow ) {
    //        case UIController.BuildWindow.Floor:
    //            floor.SetTile( coordinates, UIController.activeElement.tileBase );
    //            break;
    //        case UIController.BuildWindow.Wall:
    //            Pathfind.Occupy( coordinates );
    //            walls.SetTile( coordinates, UIController.activeElement.tileBase );
    //            break;
    //        case UIController.BuildWindow.Facility:
    //        case UIController.BuildWindow.NPCs:
    //            OnMouseClick?.Invoke( coordinates );
    //            break;
    //    }
    //}
    //private void Update() {
    //    switch( UIController.SelectedBuildWindow ) {
    //        case UIController.BuildWindow.Wall:
    //            walls.SetTile( Coordinates, null );
    //            Pathfind.Unoccupy( Coordinates );
    //            break;
    //        case UIController.BuildWindow.Facility:
    //            Facilities.Remove( Coordinates );
    //            break;
    //    }
    //}
}