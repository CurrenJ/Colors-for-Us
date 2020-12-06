using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeSwitch : EffectSwitch
{
    public int sceneBuildIndex;

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
            list.Add(new SpriteEffect("sceneChange", Time.time, time, 0, 1, false, true, consequence));
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
            SceneManager.LoadScene(sceneBuildIndex);
        }
    }
}
