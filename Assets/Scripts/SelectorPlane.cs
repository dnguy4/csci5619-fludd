using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SelectorPlane : MonoBehaviour
{
	public Action<Collider, Collider> onTriggerEntered;
	public HashSet<GameObject> currentCollisions;

	// Start is called before the first frame update
	void Start()
	{
		currentCollisions = new HashSet<GameObject>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		currentCollisions.Add(other.gameObject);
		Cylinder.ModifyOpacity(other.gameObject, 1f);

		Outline o = other.GetComponent<Outline>();
		Debug.Log(o);
		if (o) o.enabled = true;

		// If there are any listeners...
		if (onTriggerEntered != null)
		{
			// ...broadcast the callback.
			onTriggerEntered(GetComponent<Collider>(), other);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		currentCollisions.Remove(other.gameObject);
		Cylinder.ModifyOpacity(other.gameObject, 0.5f);
		Outline o = other.GetComponent<Outline>();
		if (o) o.enabled = false;
	}

    private void OnDisable()
    {
		currentCollisions.Clear();
    }
}
