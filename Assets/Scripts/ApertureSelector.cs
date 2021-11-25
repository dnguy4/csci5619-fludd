using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApertureSelector : MonoBehaviour
{
    public Cylinder flashlight;
    public Cylinder selectionPlane;
    public InputActionProperty toggleFlashAction;
    public InputActionProperty snapeForwardAction;

    public Transform nonDominantHand;
    public Transform dominantHand;

    bool isOn = false;
    bool setToClosest = false;
    float selectionDepth = 1;
    float startingDepth = 0;
    float alphaHide = 0.5f;

    SortedSet<float> snapDepths;

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
        if (isOn) //Just turned on flashlight
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

    // Update is called once per frame
    void Update()
    {
        if (isOn)
        {
            // Aperture Select - Changing Width
            float x = Mathf.Abs(nonDominantHand.localPosition.x - dominantHand.localPosition.x);
            if (x != flashlight.radius)
            {
                flashlight.ModifyRadius(x);
            }

            float depth = nonDominantHand.localPosition.z * 5f;
            if (setToClosest && flashlight.currentCollisions.Count > 0) // Set inital selectionDepth to closest colliding object
            {
                GameObject closest = GetClosest(transform, flashlight.currentCollisions);
                startingDepth = flashlight.transform.InverseTransformPoint(closest.transform.position).y - depth;
                setToClosest = false;
            }

            if (startingDepth + depth != selectionDepth)
            {
                selectionDepth = startingDepth + depth;
                Vector3 pos = selectionPlane.transform.localPosition;
                pos.y = selectionDepth;
                selectionPlane.transform.localPosition = pos;
            }
        }

    }

    private SortedDictionary<int, List<GameObject>> FindSnapBuckets(Transform start, float distBand)
    {
        SortedDictionary<int, List<GameObject>> res = new SortedDictionary<int, List<GameObject>>();
        foreach (GameObject g in flashlight.currentCollisions)
        {
            float dist = flashlight.transform.InverseTransformPoint(g.transform.position).y;
            int distBucket = (int)(dist / distBand);
            if (res.ContainsKey(distBucket))
            {
                res[distBucket].Add(g);
            } else
            {
                res.Add(distBucket, new List<GameObject>());
                res[distBucket].Add(g);
            }
        }
        return res;
    }

    private SortedSet<float> FindSnapDepths(Transform start, float distBand)
    {
        SortedSet<float> res = new SortedSet<float>;
        foreach (GameObject g in flashlight.currentCollisions)
        {
            float dist = flashlight.transform.InverseTransformPoint(g.transform.position).y;
            int distBucket = (int)(dist / distBand);
            res.Add(distBucket);
        }
        return res;
    }

    private void SnapSelect()
    {
        bool lockedIn = false;
        int maxIndex = snapDepths.Count - 1;
        int curIndex = 0;
        if (lockedIn)
        {
            
        }
        snapDepths = FindSnapDepths(flashlight.transform, 
            selectionPlane.transform.localScale.y * flashlight.transform.localScale.y);
        
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
        ModifyOpacity(other, alphaHide);
    }
}
