using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApertureSelector : MonoBehaviour
{
    public Cylinder flashlight;
    public Cylinder selectionPlane;
    public InputActionProperty toggleFlashAction;

    public Transform nonDominantHand;
    public Transform dominantHand;
    bool isOn = false;

    float selectionDepth = 1;
    float alphaHide = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        flashlight.gameObject.SetActive(isOn);
        toggleFlashAction.action.performed += ToggleFlash;

        flashlight.onTriggerEntered += OnFlashlightEnter;
        selectionPlane.onTriggerEntered += ShowOutline;

        flashlight.onTriggerExited += OnFlashlightExit;
        selectionPlane.onTriggerExited += HideOutline;
    }

    public void ToggleFlash(InputAction.CallbackContext context)
    {
        isOn = !isOn;
        if (!isOn) //Just turned off flashlight, grab selection
        { 
            foreach (GameObject s in selectionPlane.currentCollisions)
            {
                Debug.Log(s.name);
            }
        }

        flashlight.gameObject.SetActive(isOn);
       
    }

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            float x = Mathf.Abs(nonDominantHand.localPosition.x - dominantHand.localPosition.x);
            float depth = nonDominantHand.localPosition.z * 5f;
            if (x != flashlight.radius)
            {
                flashlight.ModifyRadius(x);
            }

            if (depth != selectionDepth)
            {
                Vector3 pos = selectionPlane.transform.localPosition;
                pos.y = selectionDepth;
                selectionPlane.transform.localPosition = pos;
                selectionDepth = depth;
            }
        }

    }

    public static void ModifyOpacity(GameObject obj, float newOpacity)
    {
        Renderer r = obj.GetComponent<Renderer>();
        Color oldColor = r.material.color;
        Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, newOpacity);
        r.material.SetColor("_Color", newColor);
    }

    private void OnFlashlightEnter(GameObject trigger, GameObject other)
    {
        ModifyOpacity(other, alphaHide);
    }
    private void OnFlashlightExit(GameObject trigger, GameObject other)
    {
        ModifyOpacity(other, 1);
    }

    private void ShowOutline(GameObject trigger, GameObject other)
    {
        Outline o = other.GetComponent<Outline>();
        if (o) o.enabled = true;
        ModifyOpacity(other, 1);
    }

    private void HideOutline(GameObject trigger, GameObject other)
    {
        Outline o = other.GetComponent<Outline>();
        if (o) o.enabled = false;
        ModifyOpacity(other, alphaHide);
    }
}
