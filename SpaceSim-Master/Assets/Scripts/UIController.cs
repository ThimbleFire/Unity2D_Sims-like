using AlwaysEast;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public enum BuildWindow {
        Floor,
        Wall,
        Facility
    };

    public static bool BuildMenuOpen = false;
    public static BuildWindow SelectedBuildWindow { get; set; } = BuildWindow.Floor;
    private bool canPlace = false;

    private Facility activeFacility = null;
    public static EnvironmentElement activeElement = null;

    public Sprite selected;
    public Sprite unselected;

    public GameObject bottomTabs;
    public GameObject buildMenu;

    public Image btnFloor;
    public Image btnWall;
    public Image btnFacility;

    public GameObject panel_floor;
    public GameObject panel_walls;
    public GameObject panel_facilities;

    public void ShowBuildMenu() {
        bottomTabs.SetActive( false );
        buildMenu.SetActive( true );
        BuildMenuOpen = true;
    }
    public void HideBuildMenu() {
        bottomTabs.SetActive( true );
        buildMenu.SetActive( false );
        BuildMenuOpen = false;
    }

    public void BtnPress_Floors() {
        activeElement = null;

        SelectedBuildWindow = BuildWindow.Floor;

        btnFloor.sprite = selected;
        btnWall.sprite = unselected;
        btnFacility.sprite = unselected;

        panel_floor.SetActive( true );
        panel_walls.SetActive( false );
        panel_facilities.SetActive( false );
    }
    public void BtnPress_Walls() {
        activeElement = null;

        SelectedBuildWindow = BuildWindow.Wall;

        btnFloor.sprite = unselected;
        btnWall.sprite = selected;
        btnFacility.sprite = unselected;

        panel_floor.SetActive( false );
        panel_walls.SetActive( true );
        panel_facilities.SetActive( false );
    }
    public void BtnPress_Facilities() {
        activeElement = null;

        SelectedBuildWindow = BuildWindow.Facility;

        btnFloor.sprite = unselected;
        btnWall.sprite = unselected;
        btnFacility.sprite = selected;

        panel_floor.SetActive( false );
        panel_walls.SetActive( false );
        panel_facilities.SetActive( true );
    }
    public void BtnPress_Element( EnvironmentElement element ) {

        activeElement = element;

        if( element.prefab == null )
            return;

        BoardManager.OnMouseCoordinateChange += BoardManager_OnMouseCoordinateChange;
        BoardManager.OnMouseClickChange += BoardManager_OnMouseClickChange;

        activeFacility = Facilities.Add( element.prefab );

        HideBuildMenu();
    }

    private void BoardManager_OnMouseClickChange( Vector3Int newCoordinate ) {

        if( !canPlace )
            return;

        ShowBuildMenu();

        BoardManager.OnMouseClickChange -= BoardManager_OnMouseClickChange;
        BoardManager.OnMouseCoordinateChange -= BoardManager_OnMouseCoordinateChange;

        activeFacility.GetComponent<SpriteRenderer>().color = Color.white;

        for( int y = 0; y > -activeFacility.Size.y; y-- )
            for( int x = 0; x < activeFacility.Size.x; x++ )
                Pathfind.Occupy( newCoordinate + new Vector3Int( x, y, 0 ) );

        activeFacility.Coordinates = newCoordinate;
    }
    private void BoardManager_OnMouseCoordinateChange( Vector3Int lastCoordinate, Vector3Int newCoordinate, Vector3 worldPosition ) {
        
        activeFacility.transform.position = worldPosition + new Vector3(0.04f, 0.04f);

        canPlace = true;

        for( int y = 0; y > -activeFacility.Size.y; y-- )
        for( int x = 0; x < activeFacility.Size.x; x++ ) {
            Vector3Int offset = new Vector3Int( x, y, 0 );
            if( Pathfind.IsOccupied( newCoordinate + offset ) ) {
                canPlace = false;
                goto skip;
            }
        }
        skip:
        activeFacility.GetComponent<SpriteRenderer>().color = canPlace ? Color.green : Color.red;
    }
}
