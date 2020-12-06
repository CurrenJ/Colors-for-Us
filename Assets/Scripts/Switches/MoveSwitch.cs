using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSwitch : EffectSwitch
{
    private Vector2 startPosition;
    public Vector2 offset;

    override
    public void startSwitch() {
        startPosition = this.gameObject.transform.localPosition;
    }

    override
    public List<SpriteEffect> getSpriteEffects(bool enabled) {
        List<SpriteEffect> list = new List<SpriteEffect>();
        if (enabled)
        {
            list.Add(new SpriteEffect("yOffset", Time.time, time, this.transform.localPosition.y, offset.y + startPosition.y, false, true, consequence));
            list.Add(new SpriteEffect("xOffset", Time.time, time, this.transform.localPosition.x, offset.x + startPosition.x, false, true, null));
        }
        else {
            list.Add(new SpriteEffect("yOffset", Time.time, time, this.transform.localPosition.y, startPosition.y, false, false, consequence));
            list.Add(new SpriteEffect("xOffset", Time.time, time, this.transform.localPosition.x, startPosition.x, false, false, null));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled) {
        if (enabled)
            setOffset(offset);
        else setOffset(new Vector2(0, 0));
    }

    private void setOffset(Vector2 offset) {
        this.transform.localPosition = startPosition + offset;
    }
}
