using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceSwitch : EffectSwitch
{
    private Vector2 startPosition;
    public Vector2 offset;

    override
    public void startSwitch()
    {
        startPosition = this.gameObject.transform.localPosition;
    }

    override
    public List<SpriteEffect> getSpriteEffects(bool enabled)
    {
        List<SpriteEffect> list = new List<SpriteEffect>();
        if (enabled)
        {
            list.Add(new SpriteEffect("yForce", Time.time, time, 0, offset.y, false, consequence));
            list.Add(new SpriteEffect("xForce", Time.time, time, 0, offset.x, false, null));
        }
        else
        {
            list.Add(new SpriteEffect("yForce", Time.time, time, offset.y, 0, false, consequence));
            list.Add(new SpriteEffect("xForce", Time.time, time, offset.x, 0, false, null));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled)
    {
        if (enabled)
            setForce(offset);
        else setForce(new Vector2(0, 0));
    }

    private void setForce(Vector2 offset)
    {
        this.GetComponent<Rigidbody2D>().velocity = offset;
    }
}
