using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSwitch : EffectSwitch
{
    public Vector2 volume;

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
            list.Add(new SpriteEffect("volume", Time.time, time, volume.x, volume.y, false, true, consequence));
        }
        else
        {
            list.Add(new SpriteEffect("volume", Time.time, time, volume.y, volume.x, false, false, consequence));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled)
    {
        if (enabled)
            setVolume(volume.y);
        else setVolume(volume.x);
    }

    private void setVolume(float vol)
    {
    
    }
}
