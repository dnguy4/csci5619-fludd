using UnityEngine;
using UnityEngine.InputSystem;

public class GraspGrabberRight : Grabber
{
    public InputActionProperty grabAction;
    public Grabbable grabbedObject;

    Grabbable currentObject;
    Transform initialParent;
    bool triggerPressed;

    // Start is called before the first frame update
    void Start()
    {
        grabbedObject = null;
        currentObject = null;
        triggerPressed = false;
        grabAction.action.performed += Grab;
        grabAction.action.canceled += Release;
    }

    private void OnDestroy()
    {
        grabAction.action.performed -= Grab;
        grabAction.action.canceled -= Release;
    }

    // Update is called once per frame
    void Update()
    {
        //if (grabAction.action.ReadValueAsObject() == null)
        //{
        //    //if (triggerPressed)
        //    //{
        //    //    currentObject.GetCurrentGrabber().Release(new InputAction.CallbackContext());
        //    //}
        //    //triggerPressed = false;
        //}
        //else
        //{
        //    triggerPressed = true;
        //}
    }

    public void GrabSelectableFromMenu(QuadMenu.Selectable selectable)
    {
        GameObject other = selectable.obj;
        if (other.GetComponent<Grabbable>())
        {
            currentObject = other.GetComponent<Grabbable>();

            // Reset scale and rotation
            other.transform.parent = selectable.initialParent;
            other.transform.rotation = selectable.initialRot;
            other.transform.localScale = selectable.initialSca;
            other.GetComponent<Collider>().enabled = true;

            // Move to the hand and "grab"
            GrabObject(other);
        }
    }

    public void GrabObject(GameObject other)
    {
        other.transform.position = transform.position;
        currentObject = other.GetComponent<Grabbable>();
        Release(new InputAction.CallbackContext());
        Grab(new InputAction.CallbackContext());
    }

    public override void Grab(InputAction.CallbackContext context)
    {
        if (currentObject && grabbedObject == null)
        {
            if (currentObject.GetCurrentGrabber() != null)
            {
                currentObject.GetCurrentGrabber().Release(new InputAction.CallbackContext());
            }

            grabbedObject = currentObject;
            grabbedObject.SetCurrentGrabber(this);

            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = true;
                grabbedObject.GetComponent<Rigidbody>().useGravity = false;
            }

            initialParent = grabbedObject.transform.parent;
            grabbedObject.transform.parent = this.transform;
        }
    }

    public override void Release(InputAction.CallbackContext context)
    {
        if (grabbedObject)
        {
            if (grabbedObject.GetComponent<Rigidbody>())
            {
                grabbedObject.GetComponent<Rigidbody>().isKinematic = false;
                grabbedObject.GetComponent<Rigidbody>().useGravity = true;
            }

            grabbedObject.SetCurrentGrabber(null);
            grabbedObject.transform.parent = initialParent;
            grabbedObject = null;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (currentObject == null && other.GetComponent<Grabbable>())
        {
            currentObject = other.gameObject.GetComponent<Grabbable>();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (currentObject)
        {
            if (other.GetComponent<Grabbable>() && currentObject.GetInstanceID() == other.GetComponent<Grabbable>().GetInstanceID())
            {
                currentObject = null;
            }
        }
    }
}