using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class BackgroundCirclesColors : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem ps = GetComponent<ParticleSystem>();
        var col = ps.colorOverLifetime;
        col.enabled = true;

        Color a;
        ColorUtility.TryParseHtmlString("#FFE64C", out a);
        Color b;
        ColorUtility.TryParseHtmlString("#FF55BC", out b);

        Gradient grad = new Gradient();
        grad.SetKeys(new GradientColorKey[] { new GradientColorKey(a, 0.265f), new GradientColorKey(b, 0.826F) }, new GradientAlphaKey[] { new GradientAlphaKey(0.0F, 0.0f), new GradientAlphaKey(1.0F, 0.447f), new GradientAlphaKey(1.0f, 0.0f) });

        col.color = grad;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
