using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaturationSwitch : EffectSwitch
{
    public Vector2 saturation;

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
            list.Add(new SpriteEffect("saturation", Time.time, time, saturation.x, saturation.y, backToStart, true, consequence));
        }
        else
        {
            list.Add(new SpriteEffect("saturation", Time.time, time, saturation.y, saturation.x, backToStart, false, consequence));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled)
    {
        if (enabled)
            setSaturation(saturation.y);
        else setSaturation(saturation.x);
    }

    private void setSaturation(float alpha)
    {
        //Empty? Should do this at somepoint. Or not. You'll probably never need this bit of code.
    }
}
