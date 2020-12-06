using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FocusSwitch : EffectSwitch
{
    public GameObject newFocus;
    public Vector2 customZoom;
    public float customSmoothnessZoom;

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
            list.Add(new SpriteEffect("focus", Time.time, time, 0, 1, false, true, consequence));
        }
        else
        {
            list.Add(new SpriteEffect("focus", Time.time, time, 1, 0, false, true, consequence));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled)
    {
        if (enabled)
        {
            Camera.main.GetComponent<CameraFollow>().follow = newFocus;
            Camera.main.GetComponent<CameraFollow>().customSmoothnessZoom = customSmoothnessZoom;
            Camera.main.GetComponent<CameraFollow>().customZoom = customZoom;
        }
        else {
            Camera.main.GetComponent<CameraFollow>().follow = GameObject.FindGameObjectWithTag("Player");
            Camera.main.GetComponent<CameraFollow>().customSmoothnessZoom = 0;
            Camera.main.GetComponent<CameraFollow>().customZoom = new Vector2(0, 0);
        }
    }
}
