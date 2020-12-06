using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteEffectsController : MonoBehaviour
{
    private List<SpriteEffects> allSpriteEffects;
    public List<SpriteEffects> activeSpriteEffects;
    private List<SpriteEffects> toRemove;


    // Start is called before the first frame update
    void Start()
    {
        allSpriteEffects = new List<SpriteEffects>();
        activeSpriteEffects = new List<SpriteEffects>();
        toRemove = new List<SpriteEffects>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < activeSpriteEffects.Count; i++) {
            SpriteEffects sE = activeSpriteEffects[i];
            if (!sE.updateEffects())
                toRemove.Add(sE);
        }

        foreach (SpriteEffects r in toRemove)
            activeSpriteEffects.Remove(r);

        toRemove.Clear();
    }

    public void add(SpriteEffects sE) {
        allSpriteEffects.Add(sE);
    }

    public void setActive(SpriteEffects sE) {
        if(!activeSpriteEffects.Contains(sE))
            activeSpriteEffects.Add(sE);
    }
}
