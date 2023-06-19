using AlwaysEast;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public enum BuildWindow {
        Floor,
        Wall,
        Facility,
        NPCs
    };

    public static BuildWindow SelectedBuildWindow { get; set; } = BuildWindow.Floor;
    public static Entity SelectedEntity { get; set; }
    private bool canPlace = false;

    private Facility activeFacility = null;
    public static EnvironmentElement activeElement = null;

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
        footerTabs.SetActive( false );
        buildInterface.SetActive( true );
    }
    public void HideBuildMenu() {
        canPlace = false;
        footerTabs.SetActive( true );
        buildInterface.SetActive( false );
    }
    public void HideBuildMenuManual() {
        HideBuildMenu();
        activeElement = null;
        activeFacility = null;
    }

    public void ShowNPCInspector() {

        NPCInspectorInterface.SetActive( true );
        footerTabs.SetActive( false );
        buildInterface.SetActive( false );

        NPCNameField.text = SelectedEntity.Name;
        for( int i = 0; i < 5; i++ )
            NPCRoles[i].color = SelectedEntity.responsibilities[i] ? Color.yellow : Color.black;
    }
    public void HideNPCInspector() {
        NPCInspectorInterface.SetActive( false );
        footerTabs.SetActive( true );
        SelectedEntity = null;
    }

    public void BtnPress(int index) {

        SelectedBuildWindow = (BuildWindow)index;

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

        BoardManager.OnMouseCoordinateChange += BoardManager_OnMouseCoordinateChange;
        BoardManager.OnMouseClick += BoardManager_OnMouseClickChange;

        activeFacility = Facilities.Add( element.prefab );

        HideBuildMenu();
    }

    private void BoardManager_OnMouseClickChange( Vector3Int newCoordinate ) {

        if( !canPlace )
            return;

        ShowBuildMenu();

        BoardManager.OnMouseClick -= BoardManager_OnMouseClickChange;
        BoardManager.OnMouseCoordinateChange -= BoardManager_OnMouseCoordinateChange;

        activeFacility.GetComponent<SpriteRenderer>().color = Color.white;

        for( int y = 0; y > -activeFacility.Size.y; y-- )
            for( int x = 0; x < activeFacility.Size.x; x++ )
                Pathfind.Occupy( newCoordinate + new Vector3Int( x, y, 0 ) );

        activeFacility.Coordinates = newCoordinate;

        // give a box collider component to NPCs so we can select them
        CrewBehaviour cBehaviour = activeFacility.gameObject.GetComponent<CrewBehaviour>();
        if(cBehaviour != null) {
            activeFacility.gameObject.AddComponent<BoxCollider2D>();
            cBehaviour.OnMouseClick += Entity_OnMouseClick;
        }
    }

    private void Entity_OnMouseClick( ) {
        ShowNPCInspector( );
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

    public void ToggleRank(int index) {

        SelectedEntity.responsibilities[index] = !SelectedEntity.responsibilities[index];
        NPCRoles[index].color = SelectedEntity.responsibilities[index] ? Color.yellow : Color.black;
    }

    public void FinishEditingName(TMPro.TMP_InputField inputField) {
        if(inputField.text != string.Empty)
            SelectedEntity.Name = inputField.text;
    }
}