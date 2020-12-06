using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    public List<GameObject> activators;
    public List<EffectSwitch> switches;
    public bool activated;

    private void OnTriggerEnter2D(Collider2D col)
    {
        collisionUpdate(col, true);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if(!activated)
            collisionUpdate(col, true);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        collisionUpdate(col, false);
    }

    private void collisionUpdate(Collider2D col, bool enter) {
        if (activators != null)
        {
            if (activators.Contains(col.transform.gameObject))
            {
                activated = enter;
                foreach (EffectSwitch effectSwitch in switches)
                    effectSwitch.setEnabled(enter);
            }
        }
        else
        {
            activated = enter;
            foreach (EffectSwitch effectSwitch in switches)
                effectSwitch.setEnabled(enter);
        }
    }
}
