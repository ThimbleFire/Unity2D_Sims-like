using UnityEngine;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    private static Image image;
    private static Text speaker;
    private static Text message;
    private static Animation textAnimation;
    private static Animation panelAnimation;
    private static bool active = false;
    
    // This will need fixing.
    public void Awake() {
        image = GetComponentsInChildren<Image>();
        speaker = GetComponentsInChildren<Text>();
        message = GetComponentsInChildren<Text>();
        textAnimation = GetComponentsInChildren<Animation>();
        panelAnimation = GetComponent<Animation>();
    }
    private static void Print(string speaker, string message, Sprite sprite = null) {
        if(active == false) {
            panelAnimation.Play("OnEnable");
            active = true;
        }        
        image.sprite = sprite;
        this.speaker.text = speaker;
        this.message.text = message;
        textAnimation.Play("Text_Pan_Left");
    }
    private static void Hide() {
        active = false;
        panelAnimation.Play("OnDisable");
    }
}
