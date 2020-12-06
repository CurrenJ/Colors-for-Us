using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockControlsSwitch : EffectSwitch
{
    override
    public void startSwitch()
    {
    }

    override
    public List<SpriteEffect> getSpriteEffects(bool locked)
    {
        List<SpriteEffect> list = new List<SpriteEffect>();
        setControlsLocked(locked);
        return list;
    }

    override
    public void setImmediate(bool locked)
    {
        setControlsLocked(locked);
    }

    private void setControlsLocked(bool locked)
    {
        this.gameObject.GetComponent<PlayerController>().disableControls = locked;
    }
}
