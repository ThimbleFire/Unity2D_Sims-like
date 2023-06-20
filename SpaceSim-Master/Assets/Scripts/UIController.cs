using AlwaysEast;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private enum ArrowKeysControlling
    {
        Cursor,
        Tabs,
        Item,
        NPCInspector,
        CursorBuildMode
    };
    public enum BuildWindow
    {
        Floor,
        Wall,
        Facility,
        NPCs,
        None
    };

    public static BuildWindow SelectedBuildWindow { get; set; } = BuildWindow.Floor;
    private static BuildWindow LastSelectedBuildWindow { get; set; } = BuildWindow.Floor;
    private ArrowKeysControlling arrowKeysControlling { get; set; } = ArrowKeysControlling.Cursor; 
    public static Entity SelectedEntity { get; set; }
    public static EnvironmentElement activeElement = null;
    private Facility activeFacility = null;

    private byte selectedTabIndex = 0;
    private byte selectedItemIndex = 0;

    private bool canPlace = false;

    public Sprite selected;
    public Sprite unselected;

    public GameObject buildInterface;
    public GameObject NPCInspectorInterface;

    public TMPro.TMP_InputField NPCNameField;
    public Image[] NPCRoles;

    public Image[] tabs;
    public GameObject[] panels;

    public Transform cursor;
    public RectTransform buildMenuCursor;
    public static int CoordinatesX = 5;
    public static int CoordinatesY = 5;
    public static Vector3Int Coordinates { get { return new Vector3Int( CoordinatesX, CoordinatesY, 0 ); } }

    private void Awake() => MakeActiveController();

    public void MakeActiveController() {
        Controller.OnWDown += BtnArrowUp;
        Controller.OnDDown += BtnArrowRight;
        Controller.OnSDown += BtnArrowDown;
        Controller.OnADown += BtnArrowLeft;
        Controller.OnRDown += BtnConfirm;
        Controller.OnFDown += BtnBack;
    }

    public void RelinquishController() {
        Controller.OnDDown -= BtnArrowRight;
        Controller.OnWDown -= BtnArrowUp;
        Controller.OnSDown -= BtnArrowDown;
        Controller.OnADown -= BtnArrowLeft;
        Controller.OnRDown -= BtnConfirm;
    }

    private void BtnArrowUp() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                CoordinatesY++;
                UpdateCursor();
                break;
            case ArrowKeysControlling.CursorBuildMode:
                CoordinatesY++;
                UpdateCursor();
                OnMouseOverCoordinateChange( Coordinates );
                break;
        }
    }
    private void BtnArrowDown() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                CoordinatesY--;
                UpdateCursor();
                break;
            case ArrowKeysControlling.CursorBuildMode:
                CoordinatesY--;
                UpdateCursor();
                OnMouseOverCoordinateChange( Coordinates );
                break;
        }
    }

    private void BtnArrowLeft() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                CoordinatesX--;
                UpdateCursor();
                break;
            case ArrowKeysControlling.Tabs:
                selectedTabIndex--;
                BtnTab( selectedTabIndex );
                break;
            case ArrowKeysControlling.Item:
                if( selectedItemIndex > 0 ) {
                    buildMenuCursor.position = panels[selectedTabIndex].transform.GetChild( --selectedItemIndex ).position;
                    buildMenuCursor.sizeDelta = panels[selectedTabIndex].transform.GetChild( selectedItemIndex ).GetComponent<Image>().sprite.rect.size + Vector2.one * 2;
                }
                break;
            case ArrowKeysControlling.NPCInspector:
                break;
            case ArrowKeysControlling.CursorBuildMode:
                CoordinatesX--;
                UpdateCursor();
                OnMouseOverCoordinateChange( Coordinates );
                break;
        }
    }
    private void BtnArrowRight() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                CoordinatesX++;
                UpdateCursor();
                break;
            case ArrowKeysControlling.Tabs:
                selectedTabIndex++;
                BtnTab( selectedTabIndex );
                break;
            case ArrowKeysControlling.Item:
                if( selectedItemIndex < panels[selectedTabIndex].transform.childCount - 1 ) {
                    buildMenuCursor.position = panels[selectedTabIndex].transform.GetChild( ++selectedItemIndex ).position;
                    buildMenuCursor.sizeDelta = panels[selectedTabIndex].transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite.rect.size + Vector2.one * 2;
                }
                break;
            case ArrowKeysControlling.NPCInspector:
                break;
            case ArrowKeysControlling.CursorBuildMode:
                CoordinatesX++;
                UpdateCursor();
                OnMouseOverCoordinateChange( Coordinates );
                break;
        }
    }
    
    private void BtnConfirm() {

        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                // it's possible to be in cursor mode while the editor is open, so have this check to ensure we're doing it right
                if( !buildInterface.activeSelf ) {
                    arrowKeysControlling = ArrowKeysControlling.Tabs;
                    cursor.gameObject.SetActive( false );
                    ShowBuildMenu();
                    return;
                }
                break;
            case ArrowKeysControlling.Tabs:
                buildMenuCursor.gameObject.SetActive( true );
                arrowKeysControlling = ArrowKeysControlling.Item;
                selectedItemIndex = 0; 
                buildMenuCursor.position = panels[selectedTabIndex].transform.GetChild( selectedItemIndex ).position;
                buildMenuCursor.sizeDelta = panels[selectedTabIndex].transform.GetChild( selectedItemIndex ).GetComponent<Image>().sprite.rect.size + Vector2.one * 2;
                break;
            case ArrowKeysControlling.Item:
                HideBuildMenu();
                arrowKeysControlling = ArrowKeysControlling.CursorBuildMode;
                EnvironmentElement element = panels[selectedTabIndex].transform.GetChild( selectedItemIndex ).GetComponent<EnvironmentElement>();
                if( element.prefab == null )
                    return;
                activeFacility = Facilities.Add( element.prefab, Coordinates );
                buildMenuCursor.gameObject.SetActive( false );
                break;
            case ArrowKeysControlling.CursorBuildMode:
                PlaceObjectIntoScene( Coordinates );
                break;
        }
    }
    private void BtnBack() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                break;
            case ArrowKeysControlling.Tabs:
                arrowKeysControlling = ArrowKeysControlling.Cursor;
                cursor.gameObject.SetActive( true );
                HideBuildMenuManual();
                break;
            case ArrowKeysControlling.Item:
                buildMenuCursor.gameObject.SetActive( false );
                arrowKeysControlling = ArrowKeysControlling.Tabs;
                break;
            case ArrowKeysControlling.NPCInspector:
                HideNPCInspector();
                break;
            case ArrowKeysControlling.CursorBuildMode:
                arrowKeysControlling = ArrowKeysControlling.Item;
                ShowBuildMenu();
                buildMenuCursor.gameObject.SetActive( true );
                break;
        }
    }

    private void UpdateCursor() {
        Vector3 offset = new Vector3(0.04f, 0.04f);
        Vector3 worldPosition = Helper.CellToWorld(Coordinates);
        cursor.position = worldPosition + offset;
    }

    public void ShowBuildMenu() {
        GameTime.ClockStop();
        buildInterface.SetActive( true );
        SelectedBuildWindow = LastSelectedBuildWindow;
        Controller.OnWDown -= BtnArrowUp;
        Controller.OnSDown -= BtnArrowDown;

    }
    
    public void HideBuildMenu() {
        canPlace = false;
        buildInterface.SetActive( false );
        Controller.OnWDown += BtnArrowUp;
        Controller.OnSDown += BtnArrowDown;
    }
    public void HideBuildMenuManual() {
        GameTime.ClockStart();
        HideBuildMenu();
        activeElement = null;
        activeFacility = null;
        SelectedBuildWindow = BuildWindow.None;
        Controller.OnWDown += BtnArrowUp;
        Controller.OnSDown += BtnArrowDown;
    }

    public void ShowNPCInspector() {

        NPCInspectorInterface.SetActive( true );
        buildInterface.SetActive( false );
        SelectedBuildWindow = BuildWindow.None;

        NPCNameField.text = SelectedEntity.Name;
        for( int i = 0; i < 5; i++ )
            NPCRoles[i].color = SelectedEntity.Responsibilities[i] ? Color.yellow : Color.black;
    }
    public void HideNPCInspector() {
        NPCInspectorInterface.SetActive( false );
        SelectedEntity = null;
    }

    public void BtnTab( byte index ) {

        SelectedBuildWindow = ( BuildWindow )index;

        for( int i = 0; i < 4; i++ ) {
            if( i == index ) {
                tabs[i].sprite = selected;
                panels[i].SetActive( true );
            } else {
                tabs[i].sprite = unselected;
                panels[i].SetActive( false );
            }
        }
    }

    private void PlaceObjectIntoScene( Vector3Int newCoordinate ) {

        if( !canPlace )
            return;

        activeFacility.GetComponent<SpriteRenderer>().color = Color.white;

        activeFacility.Coordinates = newCoordinate;

        // give a box collider component to NPCs so we can select them
        CrewBehaviour isNPC = activeFacility.gameObject.GetComponent<CrewBehaviour>();
        if( isNPC != null ) {
            activeFacility.gameObject.AddComponent<BoxCollider2D>();
            isNPC.OnMouseClick += SelectSceneObject;
            isNPC.SetCoordinates( newCoordinate );
        } else {
            for( int y = 0; y > -activeFacility.Size.y; y-- )
                for( int x = 0; x < activeFacility.Size.x; x++ )
                    Pathfind.Occupy( newCoordinate + new Vector3Int( x, y, 0 ) );
        }

        arrowKeysControlling = ArrowKeysControlling.Item;
        buildMenuCursor.gameObject.SetActive( true );
        Controller.OnWDown -= BtnArrowUp;
        Controller.OnSDown -= BtnArrowDown;
        ShowBuildMenu();
    }
    private void SelectSceneObject() {
        ShowNPCInspector();
    }
    private void OnMouseOverCoordinateChange( Vector3Int newCoordinate ) {

        activeFacility.transform.position = Helper.CellToWorld(newCoordinate) + new Vector3( 0.04f, 0.04f );

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
    public void ToggleRank( int index ) {

        SelectedEntity.Responsibilities[index] = !SelectedEntity.Responsibilities[index];
        NPCRoles[index].color = SelectedEntity.Responsibilities[index] ? Color.yellow : Color.black;
    }
    public void FinishEditingName( TMPro.TMP_InputField inputField ) {
        if( inputField.text != string.Empty ) {
            SelectedEntity.Name = inputField.text;
            SelectedEntity.gameObject.name = "NPC " + inputField.text;
        }
    }
}