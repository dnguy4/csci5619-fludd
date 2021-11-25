using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApertureSelector : MonoBehaviour
{
    public Cylinder flashlight; //capsule instead of cone for simplicity?
    public Transform selectionPlane;
    public InputActionProperty toggleFlashAction;

    public Transform nonDominantHand;
    public Transform dominantHand;
    bool isOn = false;

    float selectionDepth = 1;

    // Start is called before the first frame update
    void Start()
    {
        flashlight.gameObject.SetActive(isOn);
        toggleFlashAction.action.performed += ToggleFlash;
    }

    public void ToggleFlash(InputAction.CallbackContext context)
    {
        isOn = !isOn;
        flashlight.gameObject.SetActive(isOn);
        if (isOn)
        {
            flashlight.onTriggerEntered += OnTriggerEntered;
        }
        else
        {
            flashlight.onTriggerEntered -= OnTriggerEntered;
        }
       
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
                Vector3 pos = selectionPlane.localPosition;
                pos.y = selectionDepth;
                selectionPlane.localPosition = pos;
                selectionDepth = depth;
            }
        }

    }

    void ScanCone()
    {
        //Physics.SphereCastAll(transform.position, coneRange, transform.forward, 2f, layerMask);
    }
    private void OnTriggerEntered(Collider trigger, Collider collider)
    {

    }
}
