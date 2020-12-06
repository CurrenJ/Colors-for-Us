using Assets;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SpriteEffects : MonoBehaviour
{   
    private Dictionary<EffectSwitch, List<SpriteEffect>> effects = new Dictionary<EffectSwitch, List<SpriteEffect>>();
    private Renderer m_Renderer;
    private SpriteEffectsController controller;
    // Start is called before the first frame update
    void Start()
    {
        m_Renderer = GetComponent<Renderer>();
        controller = GameObject.FindGameObjectWithTag("Sprite Effects Controller").GetComponent<SpriteEffectsController>();
        controller.add(this.GetComponent<SpriteEffects>());
    }

    void OnDestroy() {
        clearEffects();
    }

    public bool updateEffects() {
        List<EffectSwitch> switches = new List<EffectSwitch>(effects.Keys);
        foreach(EffectSwitch _switch in switches)
        {
            List<SpriteEffect> _effects = effects[_switch];
            for(int s = 0; s < _effects.Count; s++)
            {
                SpriteEffect _effect = _effects[s];
                double val = _effect.update(Time.time);
                switch (_effect.getID())
                {
                    case ("opacity"):
                        updateOpacity(val);
                        break;
                    case ("saturation"):
                        updateSaturation(val);
                        break;
                    case ("hue"):
                        updateHue(val);
                        break;
                    case ("light"):
                        updateLight(val);
                        break;
                    case ("yOffset"):
                        updateOffsetY(val);
                        break;
                    case ("xOffset"):
                        updateOffsetX(val);
                        break;
                    case ("yForce"):
                        updateVelY(val);
                        break;
                    case ("xForce"):
                        updateVelX(val);
                        break;
                    case ("text"):
                        updateText(val);
                        break;
                    case ("rotation"):
                        updateRotation(val);
                        break;
                    case ("destroy"):
                        updateDestroy((DestroySwitch) _switch, val);
                        break;
                    case ("sceneChange"):
                        updateScene((SceneChangeSwitch) _switch, val);
                        break;
                    case ("focus"):
                        updateFocus((FocusSwitch)_switch, val);
                        break;
                    case ("event"):
                        updateEventInvokation((CustomEventSwitch)_switch, val);
                        break;
                    case ("wait"):
                        break;
                }
                if (_effect.isFinished(Time.time))
                {
                    effects[_switch].Remove(_effect);
                    s--;
                    List<EffectSwitch> consequence = _effect.getConsequence();
                    if (consequence != null)
                    {
                        foreach (EffectSwitch ef in consequence)
                        {
                            try
                            {
                                ef.setEnabled(_effect.isEnabled());
                            }
                            catch (System.Exception e) {
                                //Debug.LogError(e);
                                //shhhhh
                                //marked as resolved.
                            }
                        }
                    }
                }
            }
        }

        return effects.Count > 0;
    }

    private void setActive() {
        if(controller == null || !controller.activeSpriteEffects.Contains(this.GetComponent<SpriteEffects>()))
            StartCoroutine("WaitForControllerAndSetActive");
    }

    IEnumerator WaitForControllerAndSetActive()
    {
        while (controller == null)
        {
            controller = GameObject.FindGameObjectWithTag("Sprite Effects Controller").GetComponent<SpriteEffectsController>();
            yield return new WaitForSeconds(.1f);
        }
        controller.setActive(this.GetComponent<SpriteEffects>());
        yield return null;
    }

    public void addEffect(EffectSwitch _switch, SpriteEffect e) {
        if (!effects.ContainsKey(_switch)) {
            effects.Add(_switch, new List<SpriteEffect>());
        }

        if (!effects[_switch].Contains(e)) {
            effects[_switch].Add(e);
        }

        setActive();
    }

    public void addEffects(EffectSwitch _switch, List<SpriteEffect> lE) {
        foreach (SpriteEffect e in lE) {
            addEffect(_switch, e);
            setActive();
        }
    }

    public void addEffect(EffectSwitch _switch, string id, float duration, float startVal, float endVal, bool backToStart, List<EffectSwitch> consequence)
    {
        addEffect(_switch, new SpriteEffect(id, Time.time, duration, startVal, endVal, backToStart, consequence));
        setActive();
    }

    public void addEffect(EffectSwitch _switch, string id, float duration, float startVal, float endVal, bool backToStart) {
        addEffect(_switch, new SpriteEffect(id, Time.time, duration, startVal, endVal, backToStart, null));
        setActive();
    }

    public void addEffect(EffectSwitch _switch, string id, float duration, float endVal, bool backToStart)
    {
        addEffect(_switch, id, duration, 0F, endVal, backToStart);
        setActive();
    }

    public void clearEffects() {
        effects.Clear();
    }

    public void clearEffect(EffectSwitch eS) {
        effects.Remove(eS);
    }

    private void updateOpacity(double val) {
        SpriteRenderer s_Renderer;
        Image image;
        if (this.TryGetComponent<SpriteRenderer>(out s_Renderer))
        {
            Color color = s_Renderer.color;
            color.a = (float)val;
            s_Renderer.color = color;
        }
        else if (this.TryGetComponent<Image>(out image)) {
            Color color = image.color;
            color.a = (float)val;
            image.color = color;
        }
        else
        {
            Color color = m_Renderer.material.color;
            color.a = (float)val;
            m_Renderer.material.color = color;
        }

        foreach (Text text in this.GetComponentsInChildren<Text>()) {
            Color color = text.color;
            color.a = (float)val;
            text.color = color;
        }
        foreach (TextMeshPro text in this.GetComponentsInChildren<TextMeshPro>())
        {
            Color color = text.color;
            color.a = (float)val;
            text.color = color;
        }

    }

    private void updateEventInvokation(CustomEventSwitch _switch, double val) {
        if (val == 1)
            _switch.setImmediate(true);
        else if (val == 0)
            _switch.setImmediate(false);
    }

    private void updateFocus(FocusSwitch _switch, double val) {
        if (val == 1)
        {
            _switch.setImmediate(true);
        }
        else if (val == 0) {
            _switch.setImmediate(false);
        }
    }

    private void updateScene(SceneChangeSwitch _switch, double val) {
        if (val == 1) {
            _switch.setImmediate(true);
        }
    }

    private void updateDestroy(DestroySwitch dS, double val) {
        if (val == 1)
        {
            dS.setImmediate(true);
        }
    }

    private void updateText(double val) {
        TextMeshPro tmp;
        TextSwitch tSwitch;
        if (this.TryGetComponent<TextMeshPro>(out tmp) && this.TryGetComponent<TextSwitch>(out tSwitch)) {
            tmp.text = tSwitch.text.Substring(0, Mathf.RoundToInt((float)val));
        }
    }

    private void updateSaturation(double val) {
        SpriteRenderer s_Renderer;
        if (this.TryGetComponent<SpriteRenderer>(out s_Renderer))
        {
            Color.RGBToHSV(s_Renderer.color, out float h, out float s, out float v);
            s = (float)val;
            s_Renderer.color = Color.HSVToRGB(h, s, v);
        }
        else {
            Color.RGBToHSV(((Texture2D)m_Renderer.material.mainTexture).GetPixel(0, 0), out float h, out float s, out float v);
            s = (float)val;
            m_Renderer.material.mainTexture = createTexture(Color.HSVToRGB(h, s, v));
        }
    }

    private void updateHue(double val)
    {
        SpriteRenderer s_Renderer;
        if (this.TryGetComponent<SpriteRenderer>(out s_Renderer))
        {
            Color.RGBToHSV(s_Renderer.color, out float h, out float s, out float v);
            h = (float)val;
            s_Renderer.color = Color.HSVToRGB(h, s, v);
        }
        else
        {
            //Sometimes setting the pixel just doesn't work. Doing it in another asynchronous thread seems to resolve this
            StartCoroutine(setMeshColor(0.0F, val));
        }
    }

    IEnumerator setMeshColor(float time, double val)
    {
        yield return new WaitForSeconds(time);

        Color.RGBToHSV(((Texture2D)m_Renderer.material.mainTexture).GetPixel(0, 0), out float h, out float s, out float v);
        h = (float)val;
        ((Texture2D)m_Renderer.material.mainTexture).SetPixel(0, 0, Color.HSVToRGB(h, s, v));
        ((Texture2D)m_Renderer.material.mainTexture).Apply();


        Color.RGBToHSV(((Texture2D)m_Renderer.material.mainTexture).GetPixel(0, 0), out float h1, out float s1, out float v1);
    }

    private float getHue() {
        SpriteRenderer s_Renderer;
        if (this.TryGetComponent<SpriteRenderer>(out s_Renderer))
        {
            Color.RGBToHSV(s_Renderer.color, out float h, out float s, out float v);
            return h;
        }
        else
        {
            Color.RGBToHSV(((Texture2D)m_Renderer.material.mainTexture).GetPixel(0, 0), out float h, out float s, out float v);
            return h;
        }
    }

    private void updateLight(double val) {
        Light2D[] lights = this.GetComponentsInChildren<Light2D>();
        if (lights.Length > 0) {
            foreach (Light2D light in lights) {
                light.intensity = (float)val;
            }
        }
    }

    private void updateOffsetY(double val) {
            Vector3 position = this.transform.localPosition;
            position.y = (float)val;
            this.transform.localPosition = position;
    }

    private void updateOffsetX(double val)
    {
        Vector3 position = this.transform.localPosition;
        position.x = (float)val;
        this.transform.localPosition = position;
    }

    private void updateVelY(double val) {
        GetComponent<Rigidbody2D>().WakeUp();
        GetComponent<Rigidbody2D>().AddForce(new Vector2(0, Time.deltaTime*(float)val));
        Debug.Log(val);
    }

    private void updateVelX(double val)
    {
        GetComponent<Rigidbody2D>().WakeUp();
        GetComponent<Rigidbody2D>().AddForce(new Vector2(Time.deltaTime*(float)val, 0));
    }

    private void updateRotation(double val)
    {
        Vector3 locRot = transform.localEulerAngles;
        locRot.z = (float)val;
        this.transform.localEulerAngles = locRot;
    }

    public static Texture createTexture(Color color) {
        // Create a new 2x2 texture ARGB32 (32 bit with alpha) and no mipmaps
        var texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        // set the pixel values
        texture.SetPixel(0, 0, color);

        texture.filterMode = FilterMode.Point;

        // Apply all SetPixel calls
        texture.Apply();

        return texture;
    }
}
