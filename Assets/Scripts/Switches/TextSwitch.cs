using Assets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class TextSwitch : EffectSwitch
{
    public string text;
    public bool dissapear;

    override
    public void startSwitch() {
        text = GetComponent<TextMeshPro>().text;
    }

    override
    public List<SpriteEffect> getSpriteEffects(bool enabled)
    {
        List<SpriteEffect> list = new List<SpriteEffect>();
        if (enabled)
        {
            list.Add(new SpriteEffect("text", Time.time, time, GetComponent<TextMeshPro>().text.Length, text.Length, dissapear, true, null));
        }
        else
        {
            list.Add(new SpriteEffect("text", Time.time, time, GetComponent<TextMeshPro>().text.Length, 0, dissapear, false, null));
        }
        return list;
    }

    override
    public void setImmediate(bool enabled) {
        if (enabled)
            setText(text);
        else setText("");
    }

    private void setText(string text)
    {
        GetComponent<TMPro.TextMeshPro>().text = text;
    }
}
