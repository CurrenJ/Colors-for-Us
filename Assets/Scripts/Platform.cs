using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    public PlatformController controller;
    private SpriteEffects effects;
    public float angle;
    public List<SpriteRenderer> linkedSprites;

    // Start is called before the first frame update
    void Start()
    {
        effects = this.gameObject.AddComponent<SpriteEffects>();
        if (linkedSprites != null)
        {
            foreach (SpriteRenderer sR in linkedSprites)
            {
                sR.gameObject.AddComponent<SpriteEffects>();
            }
        }
        else linkedSprites = new List<SpriteRenderer>();

    }

    public void sendPulse(Vector2 direction, float hue, float saturation) {
        // Cast a ray straight down.
        RaycastHit2D hit;
        if (GetComponents<MeshRenderer>().Length > 0)
        {
            Vector3 pos = transform.position + new Vector3(GetComponent<MeshFilter>().mesh.vertices[0].x + controller.tileSize / 2, GetComponent<MeshFilter>().mesh.vertices[0].y + 0.01F, 0);
            hit = Physics2D.Raycast(pos, direction, 1F, ~controller.layerMask);
        }
        else
        {
            //Debug.Log(transform.position + " | " + direction);
            hit = Physics2D.Raycast(transform.position, direction, 1F, ~controller.layerMask);
        }

        if (getSaturation() == 0)
        {
            SaturationSwitch sS = this.gameObject.AddComponent<SaturationSwitch>();
            sS.saturation = new Vector2(0, saturation);
            sS.autoStart = true;
            sS.time = 3F;
            sS.backToStart = true;

            HueSwitch hS = this.gameObject.AddComponent<HueSwitch>();
            hS.hue = new Vector2(0F, hue);
            hS.autoStart = true;
            hS.time = 0F;

            foreach (SpriteRenderer sR in linkedSprites)
            {
                SaturationSwitch _sS = sR.gameObject.AddComponent<SaturationSwitch>();
                _sS.saturation = new Vector2(0, saturation);
                _sS.autoStart = true;
                _sS.time = 3F;
                _sS.backToStart = true;

                HueSwitch _hS = sR.gameObject.AddComponent<HueSwitch>();
                _hS.hue = new Vector2(0F, hue);
                _hS.autoStart = true;
                _hS.time = 0F;
            }
        }

        // If it hits something...
        if (hit.collider != null)
        {
            Platform platform;
            if (hit.collider.gameObject.TryGetComponent<Platform>(out platform)) {
                platform.sendPulse(-platform.transform.up, hue, saturation);
            }
        }
    }

    public void sendPulse(Vector2 direction) {
        Debug.Log(this.transform.position);
        sendPulse(direction, 0, 1);
    }

    private float getSaturation() {
        SpriteRenderer s_Renderer;
        if (this.TryGetComponent<SpriteRenderer>(out s_Renderer))
        {
            Color.RGBToHSV(s_Renderer.color, out float h, out float s, out float v);
            return s;
        }
        else {
            Color.RGBToHSV(((Texture2D)GetComponent<Renderer>().material.mainTexture).GetPixel(0, 0), out float h, out float s, out float v);
            return s;
        }
    }

    //degrees
    public float getAngle() {
        return Mathf.Rad2Deg*angle;
    }

    //rads
    public void setAngle(float angle) {
        this.angle = angle;
    }
}
