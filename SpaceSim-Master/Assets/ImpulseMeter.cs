using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpulseMeter : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    private void Awake() => spriteRenderer = GetComponent<SpriteRenderer>();
    private void Start() => spriteRenderer.enabled = false;

    public Sprite[] meterFrames;

    public void SetMeter(float meter) {
        spriteRenderer.enabled = true;
        int index =
              meter <= 257  ? 0
            : meter <= 514  ? 1
            : meter <= 771  ? 2
            : meter <= 1028 ? 3
            : meter <= 1285 ? 4
            : meter <= 1542 ? 5
            : 6;
        spriteRenderer.sprite = meterFrames[index];
    }

    public void HideMeter() {
        spriteRenderer.enabled = false;
    }
}
