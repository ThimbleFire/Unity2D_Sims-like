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
    public static Vector3Int Coordinates { get; set; }
    
    private void Awake() {
        Coordinates = new Vector3Int(5, 5, 0);
        MakeActiveController();
    }
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
    private void BtnArrowUp() => UpdateCursor(Vector3Int.up);
    private void BtnArrowDown() => UpdateCursor(Vector3Int.down);
    private void BtnArrowLeft() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
            case ArrowKeysControlling.CursorBuildMode:
                UpdateCursor(Vector3Int.left);
                break;
            case ArrowKeysControlling.Tabs:
                UpdateTab(-1);
                break;
            case ArrowKeysControlling.Item:
                UpdateItem(-1);
                break;
            case ArrowKeysControlling.NPCInspector:
                break;
        }
    }
    private void BtnArrowRight() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
            case ArrowKeysControlling.CursorBuildMode:
                UpdateCursor(Vector3Int.right);
                break;
            case ArrowKeysControlling.Tabs:
                UpdateTab(1);
                break;
            case ArrowKeysControlling.Item:
                UpdateItem(1);
                break;
            case ArrowKeysControlling.NPCInspector:
                break;
        }
    }    
    private void BtnConfirm() {
        switch( arrowKeysControlling ) {
            case ArrowKeysControlling.Cursor:
                // it's possible to be in cursor mode while the editor is open, so have this check to ensure we're doing it right
                Facility f = Facilities.Get( Coordinates );
                if( f != null ) {
                    // this will cause an error if we select an actual facility and not an NPC
                    // though we'll update this later so we can move facilities instead of deleting and replacing
                    // when we start decreasing money for purchasing facilities, this'll be useful
                    SelectedEntity = f.GetComponent<CrewBehaviour>();
                    ShowNPCInspector();
                    arrowKeysControlling = ArrowKeysControlling.NPCInspector;
                    return;
                }
                if( !buildInterface.activeSelf ) {
                    arrowKeysControlling = ArrowKeysControlling.Tabs;
                    cursor.gameObject.SetActive( false );
                    ShowBuildMenu();
                    return;
                }
                break;
            case ArrowKeysControlling.Tabs:
                ShowBuildMenuCursor();
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
                OnMouseOverCoordinateChange( Coordinates );
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
                HideBuildMenu();
                HideBuildMenuManual();
                break;
            case ArrowKeysControlling.Item:
                buildMenuCursor.gameObject.SetActive( false );
                arrowKeysControlling = ArrowKeysControlling.Tabs;
                break;
            case ArrowKeysControlling.CursorBuildMode:
                ShowBuildMenuCursor();
                ShowBuildMenu();
                break;
            case ArrowKeysControlling.NPCInspector:
                HideNPCInspector();
                arrowKeysControlling = ArrowKeysControlling.Cursor;
                cursor.gameObject.SetActive( true );
                break;
        }
    }
    private void UpdateCursor(Vector3Int addition) {
        Coordinates += addition;
        Vector3 worldPosition = Helper.CellToWorld(Coordinates);
        Vector3 offset = new Vector3(0.04f, 0.04f);
        cursor.position = worldPosition + offset;
        if(arrowKeysControlling == ArrowKeysControlling.CursorBuildMode) {
            OnMouseOverCoordinateChange( Coordinates );
        }
    }
    private void UpdateTab(byte addition) {
        // this if statement might cause errors
        if(selectedTabIndex + addition < 0 || selectedTabIndex + addition > 4)
            return;
            
        selectedTabIndex += addition;
        SelectedBuildWindow = ( BuildWindow )selectedTabIndex;
        
        for( int i = 0; i < 4; i++ ) {
            if( i == selectedTabIndex ) {
                tabs[i].sprite = selected;
                panels[i].SetActive( true );
            } else {
                tabs[i].sprite = unselected;
                panels[i].SetActive( false );
            }
        }
    }
    private void UpdateItem(byte addition) {
        // this if statement might cause errors
        if( selectedItemIndex + addition < 0 || selectedItemIndex + addition > panels[selectedTabIndex].transform.childCount - 1 )
            return;
            
        selectedItemIndex += addition;
        buildMenuCursor.position = panels[selectedTabIndex].transform.GetChild( selectedItemIndex ).position;
        buildMenuCursor.sizeDelta = panels[selectedTabIndex].transform.GetChild(selectedItemIndex).GetComponent<Image>().sprite.rect.size + Vector2.one * 2;
    }
    public void ShowBuildMenu() {
        GameTime.ClockStop();
        buildInterface.SetActive( true );
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
        activeElement = null;
        activeFacility = null;
        SelectedBuildWindow = BuildWindow.None;
    }
    public void ShowNPCInspector() {
        NPCInspectorInterface.SetActive( true );
        NPCNameField.text = SelectedEntity.Name;
        for( int i = 0; i < 5; i++ )
            NPCRoles[i].color = SelectedEntity.Responsibilities[i] ? Color.yellow : Color.black;
    }
    public void HideNPCInspector() {
        NPCInspectorInterface.SetActive( false );
        SelectedEntity = null;
    }
    private void PlaceObjectIntoScene( Vector3Int newCoordinate ) {
        if( !canPlace )
            return;
        activeFacility.GetComponent<SpriteRenderer>().color = Color.white;
        // give a box collider component to NPCs so we can select them
        CrewBehaviour isNPC = activeFacility.gameObject.GetComponent<CrewBehaviour>();
        if( isNPC != null ) {
            activeFacility.gameObject.AddComponent<BoxCollider2D>();
            isNPC.OnMouseClick += ShowNPCInspector;
            isNPC.SetCoordinates( newCoordinate );
        } else {
            for( int y = 0; y > -activeFacility.Size.y; y-- )
                for( int x = 0; x < activeFacility.Size.x; x++ )
                    Pathfind.Occupy( newCoordinate + new Vector3Int( x, y, 0 ) );
        }
        ShowBuildMenuCursor();
        ShowBuildMenu();
    }
    private void ShowBuildMenuCursor(){
        arrowKeysControlling = ArrowKeysControlling.Item;
        buildMenuCursor.gameObject.SetActive( true );
    }
    private void OnMouseOverCoordinateChange( Vector3Int newCoordinate ) {
        activeFacility.transform.position = Helper.CellToWorld(newCoordinate) + new Vector3( 0.04f, 0.04f );   
        activeFacility.Coordinates = newCoordinate;
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
