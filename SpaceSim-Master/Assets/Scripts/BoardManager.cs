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
        floor.CompressBounds();
        walls.CompressBounds();
        Pathfind.Setup( floor, walls );

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
        if( Progressing == true )
            return;
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
                    break;
                case UIController.BuildWindow.Facility:
                    Facilities.Remove( coordinates );
                    break;
            }
        }
    }
}
