using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationSwitch : EffectSwitch
{
    public float rotationChange;
    private float startRotation;

    override
    public void startSwitch()
    {
        startRotation = this.gameObject.transform.localEulerAngles.z;
    }

    override
    public List<SpriteEffect> getSpriteEffects(bool enabled)
    {
        List<SpriteEffect> list = new List<SpriteEffect>();
        if (enabled)
        {
            list.Add(new SpriteEffect("rotation", Time.time, time, this.transform.localEulerAngles.z, this.transform.localEulerAngles.z + rotationChange, false, true, consequence));
        }
        else
        {
            list.Add(new SpriteEffect("rotation", Time.time, time, this.transform.localEulerAngles.z, startRotation, false, false, consequence));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled)
    {
        if (enabled)
            setAngle(rotationChange);
        else setAngle(0);
    }

    private void setAngle(float angle)
    {
        Vector3 locRot = this.transform.localEulerAngles;
        locRot.z = angle;
        this.transform.localEulerAngles = locRot;
    }
}
