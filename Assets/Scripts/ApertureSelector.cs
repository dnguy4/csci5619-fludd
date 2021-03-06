using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ApertureSelector : MonoBehaviour
{
    public Cylinder flashlight, selectionPlane;
    public InputActionProperty toggleFlashAction, toggleSpatialMenu, cancelQuadAction;

    public Transform leftHand, rightHand, headset;
    public QuadMenu menu;
    public SpatialMenu tooltips;
    public Slider slider;

    GraspGrabberRight rHandGrabber;
    int stage = 0; //0 is off, 1 is aperture, 2 is quad

    //Spatial Select Params
    Vector3 oldHandPos;
    float alphaHide = 0.5f;
    float flashlightSize;
    public LayerMask layerMask;


    // Start is called before the first frame update
    void Start()
    {
        flashlight.gameObject.SetActive(false);
        toggleFlashAction.action.performed += ToggleFlash;
        toggleSpatialMenu.action.performed += ToggleMenu;
        cancelQuadAction.action.performed += CancelQuad;

        flashlight.onTriggerEntered += OnFlashlightEnter;
        selectionPlane.onTriggerEntered += ShowOutline;

        flashlight.onTriggerExited += OnFlashlightExit;
        selectionPlane.onTriggerExited += HideOutline;

        oldHandPos = rightHand.position;
        rHandGrabber = GetComponent<GraspGrabberRight>();

        flashlightSize = flashlight.transform.localScale.y * 2;
    }

    public void ToggleFlash(InputAction.CallbackContext context)
    {
        stage = stage + 1;
        rHandGrabber.Release(new InputAction.CallbackContext()); // Drop old obj
        if (stage > 2 && menu.DoneYet())
        {
            stage = 0;
            flashlight.gameObject.SetActive(false);
        }

        if (stage == 1) { // Just turned on, starting aperture select
            flashlight.gameObject.SetActive(true);
        }
        else if (stage == 2) //Starting progressive refinement
        {
            if (selectionPlane.currentCollisions.Count > 1) // Quad Menu needed
            {
                menu.gameObject.SetActive(true);
                menu.PopulateQuads(selectionPlane.currentCollisions.ToList());
            }
            else if (selectionPlane.currentCollisions.Count == 1) // No need for quad menu, just grab
            {
                GameObject go = selectionPlane.currentCollisions.ToList()[0];
                rHandGrabber.GrabObject(go);
                stage = 0; //Reset to off stage
            }
            else 
            {
                stage = 0;
            }
            flashlight.gameObject.SetActive(false);
        }
       
    }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        if (tooltips.showing)
            tooltips.HideMenu();
        else
            tooltips.ShowMenu();
    }

    public void CancelQuad(InputAction.CallbackContext context)
    {
        stage = 0;
        menu.gameObject.SetActive(false);
        menu.ClearQuads(-1);
        flashlight.gameObject.SetActive(false);
    }

    void Update()
    {
        if (stage == 1)
        {
            // Aperture Select - Changing Width based on horizontal distance between hands
            float x = Mathf.Abs(transform.InverseTransformPoint(leftHand.position).x);
            if (x != flashlight.radius)
            {
                flashlight.ModifyRadius(x);
            }

            //Spatial Select
            Vector3 handPos = rightHand.position;
            float handDelta = (handPos - oldHandPos).sqrMagnitude;
            if (handDelta > 0.004)
            {
                SnapToNext(handPos);
                oldHandPos = handPos;
                slider.value = 0.5f;
            }
            else
            {
                Vector3 torsoPos = headset.position + 0.5f * Vector3.down;
                float d = Vector3.Distance(handPos, torsoPos) - Vector3.Distance(oldHandPos, torsoPos);
                slider.value = 0.5f + 8 * d;
            }
            
        }

    }
    private void SnapToNext(Vector3 handPos)
    {
        float selectionDist = (selectionPlane.transform.position - transform.position).magnitude;
        Vector3 selectionDir = (selectionPlane.transform.position - transform.position).normalized;

        Vector3 pos = selectionPlane.transform.localPosition;
        Vector3 torsoPos = headset.position + 0.5f * Vector3.down;
        RaycastHit hitInfo;
        if (Vector3.Distance(handPos, torsoPos) > Vector3.Distance(oldHandPos, torsoPos))
        {
            if (Physics.SphereCast(selectionPlane.transform.position, flashlight.radius, 
                selectionDir, out hitInfo, flashlightSize - selectionDist, layerMask))
            {
                pos.y = flashlight.transform.InverseTransformPoint(hitInfo.transform.position).y;
                //Debug.Log("Hit " + hitInfo.collider.gameObject);
            }
        }
        else
        {
            if (Physics.SphereCast(selectionPlane.transform.position, flashlight.radius,
                -selectionDir, out hitInfo, selectionDist, layerMask))
            {
                pos.y = flashlight.transform.InverseTransformPoint(hitInfo.transform.position).y;
                //Debug.Log("Hit " + hitInfo.collider.gameObject);
            }
        }
        pos.y = Mathf.Max(pos.y, -0.86f); //Make sure in front of controller, potential fix for bug
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
