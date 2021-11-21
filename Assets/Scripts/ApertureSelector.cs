using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ApertureSelector : MonoBehaviour
{
    //public float coneAngle;
    //public float coneRange;
    //public LayerMask layerMask;

    public Cone flashlight;
    public InputActionProperty adjustWidth;
    bool isOn = false;

    // Start is called before the first frame update
    void Start()
    {
        //flashlight.gameObject.SetActive(isOn);
    }

    // Update is called once per frame
    void Update()
    {
        float x = adjustWidth.action.ReadValue<Vector2>().x;
        if (Mathf.Abs(x) > 0.2f)
        {
            float newRadius = flashlight.radius + 0.2f * x * Time.deltaTime;
            flashlight.ModifyVertices(newRadius, flashlight.height);
        }
    }

    void ScanCone()
    {
        //Physics.SphereCastAll(transform.position, coneRange, transform.forward, 2f, layerMask);
    }
}
