using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitSwitch : EffectSwitch
{
    override
    public void startSwitch() { }

    override
    public List<SpriteEffect> getSpriteEffects(bool enabled)
    {
        List<SpriteEffect> list = new List<SpriteEffect>();
        if (enabled)
        {
            list.Add(new SpriteEffect("wait", Time.time, time, 0, 1, false, true, consequence));
        }
        else
        {
            list.Add(new SpriteEffect("wait", Time.time, time, 0, 1, false, false, consequence));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled) { }
}
