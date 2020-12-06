using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private bool dirty;
    public GameController controller;
    public float hue;
    public float peakSaturation;
    public float flashStartTime;
    public float flashDuration = 1F;
    public bool flashed = false;

    void Start()
    {
        addTileToController();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void tileUpdate() {
        if (Time.time - flashStartTime < flashDuration)
        {
            updateFlash();
        }
    }

    public void addTileToController() {
        if (controller != null)
            controller.addTile(GetComponent<Tile>());
    }

    public void addTileToController(GameController gc)
    {
        this.controller = gc;
        if (controller != null)
            controller.addTile(GetComponent<Tile>());
    }

    public void markDirty()
    {
        this.dirty = true;
        controller.markDirty(this);
    }

    public void markClean()
    {
        this.dirty = false;
        controller.markClean(this);
    }

    private void flash() {
        if (!flashed)
        {
            markDirty();
            flashed = true;
            flashStartTime = Time.time;
        }
    }

    public void flash(float hue) {
        this.hue = hue;
        flash();
    }

    public void flash(float hue, float sat)
    {
        this.hue = hue;
        this.peakSaturation = sat;
        flash();
    }

    public void updateFlash() {
        Color color = this.GetComponent<SpriteRenderer>().color;
        Color.RGBToHSV(color, out float h, out float s, out float v);
        float scalar = (Time.time - flashStartTime) / flashDuration;
        s = easingFunction(scalar < 1 ? scalar : 1, peakSaturation);
        this.GetComponent<SpriteRenderer>().color = Color.HSVToRGB(hue, s, v);

        Debug.Log(s + " | " + scalar + " | " + flashStartTime + " now " + Time.time);

        if (scalar >= 0.25)
            controller.flashBelow(this);

        if (scalar >= 1)
            markClean();
    }

    public float easingFunction(float normalIn, float max) {
        return -1 * max * Mathf.Pow(normalIn - 1F, 2) + max;
    }

    public bool isFlashing()
    {
        return (dirty && Time.time - flashStartTime < flashDuration);
    }
}
