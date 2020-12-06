using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class GlowSwitch : EffectSwitch
{
    private Light2D[] lights;
    private SpriteRenderer spriteRenderer;
    public Vector2 lightIntensity;
    public Vector2 bloomIntensity;

    override
    public void startSwitch() {
        lights = GetComponentsInChildren<Light2D>();
        if (lightIntensity.x == 0 && lightIntensity.y == 0)
            lightIntensity = new Vector2(0, lights.Length > 0 ? lights[0].intensity : 1F);
        if (bloomIntensity.x == 0 && bloomIntensity.y == 0)
            bloomIntensity = new Vector2(0, 1F);
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    override
    public List<SpriteEffect> getSpriteEffects(bool enabled)
    {
        List<SpriteEffect> list = new List<SpriteEffect>();
        if (enabled)
        {
            list.Add(new SpriteEffect("opacity", Time.time, time, bloomIntensity.x, bloomIntensity.y, false, true, consequence));
            list.Add(new SpriteEffect("light", Time.time, time, lightIntensity.x, lightIntensity.y, false, true, null));
        }
        else
        {
            list.Add(new SpriteEffect("opacity", Time.time, time, bloomIntensity.y, bloomIntensity.x, false, false, null));
            list.Add(new SpriteEffect("light", Time.time, time, lightIntensity.y, lightIntensity.x, false, false, null));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled) {
        if (enabled)
        {
            setGlow(bloomIntensity.y);
            setLightIntensity(lightIntensity.y);
        }
        else {
            setGlow(bloomIntensity.x);
            setLightIntensity(lightIntensity.x);
        }
    }

    private void setGlow(float opacity) {
        Color color = this.spriteRenderer.color;
        color.a = opacity;
        this.spriteRenderer.color = color;
    }

    private void setLightIntensity(float intensity) {
        foreach (Light2D light in lights) {
            light.intensity = intensity;
        }
    }
}
