using AlwaysEast;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public enum BuildWindow {
        Floor,
        Wall,
        Facility,
        NPCs,
        None
    };

    public static BuildWindow SelectedBuildWindow { get; set; } = BuildWindow.Floor;
    private static BuildWindow LastSelectedBuildWindow { get; set; } = BuildWindow.Floor;
    public static Entity SelectedEntity { get; set; }
    public static EnvironmentElement activeElement = null;
    private Facility activeFacility = null;

    private bool canPlace = false;

    public Sprite selected;
    public Sprite unselected;

    public GameObject footerTabs;
    public GameObject buildInterface;
    public GameObject NPCInspectorInterface;

    public TMPro.TMP_InputField NPCNameField;
    public Image[] NPCRoles; 

    public Image[] tabs;
    public GameObject[] panels;

    public void ShowBuildMenu() {
        GameTime.ClockStop();
        footerTabs.SetActive( false );
        buildInterface.SetActive( true );
        SelectedBuildWindow = LastSelectedBuildWindow;
    }
    public void HideBuildMenu() {
        canPlace = false;
        footerTabs.SetActive( true );
        buildInterface.SetActive( false );
    }
    public void HideBuildMenuManual() {
        GameTime.ClockStart();
        HideBuildMenu();
        activeElement = null;
        activeFacility = null;
        SelectedBuildWindow = BuildWindow.None;
    }

    public void ShowNPCInspector() {

        NPCInspectorInterface.SetActive( true );
        footerTabs.SetActive( false );
        buildInterface.SetActive( false );
        SelectedBuildWindow = BuildWindow.None;

        NPCNameField.text = SelectedEntity.Name;
        for( int i = 0; i < 5; i++ )
            NPCRoles[i].color = SelectedEntity.Responsibilities[i] ? Color.yellow : Color.black;
    }
    public void HideNPCInspector() {
        NPCInspectorInterface.SetActive( false );
        footerTabs.SetActive( true );
        SelectedEntity = null;
    }

    public void BtnPress(int index) {

        SelectedBuildWindow = (BuildWindow)index;
        LastSelectedBuildWindow = ( BuildWindow )index;

        for( int i = 0; i < 4; i++ ) {
            if(i == index) {
                tabs[i].sprite = selected;
                panels[i].SetActive( true );
            }
            else {
                tabs[i].sprite = unselected;
                panels[i].SetActive( false );
            }
        }
    }

    public void BtnPress_Element( EnvironmentElement element ) {

        activeElement = element;

        if( element.prefab == null )
            return;

        BoardManager.OnMouseCoordinateChange += OnMouseOverCoordinateChange;
        BoardManager.OnMouseClick += PlaceObjectIntoScene;

        activeFacility = Facilities.Add( element.prefab );

        HideBuildMenu();
    }
    private void PlaceObjectIntoScene( Vector3Int newCoordinate ) {

        if( !canPlace )
            return;

        ShowBuildMenu();

        BoardManager.OnMouseClick -= PlaceObjectIntoScene;
        BoardManager.OnMouseCoordinateChange -= OnMouseOverCoordinateChange;

        activeFacility.GetComponent<SpriteRenderer>().color = Color.white;

        activeFacility.Coordinates = newCoordinate;

        // give a box collider component to NPCs so we can select them
        CrewBehaviour isNPC = activeFacility.gameObject.GetComponent<CrewBehaviour>();
        if(isNPC != null) {
            activeFacility.gameObject.AddComponent<BoxCollider2D>();
            isNPC.OnMouseClick += SelectSceneObject;
            isNPC.SetCoordinates( newCoordinate );
        }
        else {
        for( int y = 0; y > -activeFacility.Size.y; y-- )
            for( int x = 0; x < activeFacility.Size.x; x++ )
                Pathfind.Occupy( newCoordinate + new Vector3Int( x, y, 0 ) );
        }
    }
    private void SelectSceneObject( ) {
        ShowNPCInspector( );
    }
    private void OnMouseOverCoordinateChange( Vector3Int lastCoordinate, Vector3Int newCoordinate, Vector3 worldPosition ) {

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
    public void ToggleRank(int index) {

        SelectedEntity.Responsibilities[index] = !SelectedEntity.Responsibilities[index];
        NPCRoles[index].color = SelectedEntity.Responsibilities[index] ? Color.yellow : Color.black;
    }
    public void FinishEditingName(TMPro.TMP_InputField inputField) {
        if( inputField.text != string.Empty ) {
            SelectedEntity.Name = inputField.text;
            SelectedEntity.gameObject.name = "NPC " + inputField.text;
        }
    }
}