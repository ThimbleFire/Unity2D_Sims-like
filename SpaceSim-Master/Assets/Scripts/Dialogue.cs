using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    private Image image;
    private Text speaker;
    private Text message;
    private Animation textAnimation;
    private Animation panelAnimation;
    private bool active = false;
    
    // This will need fixing.
    public void Awake() {
        //image = GetComponentsInChildren<Image>();
        //speaker = GetComponentsInChildren<Text>();
        //message = GetComponentsInChildren<Text>();
        //textAnimation = GetComponentsInChildren<Animation>();
        panelAnimation = GetComponent<Animation>();
    }
    //private static void Print(string speaker, string message, Sprite sprite = null) {
    //    if(active == false) {
    //        panelAnimation.Play("OnEnable");
    //        active = true;
    //    }        
    //    image.sprite = sprite;
    //    this.speaker.text = speaker;
    //    this.message.text = message;
    //    textAnimation.Play("Text_Pan_Left");
    //}
    //private static void Hide() {
    //    active = false;
    //    panelAnimation.Play("OnDisable");
    //}
}
