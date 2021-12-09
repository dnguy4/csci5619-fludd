using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public class ApertureSelector : MonoBehaviour
{
    public Cylinder flashlight;
    public Cylinder selectionPlane;
    public InputActionProperty toggleFlashAction;
    public InputActionProperty snapForwardAction;

    public Transform leftHand;
    public Transform rightHand;
    public Transform headset;
    public QuadMenu menu;
    bool isOn = false;

    //Spatial Select Params
    Vector3 oldHandPos;
    float alphaHide = 0.5f;
    public LayerMask layerMask;


    // Start is called before the first frame update
    void Start()
    {
        flashlight.gameObject.SetActive(isOn);
        toggleFlashAction.action.performed += ToggleFlash;

        flashlight.onTriggerEntered += OnFlashlightEnter;
        selectionPlane.onTriggerEntered += ShowOutline;

        flashlight.onTriggerExited += OnFlashlightExit;
        selectionPlane.onTriggerExited += HideOutline;

        //planeHeight = selectionPlane.transform.localScale.y * flashlight.transform.localScale.y;

        oldHandPos = rightHand.position;
    }

    public void ToggleFlash(InputAction.CallbackContext context)
    {
        isOn = !isOn;

        menu.gameObject.SetActive(!isOn);
        if (!isOn) //Just turned off flashlight, doing selection. Replace with Grab button probably
        {
            if (selectionPlane.currentCollisions.Count > 1)
            {
                menu.PopulateQuads(selectionPlane.currentCollisions.ToList());
            }
            else
            {
                //Do selection Logic
            }
        }
        flashlight.gameObject.SetActive(isOn);
       
    }

    void Update()
    {
        if (isOn)
        {
            // Aperture Select - Changing Width
            float x = Mathf.Abs(transform.InverseTransformPoint(leftHand.position).x);
            //float x = Mathf.Abs(leftHand.localPosition.x - rightHand.localPosition.x);
            if (x != flashlight.radius)
            {
                flashlight.ModifyRadius(x);
            }

            //Spatial Select
            Vector3 handPos = rightHand.position;
            float handDelta = (handPos - oldHandPos).sqrMagnitude;
            if (handDelta > 0.004)
            //if (snapForwardAction.action.ReadValue<Vector2>().y != 0)
            {
                SnapToNext(handPos);
                oldHandPos = handPos;
            }   
            
        }

    }
    private void SnapToNext(Vector3 handPos)
    {
        float selectionDist = (selectionPlane.transform.position - transform.position).magnitude;
        Vector3 selectionDir = (selectionPlane.transform.position - transform.position).normalized;

        Vector3 pos = selectionPlane.transform.localPosition;
        Vector3 torsoPos = headset.position - 0.5f * Vector3.down;
        RaycastHit hitInfo;
        if (Vector3.Distance(handPos, torsoPos) > Vector3.Distance(oldHandPos, torsoPos))
        {
            if (Physics.SphereCast(selectionPlane.transform.position, flashlight.radius, 
                selectionDir, out hitInfo, 2-selectionDist, layerMask))
            {
                pos.y = flashlight.transform.InverseTransformPoint(hitInfo.transform.position).y;
                //Debug.Log(hitInfo.collider.gameObject);
            }
        }
        else
        {
            if (Physics.SphereCast(selectionPlane.transform.position, flashlight.radius,
                -selectionDir, out hitInfo, selectionDist, layerMask))
            {
                pos.y = flashlight.transform.InverseTransformPoint(hitInfo.transform.position).y;
                //Debug.Log(hitInfo.collider.gameObject);
            }
        } 
        selectionPlane.transform.localPosition = pos;
        
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
        if (flashlight.currentCollisions.Contains(other))
        {
            ModifyOpacity(other, alphaHide);
        }
    }
}
