using AlwaysEast;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;

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
    private PlacementType placementType { get; set; } = PlacementType.None;
    private Vector3Int Coordinates { get; set; }
    private ArrowKeysControlling arrowKeysControlling { get; set; } = ArrowKeysControlling.Cursor;
    public static Entity SelectedEntity { get; set; }
    private Facility activeFacility = null;
    private Entity activeEntity = null;
    private TileBase activeTileBase = null;
    private int selectedTabIndex = 0;
    private int[] selectedItemIndex = new int[5] { 0, 0, 0, 0, 0 };
    private bool canPlace = false;
    public GameObject NPCInspectorInterface;
    public TMPro.TMP_InputField NPCNameField;
    public Image[] NPCRoles;
    public Image[] tabs;
    public GameObject[] panels;
    public UIToggle buildWindowController;

    public Transform sceneCursor;
    public RectTransform UICursor;

    private void Awake() => Coordinates = new Vector3Int( 5, 5, 0 );

    public void MobileL() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                CursorSceneMove( Vector2Int.left );
                break;
            case ArrowKeysControlling.Tabs:
                CursorTabMove( -1 );
                break;
            case ArrowKeysControlling.Item:
                CursorItemMove( -1 );
                break;
            case ArrowKeysControlling.NPCInspector:
                CursorNPCInspectorMove( -1 );
                break;
            case ArrowKeysControlling.CursorBuildMode:
                CursorSceneMove( Vector2Int.left );
                UpdatePlacementMode();
                break;
        }
    }
    public void MobileR() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                CursorSceneMove( Vector2Int.right );
                break;
            case ArrowKeysControlling.Tabs:
                CursorTabMove( 1 );
                break;
            case ArrowKeysControlling.Item:
                CursorItemMove( 1 );
                break;
            case ArrowKeysControlling.NPCInspector:
                CursorNPCInspectorMove( 1 );
                break;
            case ArrowKeysControlling.CursorBuildMode:
                CursorSceneMove( Vector2Int.right );
                UpdatePlacementMode();
                break;
        }
    }
    public void MobileU() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                CursorSceneMove( Vector2Int.up );
                break;
            case ArrowKeysControlling.CursorBuildMode:
                CursorSceneMove( Vector2Int.up );
                UpdatePlacementMode();
                break;
        }
    }
    public void MobileD() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                CursorSceneMove( Vector2Int.down );
                break;
            case ArrowKeysControlling.CursorBuildMode:
                CursorSceneMove( Vector2Int.down );
                UpdatePlacementMode();
                break;
        }
    }
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
    public void Confirm_Mobile() {

        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                GameTime.ClockStop();
                sceneCursor.gameObject.SetActive( false );
                if( Entities.Get( Coordinates ) ) {
                    arrowKeysControlling = ArrowKeysControlling.NPCInspector;
                    SelectedEntity = Entities.Get( Coordinates );
                    ShowNPCInspector();
                    CursorNPCInspectorMove();
                    return;
                }
                if( Facilities.Get(Coordinates) ) {
                    activeFacility = Facilities.Get( Coordinates );
                    Facilities.Remove( Coordinates, false );
                    arrowKeysControlling = ArrowKeysControlling.CursorBuildMode;
                    placementType = PlacementType.Facility;
                    UpdatePlacementMode();
                    return;
                }
                arrowKeysControlling = ArrowKeysControlling.Tabs;
                buildWindowController.Enable();
                CursorTabMove();
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
                sceneCursor.gameObject.SetActive( false );
                GameTime.ClockStop();
                break;
        }

    }
    public void Confirm( InputAction.CallbackContext context ) {

        if( context.phase != InputActionPhase.Started )
            return;

        Confirm_Mobile();
    }
    public void Back_Mobile() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Tabs:
                arrowKeysControlling = ArrowKeysControlling.Cursor;
                buildWindowController.Disable();
                UICursor.gameObject.SetActive( false );
                sceneCursor.gameObject.SetActive( true );
                GameTime.ClockStart();
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
                GameTime.ClockStart();
                sceneCursor.gameObject.SetActive( true );
                break;
        }
    }
    public void Back( InputAction.CallbackContext context ) {
        if( context.phase != InputActionPhase.Started )
            return;
        Back_Mobile();
    }

    private void CursorSceneMove( Vector2 addition ) {
        Coordinates += new Vector3Int( ( int )addition.x, ( int )addition.y, 0 );
        Vector3 worldPosition = Helper.CellToWorld(Coordinates);
        sceneCursor.position = worldPosition + offset;

        AdjustCameraPosition();
    }
    private void CursorTabMove( float addition = 0 ) {
        // this if statement might cause errors
        if( selectedTabIndex + addition < 0 || selectedTabIndex + addition >= 4 )
            return;

        panels[selectedTabIndex].SetActive( false );
        selectedTabIndex += ( int )addition;
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
        if( selectedItemIndex[4] + addition < 0 || selectedItemIndex[4] + addition > NPCInspectorInterface.transform.childCount - 1 )
            return;

        selectedItemIndex[4] += ( int )addition;

        Transform panelTransform = NPCInspectorInterface.transform.GetChild( selectedItemIndex[4] );
        Image itemImage = panelTransform.GetComponent<Image>();

        UICursor.gameObject.SetActive( true );
        UICursor.position = panelTransform.position;
        UICursor.sizeDelta = itemImage.rectTransform.sizeDelta + Vector2.one * 4;
    }

    private void StartPlacementMode() {

        canPlace = false;
        UICursor.gameObject.SetActive( false );
        buildWindowController.Disable();

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
                activeFacility.CoordinateSize.position = Coordinates;
                canPlace = true;
                for( int y = 0; y > -activeFacility.CoordinateSize.size.y; y-- )
                    for( int x = 0; x < activeFacility.CoordinateSize.size.x; x++ ) {
                        Vector3Int offset = new Vector3Int( x, y, 0 );
                        if( !Pathfind.IsWalkable( Coordinates + offset ) ) {
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
                if( !Pathfind.IsWalkable( Coordinates ) ) {
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
                for( int y = 0; y > -activeFacility.CoordinateSize.size.y; y-- )
                    for( int x = 0; x < activeFacility.CoordinateSize.size.x; x++ )
                        Pathfind.Occupy( Coordinates + new Vector3Int( x, y, 0 ) );
                Facilities.Add( activeFacility );
                EndPlacementMode( false );

                // Reset AI behaviour in case we changed the position of a facility they were interacting with
                foreach( Entity item in Entities.Get()) {
                    item.CurrentBehaviour = Entity.Behaviour.WonderingWhatToDo;
                }

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
        buildWindowController.Enable();
        arrowKeysControlling = ArrowKeysControlling.Item;
        placementType = PlacementType.None;
    }

    public void ShowNPCInspector() {
        NPCInspectorInterface.SetActive( true );
        sceneCursor.gameObject.SetActive( false ); 
        NPCNameField.text = SelectedEntity.Name;
        for( int i = 0; i < 5; i++ )
            NPCRoles[i].color = SelectedEntity.Responsibilities[i] ? Color.yellow : Color.black;
    }
    public void HideNPCInspector() {
        arrowKeysControlling = ArrowKeysControlling.Cursor;
        NPCInspectorInterface.SetActive( false );
        UICursor.gameObject.SetActive( false );
        sceneCursor.gameObject.SetActive( true );
        SelectedEntity = null;
    }
    public void ToggleRank() {
        if( selectedItemIndex[4] < 1 || selectedItemIndex[4] > NPCInspectorInterface.transform.childCount - 2 )
            return;

        SelectedEntity.Responsibilities[selectedItemIndex[4] - 1] = !SelectedEntity.Responsibilities[selectedItemIndex[4] - 1];
        NPCRoles[selectedItemIndex[4] - 1].color = SelectedEntity.Responsibilities[selectedItemIndex[4] - 1] ? Color.yellow : Color.black;
        SelectedEntity.CurrentBehaviour = Entity.Behaviour.WonderingWhatToDo; // reset NPC behaviour
    }

    private void AdjustCameraPosition() {
        Vector3 cursorPositionOnScreen = Camera.main.WorldToScreenPoint( sceneCursor.position );

        if( cursorPositionOnScreen.x < 32 ) {

            //less than 32 to the left

        }
        if( cursorPositionOnScreen.x > 124 ) {

            //more than 124 to the right

        }

    }

    //public void FinishEditingName( TMPro.TMP_InputField inputField ) {
    //    if( inputField.text != string.Empty ) {
    //        SelectedEntity.Name = inputField.text;
    //        SelectedEntity.gameObject.name = "NPC " + inputField.text;
    //    }
    //}
}
