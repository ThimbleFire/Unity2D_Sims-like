using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseMeter : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();
    private void Start() => spriteRenderer.enabled = false;

    public Sprite[] meterFrames;

    public void SetMeter(byte meter) {
        spriteRenderer.enabled = true;
        int index =
              meter <= 36  ? 0
            : meter <= 72  ? 1
            : meter <= 108 ? 2
            : meter <= 144 ? 3
            : meter <= 180 ? 4
            : meter <= 216 ? 5
            : 6;
        spriteRenderer.sprite = meterFrames[index];
    }

    public void HideMeter() {
        spriteRenderer.enabled = false;
    }
}
