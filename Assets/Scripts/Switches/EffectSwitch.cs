using Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectSwitch : MonoBehaviour
{
    protected SpriteEffects effects;
    public bool startEnabled;
    public List<EffectSwitch> consequence;
    public abstract List<SpriteEffect> getSpriteEffects(bool enabled);
    public abstract void setImmediate(bool enabled);
    public abstract void startSwitch();
    public bool invert;
    public float time;
    public bool autoStart;
    public bool backToStart;

    // Start is called before the first frame update
    void Start()
    {
        if (!this.TryGetComponent<SpriteEffects>(out effects))
            effects = this.gameObject.AddComponent<SpriteEffects>();

        startSwitch();

        if (startEnabled)
            enable(true);
        else disable(true);

        if (autoStart)
            setEnabled(true);
    }

    void OnDestroy() {
        SpriteEffects sE;
        if (this.TryGetComponent<SpriteEffects>(out sE))
            sE.clearEffect(this);
    }

    public void destroy()
    {
        SpriteEffects sE;
        if(this.TryGetComponent<SpriteEffects>(out sE))
            sE.clearEffect(this);

        Destroy(this);

        Debug.Log("Effect Switch destroyed. " + this.gameObject.name);
    }

    public void setEnabled(bool enabled)
    {
        if (invert)
            enabled = !enabled;

        if (enabled)
            enable(false);
        else disable(false);
    }

    public void enable(bool immediate)
    {
        //effects.clearEffects();
        if (immediate)
        {
            setImmediate(true);
        }
        else
        {
            effects.addEffects(this, getSpriteEffects(true));
        }
    }

    public void disable(bool immediate)
    {
        //effects.clearEffects();
        if (immediate)
        {
            setImmediate(false);
        }
        else
        {
            effects.addEffects(this, getSpriteEffects(false));
        }
    }
}
