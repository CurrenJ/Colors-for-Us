using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpacitySwitch : EffectSwitch
{
    public Vector2 opacity;

    override
    public void startSwitch()
    {
    }

    override
    public List<SpriteEffect> getSpriteEffects(bool enabled)
    {
        List<SpriteEffect> list = new List<SpriteEffect>();
        if (enabled)
        {
            list.Add(new SpriteEffect("opacity", Time.time, time, opacity.x, opacity.y, false, true, consequence));
        }
        else
        {
            list.Add(new SpriteEffect("opacity", Time.time, time, opacity.y, opacity.x, false, false, consequence));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled)
    {
        if (enabled)
            setOpacity(opacity.y);
        else setOpacity(opacity.x);
    }

    private void setOpacity(float alpha)
    {
        SpriteRenderer s_Renderer;
        Image image;
        if (this.TryGetComponent<SpriteRenderer>(out s_Renderer))
        {
            Color color = this.GetComponent<SpriteRenderer>().color;
            color.a = alpha;
            this.GetComponent<SpriteRenderer>().color = color;
        } else if (this.TryGetComponent<Image>(out image)) {
            Color color = image.color;
            color.a = alpha;
            image.color = color;
        }
    }
}
