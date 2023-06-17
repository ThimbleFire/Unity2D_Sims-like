using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Sprite selected;
    public Sprite unselected;

    public GameObject bottomTabs;
    public GameObject buildMenu;

    public Image btnFloor;
    public Image btnWall;
    public Image btnFacility;

    public void ShowBuildMenu() {
        bottomTabs.SetActive( false );
        buildMenu.SetActive( true ); 
    }
    public void HideBuildMenu() {
        bottomTabs.SetActive( true );
        buildMenu.SetActive( false );
    }
    public void BtnPress_Floors() {
        btnFloor.sprite = selected;
        btnWall.sprite = unselected;
        btnFacility.sprite = unselected;
    }
    public void BtnPress_Walls() {
        btnFloor.sprite = unselected;
        btnWall.sprite = selected;
        btnFacility.sprite = unselected;
    }
    public void BtnPress_Facilities() {
        btnFloor.sprite = unselected;
        btnWall.sprite = unselected;
        btnFacility.sprite = selected;
    }
}
