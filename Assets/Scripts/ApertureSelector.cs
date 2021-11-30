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

    public Transform nonDominantHand;
    public Transform dominantHand;
    bool isOn = false;

    //Spatial Select Params
    bool setToClosest = false;
    float selectionDepth = 1;
    float startingDepth = 0;
    float alphaHide = 0.5f;

    //Snap Select Params
    float[] snapDepths;
    float planeHeight;
    bool lockedIn = false;
    bool snapMode = false;
    bool snapIntiated = false;
    int curIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        flashlight.gameObject.SetActive(isOn);
        toggleFlashAction.action.performed += ToggleFlash;

        flashlight.onTriggerEntered += OnFlashlightEnter;
        selectionPlane.onTriggerEntered += ShowOutline;

        flashlight.onTriggerExited += OnFlashlightExit;
        selectionPlane.onTriggerExited += HideOutline;

        planeHeight = selectionPlane.transform.localScale.y * flashlight.transform.localScale.y;

        toggleSnapAction.action.performed += ToggleSnap;
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
            snapMode = lockedIn = false;
            foreach (GameObject s in selectionPlane.currentCollisions)
            {
                Debug.Log(s.name);
            }
        }

        flashlight.gameObject.SetActive(isOn);
       
    }

    public void ToggleSnap(InputAction.CallbackContext context)
    {
        if (snapMode && lockedIn)
        {
            ToggleFlash(context);
        }
        else if (snapMode && !lockedIn)
        {
            lockedIn = true;
        }
        else if (!snapMode && isOn)
        {
            snapMode = true;
            // Should change cylinder color and add a spatial UI notification to indicate snapMode on
        }
    }

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

            if (snapMode)
            {
                SnapSelect();
            }
            else
            {
                //Spatial Select
                float depth = nonDominantHand.localPosition.z * 5f;
                if (setToClosest && flashlight.currentCollisions.Count > 0) 
                {
                    // Set inital selectionDepth to closest colliding object
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

    private float[] FindSnapDepths(Transform start, float distBand)
    {
        SortedSet<float> res = new SortedSet<float>();
        foreach (GameObject g in flashlight.currentCollisions)
        {
            float dist = flashlight.transform.InverseTransformPoint(g.transform.position).y;
            int distBucket = (int)(dist / distBand);
            res.Add(distBucket);
        }
        float[] snapDistances = new float[res.Count];
        int i = 0;
        foreach (float f in res)
        {
            snapDistances[i++] = f;
        }
        return snapDistances;
    }

    private void SnapSelect()
    {
        //set locked in
        if (lockedIn)
        {
            float joyStick = snapForwardAction.action.ReadValue<Vector2>().y;
            if (joyStick == 0)
            {
                snapIntiated = false;
            }
            else if (joyStick > 0.8 && !snapIntiated)
            {
                curIndex = (curIndex + 1) % snapDepths.Length;
                snapIntiated = true;
            }
            else if (joyStick < -0.8 && !snapIntiated)
            {
                curIndex = (curIndex + snapDepths.Length - 1) % snapDepths.Length;
                snapIntiated = true;
            }
            Vector3 pos = selectionPlane.transform.localPosition;
            pos.y = snapDepths[curIndex] * planeHeight;
            selectionPlane.transform.localPosition = pos;
        }
        else
        {
            curIndex = 0;
            snapDepths = FindSnapDepths(flashlight.transform,
                planeHeight);
        }
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
