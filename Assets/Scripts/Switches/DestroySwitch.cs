using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySwitch : EffectSwitch
{
    public GameObject gameobjectToDestroy;
    public Component componentToDestroy;

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
            list.Add(new SpriteEffect("destroy", Time.time, time, 0, 1, false, true, consequence));
        }
        else
        {
        }
        return list;
    }

    override
    public void setImmediate(bool enabled)
    {
        if (enabled)
        {
            if(gameobjectToDestroy == null && componentToDestroy == null)
                Destroy(this.gameObject);
            else {
                if (gameobjectToDestroy != null)
                    Destroy(gameobjectToDestroy);
                if (componentToDestroy != null)
                    Destroy(componentToDestroy);
            }
        }
    }
}
