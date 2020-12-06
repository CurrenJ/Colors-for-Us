using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomEventSwitch : EffectSwitch
{
    public UnityEvent enableEvents = new UnityEvent();
    public UnityEvent disableEvents =  new UnityEvent();

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
            list.Add(new SpriteEffect("event", Time.time, time, 0, 1, false, backToStart, consequence));
        }
        else
        {
            list.Add(new SpriteEffect("event", Time.time, time, 1, 0, false, backToStart, consequence));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled)
    {
        if (enabled)
        {
            enableEvents.Invoke();
        }
        else
        {
            disableEvents.Invoke();
        }
    }
}
