using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileUIManager : MonoBehaviour
{
    public GameObject uiButtonPrefab;
    public int perButtonOffsetY;
    List<GameObject> buttons;
    Dictionary<GameObject, GameObject> popupReferences;

    // Start is called before the first frame update
    void Start()
    {
        buttons = new List<GameObject>();
        popupReferences = new Dictionary<GameObject, GameObject>();
    }

    public void createButton(GameObject popupReference) {
        string text = popupReference.GetComponentInChildren<KeyPopup>().letter;
        //Shift all existing buttons up
        shiftAllButtonsUp();

        //Create new button
        GameObject newButton = Instantiate(uiButtonPrefab, this.transform);
        Rect rect = newButton.GetComponent<RectTransform>().rect;
        newButton.GetComponent<RectTransform>().rect.Set(0, rect.height * buttons.Count, rect.width, rect.height);
        newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -rect.height, 0);
        newButton.GetComponentInChildren<Text>().text = text;
        newButton.name += buttons.Count;

        //Slide it up
        MoveSwitch moveSwitch = newButton.AddComponent<MoveSwitch>();
        moveSwitch.offset = new Vector2(0, rect.height);
        moveSwitch.autoStart = true;
        moveSwitch.time = 1;

        //Fade it in
        OpacitySwitch opacitySwitch = newButton.AddComponent<OpacitySwitch>();
        opacitySwitch.opacity = new Vector2(0, 1);
        opacitySwitch.autoStart = true;
        opacitySwitch.time = 1;

        //On Press
        Button newButtonComponent = newButton.GetComponent<Button>();
        EventTrigger eTrig = newButton.GetComponent<EventTrigger>();
        EventTrigger.Entry entry1 = new EventTrigger.Entry();
        entry1.eventID = EventTriggerType.PointerDown;
        entry1.callback.AddListener(delegate { ButtonPressed(newButton); });
        eTrig.triggers.Add(entry1);

        //On Depress
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerUp;
        entry2.callback.AddListener(delegate { ButtonDepressed(newButton); });
        eTrig.triggers.Add(entry2);

        //Add references to lists
        buttons.Insert(0, newButton);
        popupReferences.Add(popupReference, newButton);
    }

    public void removeButton(GameObject popupReference) {
        GameObject button;
        if (popupReferences.TryGetValue(popupReference, out button)) {
            //Shift all buttons down, also destroys bottom button
            shiftAllButtonsDown(popupReference); 
        }
    }

    private void shiftAllButtonsUp() {
        foreach (GameObject g in buttons) {
            Rect rect = g.GetComponent<RectTransform>().rect;
            g.GetComponent<MoveSwitch>().destroy();
            Debug.Log(g.GetComponentInChildren<Text>().text + " " + buttons.IndexOf(g) + " | " + g.GetComponent<RectTransform>().anchoredPosition);

            MoveSwitch mS = g.AddComponent<MoveSwitch>();
            mS.offset = new Vector2(0, (rect.height * buttons.IndexOf(g) - g.GetComponent<RectTransform>().anchoredPosition.y) + rect.height);
            mS.autoStart = true;
            mS.time = 1;
            Debug.Log("Effect switch added. " + g.name);
        }
    }
    private void shiftAllButtonsDown(GameObject popupReference) {
        for(int i = buttons.Count-1; i >= 0; i--)
        {
            GameObject g = buttons[i];

            Rect rect = g.GetComponent<RectTransform>().rect;
            g.GetComponent<MoveSwitch>().destroy();
            g.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, rect.height * buttons.IndexOf(g), 0);

            GameObject buttonFromRef;
            popupReferences.TryGetValue(popupReference, out buttonFromRef);
            if (g.Equals(buttonFromRef))
            {
                buttons.Remove(g);
                popupReferences.Remove(popupReference);
                i++;

                DestroySwitch dS = g.AddComponent<DestroySwitch>();
                dS.time = 0;

                OpacitySwitch oS = g.AddComponent<OpacitySwitch>();
                oS.opacity = new Vector2(1, 0);
                oS.autoStart = true;
                oS.time = 1;
                oS.consequence = new List<EffectSwitch>();
                oS.consequence.Add(dS);
                break;
            }
            else
            {
                MoveSwitch mS = g.AddComponent<MoveSwitch>();
                mS.offset = new Vector2(0, -rect.height);
                mS.autoStart = true;
                mS.time = 1;
            }

            
        }
    }

    void ButtonPressed(GameObject pressedButton) {
        char c = pressedButton.GetComponentInChildren<Text>().text.ToLower()[0];
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().tryInteract(c);
        Debug.Log("Pressed " + c);
    }

    void ButtonDepressed(GameObject pressedButton)
    {
        char c = pressedButton.GetComponentInChildren<Text>().text.ToLower()[0];
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().tryStopInteract(c);
        Debug.Log("Depressed " + c);
    }
}
