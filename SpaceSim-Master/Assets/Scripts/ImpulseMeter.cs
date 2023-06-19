using UnityEngine;

public class ImpulseMeter : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();
    private void Start() => spriteRenderer.enabled = false;

    public Sprite[] meterFrames;

    public void SetMeter( float meter ) {
        spriteRenderer.enabled = true;
        int index =
            //0.1428f is 1.0 / 7
              meter <= Entity.ImpulseMax * 0.1428f ? 0
            : meter <= Entity.ImpulseMax * 0.2856f ? 1
            : meter <= Entity.ImpulseMax * 0.4284f ? 2
            : meter <= Entity.ImpulseMax * 0.5712f ? 3
            : meter <= Entity.ImpulseMax * 0.7140f ? 4
            : meter <= Entity.ImpulseMax * 0.8568f ? 5
            : 6;
        spriteRenderer.sprite = meterFrames[index];
    }

    public void HideMeter() {
        spriteRenderer.enabled = false;
    }
}
