using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApertureSelector : MonoBehaviour
{
    public Cylinder flashlight;
    public Cylinder selectionPlane;
    public InputActionProperty toggleFlashAction;
    public InputActionProperty snapForwardAction;
    public InputActionProperty toggleSnapAction;

    public Transform leftHand;
    public Transform rightHand;
    public Transform torso;
    bool isOn = false;

    //Spatial Select Params
    bool setToClosest = false;
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
        if (isOn) //Just turned on flashlight, configure
        {
            setToClosest = true;
        } 
        else //Just turned off flashlight, doing selection
        {
            foreach (GameObject s in selectionPlane.currentCollisions)
            {
                Debug.Log(s.name);
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
            //Vector3 nonDominantPos = transform.InverseTransformPoint(leftHand.transform.position);

            Vector3 handPos = rightHand.position;
            float handDelta = (handPos - oldHandPos).sqrMagnitude;
            if (handDelta > 0.004)
            //if (snapForwardAction.action.ReadValue<Vector2>().y != 0)
            {
                SnapToNext(handPos);
                oldHandPos = handPos;
            }
            // Set inital selectionDepth to closest colliding object
            // GameObject closest = GetClosest(transform, flashlight.currentCollisions);

            //Vector3 pos = selectionPlane.transform.localPosition;
            //pos.y = flashlight.transform.InverseTransformPoint(closest.transform.position).y;
            //selectionPlane.transform.localPosition = pos;
                
            
        }

    }
    private void SnapToNext(Vector3 handPos)
    {
        float selectionDist = (selectionPlane.transform.position - transform.position).magnitude;
        Vector3 selectionDir = (selectionPlane.transform.position - transform.position).normalized;

        Vector3 pos = selectionPlane.transform.localPosition;
        RaycastHit hitInfo;
        if (Vector3.Distance(handPos, torso.position) > Vector3.Distance(oldHandPos, torso.position))
        //if (snapForwardAction.action.ReadValue<Vector2>().y > 0.2)
        {
            if (Physics.SphereCast(selectionPlane.transform.position, flashlight.radius, 
                selectionDir, out hitInfo, 5, layerMask))
            {
                pos.y = flashlight.transform.InverseTransformPoint(hitInfo.transform.position).y;
                //Debug.Log(hitInfo.collider.gameObject);
            }
        }
        else
        {
            if (Physics.SphereCast(selectionPlane.transform.position, flashlight.radius,
                -selectionDir, out hitInfo, 5, layerMask))
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

    public static GameObject GetClosest(Transform start, IEnumerable<GameObject> lst)
    {
        float closestDist = Mathf.Infinity;
        GameObject bestTarget = null;
        foreach (GameObject s in lst)
        {
            float dist = (s.transform.position - start.position).sqrMagnitude;
            if (dist < closestDist)
            {
                closestDist = dist;
                bestTarget = s;
            }
        }
        return bestTarget;
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
