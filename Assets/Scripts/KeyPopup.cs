using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(SwitchTrigger))]
public class KeyPopup : MonoBehaviour
{
    public GameObject keyPopupPrefab;
    public Vector2 relativeOffset;
    private GameObject popup;
    public bool showing;
    public Collider2D triggercollider;
    private SwitchTrigger trigger;
    public string letter;
    public enum Interactions { Focus = 0, None = 1 , PullOpposite}
    public Interactions interaction;

    public GameObject objectToPull;


    // Start is called before the first frame update
    void Start()
    {
        GameObject popup = Instantiate(keyPopupPrefab);
        popup.transform.SetParent(this.transform);
        popup.transform.localPosition = relativeOffset;
        trigger = GetComponent<SwitchTrigger>();
        if (trigger.switches == null)
            trigger.switches = new List<EffectSwitch>();

        foreach (EffectSwitch s in popup.GetComponentsInChildren<EffectSwitch>())
        {
            trigger.switches.Add(s);
        }

        triggercollider.isTrigger = true;
        trigger.activators = new List<GameObject>();
        trigger.activators.Add(GameObject.FindGameObjectWithTag("Player"));
        popup.GetComponentInChildren<TextMeshPro>().text = letter;
    }

    void OnDestroy() {
        try
        {
            setEnabled(false);
        }
        catch (System.Exception e) { }
    }

    IEnumerator WaitForPlayerController()
    {
        while (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().interactions == null)
        {
            yield return new WaitForSeconds(.1f);
        }
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().interactions.Add(letter.ToLower()[0], this.gameObject);
        yield return null;
    }

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.Equals(GameObject.FindGameObjectWithTag("Player")))
        {
            showing = true;
            setEnabled(true);
        }
    }

    void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.Equals(GameObject.FindGameObjectWithTag("Player")))
        {
            showing = false;
            setEnabled(false);
        }
    }

    private void setEnabled(bool enabled) {
        if (enabled)
        {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().addInteraction(letter.ToLower()[0], this.gameObject);
        }
        else
        {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().removeInteraction(letter.ToLower()[0]); 
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(KeyPopup))]
public class MyScriptEditor : Editor
{
    override
    public void OnInspectorGUI()
    {
        var _script = target as KeyPopup;

        _script.keyPopupPrefab = (GameObject) EditorGUILayout.ObjectField("Key Popup Prefab", _script.keyPopupPrefab, typeof(GameObject), false);
        _script.relativeOffset = EditorGUILayout.Vector2Field("Relative Offset", _script.relativeOffset);
        _script.showing = EditorGUILayout.Toggle("Showing", _script.showing);
        _script.triggercollider = (Collider2D)EditorGUILayout.ObjectField("Trigger Collider", _script.triggercollider, typeof(Collider2D), true);
        _script.letter = EditorGUILayout.TextField("Letter", _script.letter);
        _script.interaction = (KeyPopup.Interactions)EditorGUILayout.EnumPopup(_script.interaction);

        if (_script.interaction == KeyPopup.Interactions.PullOpposite)
        {
            _script.objectToPull = (GameObject)EditorGUILayout.ObjectField("Object To Pull", _script.objectToPull, typeof(GameObject), true);
        }
    }
}
#endif