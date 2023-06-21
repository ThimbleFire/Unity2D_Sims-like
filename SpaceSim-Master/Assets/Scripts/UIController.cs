using AlwaysEast;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private readonly Vector3 offset = new Vector3(0.04f, 0.04f);

    private enum PlacementType
    {
        Facility, NPC, Floor, None
    }
    private enum ArrowKeysControlling
    {
        Cursor,
        Tabs,
        Item,
        NPCInspector,
        CursorBuildMode
    };
    private enum BuildWindow
    {
        Floor,
        Wall,
        Facility,
        NPCs,
        None
    };
    private PlacementType placementType { get; set; } = PlacementType.None;
    private Vector3Int Coordinates { get; set; }
    private BuildWindow SelectedBuildWindow { get; set; } = BuildWindow.Floor;
    private ArrowKeysControlling arrowKeysControlling { get; set; } = ArrowKeysControlling.Cursor;
    public static Entity SelectedEntity { get; set; }
    private Facility activeFacility = null;
    private Entity activeEntity = null;
    private TileBase activeTileBase = null;
    private int selectedTabIndex = 0;
    private int[] selectedItemIndex = new int[5] { 0, 0, 0, 0, 0 };
    private bool canPlace = false;
    public GameObject buildInterface;
    public GameObject NPCInspectorInterface;
    public TMPro.TMP_InputField NPCNameField;
    public Image[] NPCRoles;
    public Image[] tabs;
    public GameObject[] panels;

    public Transform sceneCursor;
    public RectTransform UICursor;

    private void Awake() => Coordinates = new Vector3Int( 5, 5, 0 );

    public void ArrowKeyDown( InputAction.CallbackContext context ) {
        if( context.phase != InputActionPhase.Started )
            return;
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                CursorSceneMove( context.ReadValue<Vector2>() );
                break;
            case ArrowKeysControlling.Tabs:
                CursorTabMove( context.ReadValue<Vector2>().x );
                break;
            case ArrowKeysControlling.Item:
                CursorItemMove( context.ReadValue<Vector2>().x );
                break;
            case ArrowKeysControlling.NPCInspector:
                CursorNPCInspectorMove( context.ReadValue<Vector2>().x );
                break;
            case ArrowKeysControlling.CursorBuildMode:
                CursorSceneMove( context.ReadValue<Vector2>() );
                UpdatePlacementMode();
                break;
        }
    }
    public void Confirm( InputAction.CallbackContext context ) {
        if( context.phase != InputActionPhase.Started )
            return;
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                if( Entities.Get( Coordinates ) ) {
                    arrowKeysControlling = ArrowKeysControlling.NPCInspector;
                    SelectedEntity = Entities.Get( Coordinates );
                    ShowNPCInspector();
                    CursorNPCInspectorMove();
                } else {
                    arrowKeysControlling = ArrowKeysControlling.Tabs;
                    buildInterface.SetActive( true );
                    CursorTabMove();
                }
                break;
            case ArrowKeysControlling.Tabs:
                arrowKeysControlling = ArrowKeysControlling.Item;
                CursorItemMove();
                break;
            case ArrowKeysControlling.Item:
                arrowKeysControlling = ArrowKeysControlling.CursorBuildMode;
                StartPlacementMode();
                UpdatePlacementMode();
                break;
            case ArrowKeysControlling.CursorBuildMode:
                ConfirmPlacementMode();
                break;
            case ArrowKeysControlling.NPCInspector:
                ToggleRank();
                break;
        }
    }
    public void Back( InputAction.CallbackContext context ) {
        if( context.phase != InputActionPhase.Started )
            return;
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Tabs:
                arrowKeysControlling = ArrowKeysControlling.Cursor;
                buildInterface.SetActive( false );
                UICursor.gameObject.SetActive( false );
                break;
            case ArrowKeysControlling.Item:
                arrowKeysControlling = ArrowKeysControlling.Tabs;
                CursorTabMove();
                break;
            case ArrowKeysControlling.CursorBuildMode:
                EndPlacementMode( true );
                break;
            case ArrowKeysControlling.NPCInspector:
                HideNPCInspector();
                break;
        }
    }

    private void CursorSceneMove( Vector2 addition ) {
        Coordinates += new Vector3Int( ( int )addition.x, ( int )addition.y, 0 );
        Vector3 worldPosition = Helper.CellToWorld(Coordinates);
        sceneCursor.position = worldPosition + offset;
    }
    private void CursorTabMove( float addition = 0 ) {
        // this if statement might cause errors
        if( selectedTabIndex + addition < 0 || selectedTabIndex + addition >= 4 )
            return;

        panels[selectedTabIndex].SetActive( false );
        selectedTabIndex += ( int )addition;
        SelectedBuildWindow = ( BuildWindow )selectedTabIndex;
        panels[selectedTabIndex].SetActive( true );

        UICursor.gameObject.SetActive( true );
        UICursor.position = tabs[selectedTabIndex].gameObject.gameObject.transform.position;
        UICursor.sizeDelta = tabs[selectedTabIndex].GetComponent<Image>().rectTransform.sizeDelta;
    }
    private void CursorItemMove( float addition = 0 ) {
        // this if statement might cause errors
        if( selectedItemIndex[selectedTabIndex] + addition < 0 || selectedItemIndex[selectedTabIndex] + addition > panels[selectedTabIndex].transform.childCount - 1 )
            return;

        selectedItemIndex[selectedTabIndex] += ( int )addition;

        Transform panelTransform = panels[selectedTabIndex].transform.GetChild( selectedItemIndex[selectedTabIndex] );
        Image itemImage = panelTransform.GetComponent<Image>();

        UICursor.gameObject.SetActive( true );
        UICursor.position = panelTransform.position;
        UICursor.sizeDelta = itemImage.rectTransform.sizeDelta + Vector2.one * 4;
    }
    private void CursorNPCInspectorMove( float addition = 0) {
        if( selectedItemIndex[selectedTabIndex] + addition < 0 || selectedItemIndex[selectedTabIndex] + addition > NPCInspectorInterface.transform.childCount - 1 )
            return;

        selectedItemIndex[selectedTabIndex] += ( int )addition;

        Transform panelTransform = NPCInspectorInterface.transform.GetChild( selectedItemIndex[selectedTabIndex] );
        Image itemImage = panelTransform.GetComponent<Image>();

        UICursor.gameObject.SetActive( true );
        UICursor.position = panelTransform.position;
        UICursor.sizeDelta = itemImage.rectTransform.sizeDelta + Vector2.one * 4;
    }

    private void StartPlacementMode() {

        canPlace = false;
        UICursor.gameObject.SetActive( false );
        buildInterface.SetActive( false );

        EnvironmentElement element = panels[selectedTabIndex].transform.GetChild( selectedItemIndex[selectedTabIndex] ).GetComponent<EnvironmentElement>();

        if( element.prefab != null ) {
            // element is a facility or NPC

            Facility facility = element.prefab.GetComponent<Facility>();

            if( facility != null ) {
                placementType = PlacementType.Facility;
                activeFacility = Instantiate( element.prefab, Helper.CellToWorld( Coordinates ) + offset, Quaternion.identity ).GetComponent<Facility>();
                return;
            }

            Entity entity = element.prefab.GetComponent<Entity>();

            if( entity != null ) {
                placementType = PlacementType.NPC;
                activeEntity = Instantiate( element.prefab, Helper.CellToWorld( Coordinates ) + offset, Quaternion.identity ).GetComponent<Entity>();
                return;
            }
        } else {
            placementType = PlacementType.Floor;
            activeTileBase = element.tileBase;
        }
    }
    private void UpdatePlacementMode() {
        switch( placementType ) {
            case PlacementType.Facility:
                activeFacility.transform.position = Helper.CellToWorld( Coordinates ) + offset;
                activeFacility.Coordinates = Coordinates;
                canPlace = true;
                for( int y = 0; y > -activeFacility.Size.y; y-- )
                    for( int x = 0; x < activeFacility.Size.x; x++ ) {
                        Vector3Int offset = new Vector3Int( x, y, 0 );
                        if( Pathfind.IsOccupied( Coordinates + offset ) ) {
                            canPlace = false;
                            goto skip;
                        }
                    }
                skip:
                activeFacility.GetComponent<SpriteRenderer>().color = canPlace ? Color.green : Color.red;
                break;
            case PlacementType.NPC:
                activeEntity.transform.position = Helper.CellToWorld( Coordinates ) + offset;
                activeEntity.Coordinates = Coordinates;
                canPlace = true;
                if( Pathfind.IsOccupied( Coordinates ) ) {
                    canPlace = false;
                }
                activeEntity.GetComponent<SpriteRenderer>().color = canPlace ? Color.green : Color.red;
                break;
        }
    }
    private void ConfirmPlacementMode() {
        if( !canPlace )
            return;
        switch( placementType ) {
            case PlacementType.Facility:
                activeFacility.GetComponent<SpriteRenderer>().color = Color.white;
                for( int y = 0; y > -activeFacility.Size.y; y-- )
                    for( int x = 0; x < activeFacility.Size.x; x++ )
                        Pathfind.Occupy( Coordinates + new Vector3Int( x, y, 0 ) );
                Facilities.Add( activeFacility );
                EndPlacementMode( false );
                break;
            case PlacementType.NPC:
                activeEntity.GetComponent<SpriteRenderer>().color = Color.white;
                activeEntity.gameObject.AddComponent<BoxCollider2D>();
                Entities.Add( activeEntity );
                EndPlacementMode( false );
                break;
            case PlacementType.Floor:
                break;
        }
        arrowKeysControlling = ArrowKeysControlling.Item;
    }
    private void EndPlacementMode( bool abort ) {

        if( abort )
            switch( placementType ) {
                case PlacementType.Facility:
                    Destroy( activeFacility.gameObject );
                    activeFacility = null;
                    break;
                case PlacementType.NPC:
                    Destroy( activeEntity.gameObject );
                    activeEntity = null;
                    break;
            }

        UICursor.gameObject.SetActive( true );
        buildInterface.SetActive( true );
        arrowKeysControlling = ArrowKeysControlling.Item;
        placementType = PlacementType.None;
    }

    public void ShowNPCInspector() {
        NPCInspectorInterface.SetActive( true );
        NPCNameField.text = SelectedEntity.Name;
        for( int i = 0; i < 5; i++ )
            NPCRoles[i].color = SelectedEntity.Responsibilities[i] ? Color.yellow : Color.black;
    }
    public void HideNPCInspector() {
        arrowKeysControlling = ArrowKeysControlling.Cursor;
        NPCInspectorInterface.SetActive( false );
        UICursor.gameObject.SetActive( false );
        SelectedEntity = null;
    }
    public void ToggleRank() {
        if( selectedItemIndex[4] < 1 || selectedItemIndex[4] > NPCInspectorInterface.transform.childCount - 2 )
            return;

        SelectedEntity.Responsibilities[selectedItemIndex[4] - 1] = !SelectedEntity.Responsibilities[selectedItemIndex[4] - 1];
        NPCRoles[selectedItemIndex[4] - 1].color = SelectedEntity.Responsibilities[selectedItemIndex[4] - 1] ? Color.yellow : Color.black;
    }

    //public void FinishEditingName( TMPro.TMP_InputField inputField ) {
    //    if( inputField.text != string.Empty ) {
    //        SelectedEntity.Name = inputField.text;
    //        SelectedEntity.gameObject.name = "NPC " + inputField.text;
    //    }
    //}
}
