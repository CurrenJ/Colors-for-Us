using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HueSwitch : EffectSwitch
{
    public Vector2 hue;

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
            list.Add(new SpriteEffect("hue", Time.time, time, hue.x, hue.y, backToStart, true, consequence));
        }
        else
        {
            list.Add(new SpriteEffect("hue", Time.time, time, hue.y, hue.x, backToStart, false, consequence));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled)
    {
        if (enabled)
            setSaturation(hue.y);
        else setSaturation(hue.x);
    }

    private void setSaturation(float alpha)
    {
        //Empty? Should do this at somepoint. Or not. You'll probably never need this bit of code.
    }
}
